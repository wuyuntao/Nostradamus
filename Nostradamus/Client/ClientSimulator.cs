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
		private int time;
		private ClientSyncFrame clientSyncFrame;
		private Queue<ServerSyncFrame> serverSyncFrames = new Queue<ServerSyncFrame>();
		private int maxCommandSeq;
		private Queue<Command> unacknowledgedCommands = new Queue<Command>();
		private readonly Dictionary<ActorId, ActorContext> actorContexts = new Dictionary<ActorId, ActorContext>();

		public ClientSimulator(Scene scene, ClientId clientId, int reconciliationDeltaTime)
			: base(scene)
		{
			this.clientId = clientId;
			this.reconciliationDeltaTime = reconciliationDeltaTime;
			this.clientSyncFrame = new ClientSyncFrame(clientId, 0);

			scene.OnActorAdded += Scene_OnActorAdded;
		}

		protected override void DisposeManaged()
		{
			foreach (var actorContext in actorContexts.Values)
				actorContext.Dispose();

			actorContexts.Clear();

			base.DisposeManaged();
		}

		public void AddServerSyncFrame(ServerSyncFrame frame)
		{
			serverSyncFrames.Enqueue(frame);
		}

		public void AddCommand(ActorId actorId, ICommandArgs commandArgs)
		{
			var command = new Command(actorId, Scene.Time, Scene.DeltaTime, ++maxCommandSeq, commandArgs);

			clientSyncFrame.Commands.Add(command);
		}

		public ClientSyncFrame Simulate(int deltaTime)
		{
			if (serverSyncFrames.Count > 0)
				Synchronize();

			Predict(clientSyncFrame, time, deltaTime, false);

			time += deltaTime;

			var frame = clientSyncFrame;
			clientSyncFrame = new ClientSyncFrame(clientId, time);
			return frame;
		}

		private void Synchronize()
		{
			int lastCommandSeq = 0;
			while (serverSyncFrames.Count > 0)
			{
				var frame = serverSyncFrames.Dequeue();
				lastCommandSeq = frame.GetLastCommandSeq(clientId);

				Synchronize(frame);
			}

			if (lastCommandSeq == 0)
				return;

			var lastCommandTime = DequeueAcknowledgedCommands(lastCommandSeq);

			if (IsSynchronized(lastCommandTime))
				return;

			RewindAndReplay(lastCommandTime);
		}

		private void Synchronize(ServerSyncFrame frame)
		{
			Scene.Time = frame.Time;
			Scene.DeltaTime = frame.DeltaTime;

			foreach (var @event in frame.Events)
			{
				var actorContext = GetActorContext(@event.ActorId);
				if (actorContext != null)
					actorContext.AddAuthoritativeEvent(@event);
				else
					logger.Warn("Cannot find actor '{0}' of event {1}", @event.ActorId, @event.Args);
			}

			foreach (var actorContext in actorContexts.Values)
			{
				actorContext.AddAuthoritativeTimepoint();
			}
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

		private bool IsSynchronized(int time)
		{
			return actorContexts.Values.All(a => a.IsSynchronized(time));
		}

		private void RewindAndReplay(int time)
		{
			bool replay = false;
			foreach (var actorContext in actorContexts.Values)
			{
				replay = actorContext.Rewind(time) || replay;
			}

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
			for (; time < this.time; time += deltaTime)
			{
				if (time + deltaTime > this.time)
					deltaTime = this.time - time;

				var syncFrame = new ClientSyncFrame(clientId, time);

				if (commands != null)
				{
					while (commands.Count > 0 && commands.Peek().Time <= time)
					{
						var command = commands.Dequeue();
						var newCommand = new Command(command.ActorId, time, deltaTime, command.Sequence, command.Args);

						syncFrame.Commands.Add(newCommand);
					}
				}

				Predict(syncFrame, time, deltaTime, true);
			}
		}

		private void Predict(ClientSyncFrame syncFrame, int time, int deltaTime, bool isReplay)
		{
			Scene.Time = time;
			Scene.DeltaTime = deltaTime;

			foreach (var command in syncFrame.Commands)
			{
				var actorContext = GetActorContext(command.ActorId);
				if (actorContext != null)
				{
					command.Time = time;
					command.DeltaTime = deltaTime;

					actorContext.AddPredictiveCommand(command);

					unacknowledgedCommands.Enqueue(command);
				}
				else
					logger.Warn("Cannot find actor '{0}'  of command {1}", command.ActorId, command.Args);
			}

			Scene.OnUpdate();

			foreach (var actorContext in actorContexts.Values)
			{
				actorContext.AddPredictiveTimepoint(time + deltaTime, isReplay);
			}
		}

		private void Scene_OnActorAdded(Actor actor)
		{
			var actorContext = new ActorContext(actor, Scene.Time, actor.Snapshot);

			actorContexts.Add(actor.Id, actorContext);
		}

		private ActorContext GetActorContext(ActorId id)
		{
			ActorContext sync;
			actorContexts.TryGetValue(id, out sync);
			return sync;
		}
	}
}
