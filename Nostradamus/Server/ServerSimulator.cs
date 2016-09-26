using NLog;
using Nostradamus.Client;
using System.Collections.Generic;

namespace Nostradamus.Server
{
	public sealed class ServerSimulator
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		private readonly Scene scene;
		private int time;
		private ServerSynchronizationFrame serverSyncFrame;
		private Queue<ClientSynchronizationFrame> clientSyncFrames = new Queue<ClientSynchronizationFrame>();

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

				foreach (var command in frame.Commands)
				{
					var actor = scene.GetActor(command.ActorId);
					if (actor != null)
						actor.OnCommandReceived(command.Args);
					else
						logger.Warn("Cannot find actor '{0}'  of command {1}", command.ActorId, command.Args);
				}
			}

			scene.OnUpdate();

			time += deltaTime;

			return serverSyncFrame;
		}
	}
}
