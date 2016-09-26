using NLog;
using Nostradamus.Server;
using System.Collections.Generic;

namespace Nostradamus.Client
{
	public sealed class ClientSimulator
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		private readonly Scene scene;
		private readonly int clientId;
		private int time;
		private ClientSynchronizationFrame clientSyncFrame;
		private Queue<ServerSynchronizationFrame> serverSyncFrames = new Queue<ServerSynchronizationFrame>();

		public ClientSimulator(Scene scene, int clientId)
		{
			this.scene = scene;
			this.clientId = clientId;
		}

		public void AddServerSyncFrame(ServerSynchronizationFrame frame)
		{
			serverSyncFrames.Enqueue(frame);
		}

		public ClientSynchronizationFrame Update(int deltaTime)
		{
			clientSyncFrame = new ClientSynchronizationFrame(clientId, time);

			scene.Time = time;
			scene.DeltaTime = deltaTime;

			// TODO

			time += deltaTime;
			return clientSyncFrame;
		}
	}
}
