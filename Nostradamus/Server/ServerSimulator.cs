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
		private ServerSynchronizationFrame serverSyncFrame;
		private readonly Queue<ClientSynchronizationFrame> clientSyncFrames = new Queue<ClientSynchronizationFrame>();

		public ServerSimulator(Scene scene)
		{
			this.scene = scene;
		}

		public void AddClientSyncFrame(ClientSynchronizationFrame frame)
		{
			clientSyncFrames.Enqueue(frame);
		}

		public ServerSynchronizationFrame Update(int deltaTime)
		{
			serverSyncFrame = new ServerSynchronizationFrame(time, deltaTime);

			scene.Time = time;
			scene.DeltaTime = deltaTime;

			while (clientSyncFrames.Count > 0)
			{
				var frame = clientSyncFrames.Dequeue();
				var lastCommandSeq = serverSyncFrame.GetLastCommandSequence(frame.ClientId);

				foreach (var command in frame.Commands)
				{
					if (command.Sequence <= lastCommandSeq)
					{
						logger.Warn("Received unorderred command: {0} <= {1}", command.Sequence, lastCommandSeq);
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
					serverSyncFrame.LastCommandSequences[frame.ClientId] = lastCommandSeq;
			}

			scene.OnUpdate();

			time += deltaTime;

			return serverSyncFrame;
		}
	}
}
