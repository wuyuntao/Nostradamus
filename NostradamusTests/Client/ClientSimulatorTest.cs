using Nostradamus.Client;
using Nostradamus.Server;
using Nostradamus.Tests.Commnads;
using Nostradamus.Tests.Events;
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

			// Time : 20 - 40
			var command1 = new MoveActorCommand() { DeltaX = 1, DeltaY = 1 };
			simulator.AddCommand(scene.Character.Id, command1);

			var frame20 = simulator.Update(20);
			Assert.AreEqual(20, scene.Time);
			Assert.AreEqual(20, scene.DeltaTime);
			Assert.AreEqual(20, frame20.Time);
			Assert.AreEqual(1, frame20.Commands.Count);

			var snapshot40 = (ActorSnapshot)scene.Character.Snapshot;
			Assert.AreEqual(0.02f, snapshot40.PositionX);
			Assert.AreEqual(0.02f, snapshot40.PositionY);

			// Time: 40 - 60
			var serverFrame50 = new ServerSyncFrame(50, 50);
			var eventArgs = new ActorMovedEvent() { PositionX = 0.05f, PositionY = 0.05f };
			var @event = new Event(scene.Character.Id, clientId, 100, 1, eventArgs);
			serverFrame50.Events.Add(@event);
			serverFrame50.LastCommandSeqs.Add(clientId, 1);
			simulator.AddServerSyncFrame(serverFrame50);

			var command2 = new MoveActorCommand() { DeltaX = 1, DeltaY = 1 };
			simulator.AddCommand(scene.Character.Id, command2);

			var frame40 = simulator.Update(20);
			Assert.AreEqual(40, scene.Time);
			Assert.AreEqual(20, scene.DeltaTime);
			Assert.AreEqual(40, frame40.Time);
			Assert.AreEqual(1, frame40.Commands.Count);

			var snapshot60 = (ActorSnapshot)scene.Character.Snapshot;
			Assert.AreEqual(0.07f, snapshot60.PositionX);
			Assert.AreEqual(0.07f, snapshot60.PositionY);
		}
	}
}
