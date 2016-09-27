using Nostradamus.Client;
using Nostradamus.Server;
using Nostradamus.Tests.Commnads;
using Nostradamus.Tests.Scenes;
using Nostradamus.Tests.Snapshots;
using NUnit.Framework;

namespace Nostradamus.Tests.Server
{
	public class ServerSimulatorTest
	{
		[Test]
		public void TestSimpleScene()
		{
			var scene = new SimpleScene();
			var simulator = new ServerSimulator(scene);

			// Time: 0 - 20
			var frame0 = simulator.Update(20);
			Assert.AreEqual(0, scene.Time);
			Assert.AreEqual(20, scene.DeltaTime);
			Assert.AreEqual(0, frame0.Time);
			Assert.AreEqual(20, frame0.DeltaTime);
			Assert.AreEqual(0, frame0.Events.Count);

			var snapshot20 = (ActorSnapshot)scene.Character.Snapshot;
			Assert.AreEqual(0, snapshot20.PositionX);
			Assert.AreEqual(0, snapshot20.PositionY);

			var commandArgs = new MoveActorCommand() { DeltaX = 1, DeltaY = 1 };
			var command = new Command(scene.Character.Id, 20, 20, 1, commandArgs);
			var clientId = new ClientId(1);
			var clientFrame = new ClientSyncFrame(clientId, 20);
			clientFrame.Commands.Add(command);
			simulator.AddClientSyncFrame(clientFrame);

			// Time: 20 - 40
			var frame20 = simulator.Update(20);
			Assert.AreEqual(20, scene.Time);
			Assert.AreEqual(20, scene.DeltaTime);
			Assert.AreEqual(20, frame20.Time);
			Assert.AreEqual(20, frame20.DeltaTime);
			Assert.AreEqual(1, frame20.Events.Count);

			var snapshot40 = (ActorSnapshot)scene.Character.Snapshot;
			Assert.AreEqual(0.02f, snapshot40.PositionX);
			Assert.AreEqual(0.02f, snapshot40.PositionY);
		}
	}
}
