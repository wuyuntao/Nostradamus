using Nostradamus.Client;
using Nostradamus.Server;
using Nostradamus.Tests.Commnads;
using Nostradamus.Tests.Scenes;
using Nostradamus.Tests.Snapshots;
using NUnit.Framework;

namespace Nostradamus.Tests.Client
{
	public class ClientSimulatorTest
	{
		[Test]
		public void TestSimpleScene()
		{
			var clientId = new ClientId(1);
			var scene = new SimpleScene();
			var simulator = new ClientSimulator(scene, clientId);

			// Time: 0 - 20
			var serverFrame0 = new ServerSyncFrame(0, 50);
			simulator.AddServerSyncFrame(serverFrame0);

			var frame0 = simulator.Update(20);
			Assert.AreEqual(0, scene.Time);
			Assert.AreEqual(20, scene.DeltaTime);
			Assert.AreEqual(0, frame0.Time);
			Assert.AreEqual(0, frame0.Commands.Count);

			var snapshot20 = (ActorSnapshot)scene.Character.Snapshot;
			Assert.AreEqual(0f, snapshot20.PositionX);
			Assert.AreEqual(0f, snapshot20.PositionY);

			var command = new MoveActorCommand() { DeltaX = 1, DeltaY = 1 };
			simulator.AddCommand(scene.Character.Id, command);

			// Time : 20 - 40
			var frame20 = simulator.Update(20);
			Assert.AreEqual(20, scene.Time);
			Assert.AreEqual(20, scene.DeltaTime);
			Assert.AreEqual(20, frame20.Time);
			Assert.AreEqual(1, frame20.Commands.Count);

			var snapshot40 = (ActorSnapshot)scene.Character.Snapshot;
			Assert.AreEqual(0.02f, snapshot40.PositionX);
			Assert.AreEqual(0.02f, snapshot40.PositionY);

		}
	}
}
