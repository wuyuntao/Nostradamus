using NLog;
using Nostradamus.Server;
using System.Collections.Generic;

namespace Nostradamus.Client
{
	public sealed class ClientSimulator
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		private readonly Scene scene;
		private readonly ClientId clientId;
		private int time;
		private ClientSynchronizationFrame clientSyncFrame;
		private Queue<ServerSynchronizationFrame> serverSyncFrames = new Queue<ServerSynchronizationFrame>();
		private int maxCommandSequence;
		private Queue<Command> unacknowledgedCommands = new Queue<Command>();
		private Dictionary<ActorId, ActorSynchronization> actorSyncs = new Dictionary<ActorId, ActorSynchronization>();

		public ClientSimulator(Scene scene, ClientId clientId)
		{
			this.scene = scene;
			this.clientId = clientId;
			this.clientSyncFrame = new ClientSynchronizationFrame(clientId, 0);
		}

		public void AddServerSyncFrame(ServerSynchronizationFrame frame)
		{
			serverSyncFrames.Enqueue(frame);
		}

		public void AddCommand(ActorId actorId, ICommandArgs commandArgs)
		{
			var command = new Command(actorId, scene.Time, scene.DeltaTime, ++maxCommandSequence, commandArgs);

			clientSyncFrame.Commands.Add(command);
		}

		public ClientSynchronizationFrame Update(int deltaTime)
		{
			ProcessServerSyncFrames();

			clientSyncFrame = new ClientSynchronizationFrame(clientId, time);

			scene.Time = time;
			scene.DeltaTime = deltaTime;

			foreach (var command in clientSyncFrame.Commands)
			{
				var actor = scene.GetActor(command.ActorId);
				if (actor != null)
				{
					actor.OnCommandReceived(command.Args);

					unacknowledgedCommands.Enqueue(command);
				}
				else
					logger.Warn("Cannot find actor '{0}'  of command {1}", command.ActorId, command.Args);
			}

			scene.OnUpdate();

			time += deltaTime;

			var frame = clientSyncFrame;
			clientSyncFrame = new ClientSynchronizationFrame(clientId, time);
			return frame;
		}

		private void ProcessServerSyncFrames()
		{
			if (serverSyncFrames.Count == 0)
				return;

			while (serverSyncFrames.Count > 0)
			{
				var frame = serverSyncFrames.Dequeue();

				var lastCommandSeq = frame.GetLastCommandSequence(clientId);
				var lastCommandTime = 0;

				if (lastCommandSeq > 0 && unacknowledgedCommands.Count > 0)
				{
					while (unacknowledgedCommands.Count > 0 && unacknowledgedCommands.Peek().Sequence <= lastCommandSeq)
					{
						var command = unacknowledgedCommands.Dequeue();
						lastCommandTime = command.Time + command.DeltaTime;
					}
				}

				foreach (var @event in frame.Events)
				{
					var actorSync = GetActorSync(@event.ActorId);
					if (actorSync != null)
						actorSync.AddAuthoritativeEvent(@event);
					else
						logger.Warn("Cannot find actor '{0}' of event {1}", @event.ActorId, @event.Args);
				}

				scene.Time = frame.Time;
				scene.DeltaTime = frame.DeltaTime;

				var reconciliation = false;
				foreach (var actorSync in actorSyncs.Values)
				{
					actorSync.AddAuthoritativeTimepoint();

					if (lastCommandTime > 0)
						reconciliation = reconciliation || actorSync.CheckReconciliation();
				}

				if (reconciliation)
				{
					foreach (var actorSync in actorSyncs.Values)
					{
						actorSync.RollbackPredictiveSnapshot(lastCommandTime);
					}

					var reconciliationDeltaTime = 50;
					for (var time = lastCommandTime; time < this.time; time += reconciliationDeltaTime)
					{
						// Replay unacknowledged commands
					}
				}
			}
		}

		private ActorSynchronization GetActorSync(ActorId id)
		{
			ActorSynchronization sync;
			actorSyncs.TryGetValue(id, out sync);
			return sync;
		}
	}
}
