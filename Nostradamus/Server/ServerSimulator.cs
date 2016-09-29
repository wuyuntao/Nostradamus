using Nostradamus.Client;
using System.Collections.Generic;

namespace Nostradamus.Server
{
	public sealed class ServerSimulator : Simulator
	{
		private int time;
		private ServerSyncFrame serverSyncFrame;
		private readonly Queue<ClientSyncFrame> clientSyncFrames = new Queue<ClientSyncFrame>();
		private readonly Dictionary<ActorId, ActorContext> actorContexts = new Dictionary<ActorId, ActorContext>();

		public ServerSimulator(Scene scene)
			: base(scene)
		{
			Scene.OnActorAdded += Scene_OnActorAdded;
			Scene.OnEventCreated += Scene_OnEventCreated;
		}

		protected override void DisposeManaged()
		{
			foreach (var actorContext in actorContexts.Values)
				actorContext.Dispose();

			actorContexts.Clear();

			base.DisposeManaged();
		}

		public void AddClientSyncFrame(ClientSyncFrame frame)
		{
			clientSyncFrames.Enqueue(frame);
		}

		public ServerSyncFrame Simulate(int deltaTime)
		{
			serverSyncFrame = new ServerSyncFrame(time, deltaTime);

			Scene.Time = time;
			Scene.DeltaTime = deltaTime;

			while (clientSyncFrames.Count > 0)
			{
				var frame = clientSyncFrames.Dequeue();
				var lastCommandSeq = serverSyncFrame.GetLastCommandSeq(frame.ClientId);

				foreach (var command in frame.Commands)
				{
					if (command.Sequence <= lastCommandSeq)
					{
						logger.Warn("Received unordered command: {0} <= {1}", command.Sequence, lastCommandSeq);
						continue;
					}

					var actorContext = GetActorContext(command.ActorId);
					if (actorContext != null)
					{
						actorContext.Actor.OnCommandReceived(command.GetArgs());
					}
					else
						logger.Warn("Cannot find actor '{0}'  of command {1}", command.ActorId, command.Args);

					lastCommandSeq = command.Sequence;
				}

				if (lastCommandSeq > 0)
					serverSyncFrame.LastCommandSeqs[frame.ClientId] = lastCommandSeq;
			}

			Scene.OnUpdate();

			time += deltaTime;

			return serverSyncFrame;
		}
		private void Scene_OnActorAdded(Actor actor)
		{
			actorContexts.Add(actor.Id, new ActorContext(actor));
		}

		private void Scene_OnEventCreated(Event @event)
		{
			serverSyncFrame.Events.Add(@event);
		}

		private ActorContext GetActorContext(ActorId id)
		{
			ActorContext sync;
			actorContexts.TryGetValue(id, out sync);
			return sync;
		}
	}
}
