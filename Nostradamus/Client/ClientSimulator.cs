using Nostradamus.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nostradamus.Client
{
	public sealed class ClientSimulator : Simulator
	{
		private readonly ClientId clientId;
		private readonly int reconciliationDeltaTime;

		private Queue<DeltaSyncFrame> deltaSyncFrames = new Queue<DeltaSyncFrame>();
		private int maxCommandSeq;
		private CommandFrame commandFrame;
		private Queue<Command> unacknowledgedCommands = new Queue<Command>();

		public ClientSimulator(ClientId clientId, int reconciliationDeltaTime)
		{
			this.clientId = clientId;
			this.reconciliationDeltaTime = reconciliationDeltaTime;
		}

		internal override ActorContext CreateActorContext(Actor actor)
		{
			return new ClientActorContext(actor);
		}

		public void ReceiveDeltaSyncFrame(DeltaSyncFrame frame)
		{
			deltaSyncFrames.Enqueue(frame);
		}

		public void ReceiveCommand(ActorId actorId, ICommandArgs commandArgs)
		{
			var command = new Command(clientId, actorId, ++maxCommandSeq, Scene.Time, Scene.DeltaTime, commandArgs);

			if (commandFrame == null)
				commandFrame = new CommandFrame(clientId);

			commandFrame.Commands.Add(command);
		}

		public void Simulate(int deltaTime)
		{
			if (deltaSyncFrames.Count > 0)
				Synchronize();

			UpdateScene(commandFrame != null ? commandFrame.Commands : null, Time, deltaTime, false);

			Time += deltaTime;
		}

		private void Synchronize()
		{
			int lastFrameTime = 0;
			int lastCommandSeq = 0;
			while (deltaSyncFrames.Count > 0)
			{
				var frame = deltaSyncFrames.Dequeue();
				lastFrameTime = frame.Time + frame.DeltaTime;

				UpdateAuthoritativeTimeline(frame);

				int commandSeq;
				if (frame.LastCommandSeqs.TryGetValue(clientId, out commandSeq))
					lastCommandSeq = commandSeq;
			}

			if (lastCommandSeq == 0)
				return;

			var lastCommandTime = DequeueAcknowledgedCommands(lastCommandSeq);

			if (ActorContexts.Any(context => !context.IsSynchronized(lastFrameTime, lastCommandTime)))
			{
				RewindAndReplay(lastFrameTime, lastCommandTime);
			}
		}

		private void UpdateAuthoritativeTimeline(DeltaSyncFrame frame)
		{
			Scene.Time = frame.Time;
			Scene.DeltaTime = frame.DeltaTime;

			var eventsByActorId = from e in frame.Events
								  group e by e.ActorId into g
								  select g;

			foreach (var events in eventsByActorId)
			{
				var actorContext = Scene.GetActorContext(events.Key) as ClientActorContext;
				if (actorContext == null)
				{
					logger.Warn("Cannot find actor '{0}'", events.Key);
					continue;
				}

				actorContext.CreateAuthoritativeTimepoint(events);
			}

			logger.Debug("Update authoritative timeline to {0} / {1}", frame.Time, frame.DeltaTime);
		}

		private int DequeueAcknowledgedCommands(int lastCommandSeq)
		{
			while (unacknowledgedCommands.Count > 0 && unacknowledgedCommands.Peek().Sequence <= lastCommandSeq)
			{
				var command = unacknowledgedCommands.Dequeue();

				if (command.Sequence == lastCommandSeq)
					return command.Time + command.DeltaTime;
			}

			throw new InvalidOperationException(string.Format("Cannot find acknowledged command #{0}", lastCommandSeq));
		}

		private void RewindAndReplay(int lastFrameTime, int lastCommandTime)
		{
			bool replay = false;
			foreach (var context in ActorContexts)
			{
				replay = context.Rollback(lastFrameTime, lastCommandTime) || replay;
			}

			logger.Debug("Rollback predictive timeline to {0} / {1}", lastFrameTime, lastCommandTime);

			if (!replay)
				return;

			Queue<Command> commands;
			if (unacknowledgedCommands.Count > 0)
			{
				commands = unacknowledgedCommands;
				unacknowledgedCommands = new Queue<Command>();
			}
			else
				commands = null;

			var deltaTime = reconciliationDeltaTime;
			for (; lastCommandTime < Time; lastCommandTime += deltaTime)
			{
				if (lastCommandTime + deltaTime > Time)
					deltaTime = Time - lastCommandTime;

				var commandsDequeued = commands != null ? DequeueCommands(commands, lastCommandTime) : null;
				UpdateScene(commandsDequeued, lastCommandTime, deltaTime, true);
			}
		}

		private IEnumerable<Command> DequeueCommands(Queue<Command> commands, int time)
		{
			while (commands.Count > 0 && commands.Peek().Time <= time)
			{
				yield return commands.Dequeue();
			}
		}

		private void UpdateScene(IEnumerable<Command> commands, int time, int deltaTime, bool isReplay)
		{
			if (commands != null)
			{
				foreach (var command in commands)
				{
					var actorContext = Scene.GetActorContext(command.ActorId) as ClientActorContext;
					if (actorContext != null)
					{
						command.Time = time;
						command.DeltaTime = deltaTime;

						actorContext.EnqueuePredictiveCommand(command);

						if (!isReplay)
						{
							unacknowledgedCommands.Enqueue(command);
						}
					}
					else
					{
						logger.Warn("Cannot find actor '{0}'  of command {1}",
								command.ActorId, command.Args);
					}
				}
			}

			Scene.Update(time, deltaTime);

			foreach (var context in ActorContexts)
			{
				context.CreatePredictiveTimepoint(time + deltaTime, isReplay);
			}

			logger.Debug("Update scene to {0} / {1}. Replay: {2}", time, deltaTime, isReplay);
		}

		public CommandFrame FetchCommandFrame()
		{
			var frame = commandFrame;

			commandFrame = null;

			return frame;
		}

		private IEnumerable<ClientActorContext> ActorContexts
		{
			get
			{
				return from context in Scene.ActorContexts
					   select (ClientActorContext)context;
			}
		}
	}
}
