using Nostradamus.Networking;
using Nostradamus.Server;
using Nostradamus.Tests.Scenes;
using System;

namespace Nostradamus.Examples
{
	class SimpleServerExample
	{
		public static void Run()
		{
			var simulator = new ServerSimulator();
			var scene = new SimplePhysicsScene(simulator);
			var server = new ReliableUdpServer(simulator, 50, 9000);
			server.Start();

			Console.ReadLine();
			server.Stop();
		}
	}
}
