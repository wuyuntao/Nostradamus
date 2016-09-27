using NLog;
using Nostradamus.Client;
using System.Collections.Generic;

namespace Nostradamus.Server
{
	public sealed class ServerSimulator
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		private readonly Scene scene;
		private int time;
		private ServerSyncFrame serverSyncFrame;
		private readonly Queue<ClientSyncFrame> clientSyncFrames = new Queue<ClientSyncFrame>();

		public ServerSimulator(Scene scene)
		{
			this.scene = scene;
			this.scene.OnEventCreated += Scene_OnEventCreated;
		}

		public void AddClientSyncFrame(ClientSyncFrame frame)
		{
			clientSyncFrames.Enqueue(frame);
		}

		public ServerSyncFrame Update(int deltaTime)
		{
			serverSyncFrame = new ServerSyncFrame(time, deltaTime);

			scene.Time = time;
			scene.DeltaTime = deltaTime;

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

					var actor = scene.GetActor(command.ActorId);
					if (actor != null)
					{
						actor.OnCommandReceived(command.Args);
					}
					else
						logger.Warn("Cannot find actor '{0}'  of command {1}", command.ActorId, command.Args);

					lastCommandSeq = command.Sequence;
				}

				if (lastCommandSeq > 0)
					serverSyncFrame.LastCommandSeqs[frame.ClientId] = lastCommandSeq;
			}

			scene.OnUpdate();

			time += deltaTime;

			return serverSyncFrame;
		}

		private void Scene_OnEventCreated(Event @event)
		{
			serverSyncFrame.Events.Add(@event);
		}
	}
}
