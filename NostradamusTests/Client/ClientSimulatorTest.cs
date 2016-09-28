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
			var simulator = new ClientSimulator(scene, clientId, 50);

			// Time: 0 - 20
			var serverFrame0 = new ServerSyncFrame(0, 50);
			simulator.AddServerSyncFrame(serverFrame0);

			var frame0 = simulator.Simulate(20);
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

			var frame20 = simulator.Simulate(20);
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

			var frame40 = simulator.Simulate(20);
			Assert.AreEqual(40, scene.Time);
			Assert.AreEqual(20, scene.DeltaTime);
			Assert.AreEqual(40, frame40.Time);
			Assert.AreEqual(1, frame40.Commands.Count);

			var snapshot60 = (ActorSnapshot)scene.Character.Snapshot;
			Assert.AreEqual(0.07f, snapshot60.PositionX);
			Assert.AreEqual(0.07f, snapshot60.PositionY);
		}

		[Test]
		public void TestSimpleSceneWithServer()
		{
			// Server tick freq: 20Hz. Client tick freq: 50Hz. Client latency: 20ms

			var serverScene = new SimpleScene();
			var serverSimulator = new ServerSimulator(serverScene);

			var clientId = new ClientId(1);
			var clientScene = new SimpleScene();
			var clientSimulator = new ClientSimulator(clientScene, clientId, 50);

			// +++ Server: 0. Updated #1
			var serverFrame0 = serverSimulator.Simulate(50);
			Assert.AreEqual(0, serverScene.Time);
			Assert.AreEqual(50, serverScene.DeltaTime);
			Assert.AreEqual(0, serverFrame0.Time);
			Assert.AreEqual(50, serverFrame0.DeltaTime);
			Assert.AreEqual(0, serverFrame0.Events.Count);

			var serverSnapshot0 = (ActorSnapshot)serverScene.Character.Snapshot;
			Assert.AreEqual(0, serverSnapshot0.PositionX);
			Assert.AreEqual(0, serverSnapshot0.PositionY);

			// --- Client: 20 / 0. Update #1 received.
			clientSimulator.AddServerSyncFrame(serverFrame0);

			var clientFrame0 = clientSimulator.Simulate(20);
			Assert.AreEqual(0, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);
			Assert.AreEqual(0, clientFrame0.Time);
			Assert.AreEqual(0, clientFrame0.Commands.Count);

			var clientSnapshot0 = (ActorSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0, clientSnapshot0.PositionX);
			Assert.AreEqual(0, clientSnapshot0.PositionY);

			// --- Client: 40 / 20. Updated #2. Move command #1 sent
			var clientCommand1 = new MoveActorCommand() { DeltaX = 1, DeltaY = 1 };
			clientSimulator.AddCommand(clientScene.Character.Id, clientCommand1);

			var clientFrame20 = clientSimulator.Simulate(20);
			Assert.AreEqual(20, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);
			Assert.AreEqual(20, clientFrame20.Time);
			Assert.AreEqual(1, clientFrame20.Commands.Count);

			var clientSnapshot20 = (ActorSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0.02f, clientSnapshot20.PositionX);
			Assert.AreEqual(0.02f, clientSnapshot20.PositionY);

			// +++ Server: 50 / 30. Updated #2.
			var serverFrame50 = serverSimulator.Simulate(50);
			Assert.AreEqual(50, serverScene.Time);
			Assert.AreEqual(50, serverScene.DeltaTime);
			Assert.AreEqual(50, serverFrame50.Time);
			Assert.AreEqual(50, serverFrame50.DeltaTime);
			Assert.AreEqual(0, serverFrame50.Events.Count);

			var serverSnapshot50 = (ActorSnapshot)serverScene.Character.Snapshot;
			Assert.AreEqual(0, serverSnapshot50.PositionX);
			Assert.AreEqual(0, serverSnapshot50.PositionY);

			// +++ Server: 60 / 40. Move command #1 received
			serverSimulator.AddClientSyncFrame(clientFrame20);

			// --- Client: 60 / 40. Updated #3. Move command #2 sent.
			var clientCommand2 = new MoveActorCommand() { DeltaX = 1, DeltaY = 1 };
			clientSimulator.AddCommand(clientScene.Character.Id, clientCommand2);

			var clientFrame40 = clientSimulator.Simulate(20);
			Assert.AreEqual(40, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);
			Assert.AreEqual(40, clientFrame40.Time);
			Assert.AreEqual(1, clientFrame40.Commands.Count);

			var clientSnapshot40 = (ActorSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0.04f, clientSnapshot40.PositionX);
			Assert.AreEqual(0.04f, clientSnapshot40.PositionY);

			// --- Client: 70 / 50. Update #2 received.
			clientSimulator.AddServerSyncFrame(serverFrame50);

			// +++ Server: 80 / 60. Move command #2 received.
			serverSimulator.AddClientSyncFrame(clientFrame40);

			// --- Client: 80 / 60. Updated #4. Move command #3 sent.
			var clientCommand3 = new MoveActorCommand() { DeltaX = 1, DeltaY = 1 };
			clientSimulator.AddCommand(clientScene.Character.Id, clientCommand3);

			var clientFrame60 = clientSimulator.Simulate(20);
			Assert.AreEqual(60, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);
			Assert.AreEqual(60, clientFrame60.Time);
			Assert.AreEqual(1, clientFrame60.Commands.Count);

			var clientSnapshot60 = (ActorSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0.06f, clientSnapshot60.PositionX);
			Assert.AreEqual(0.06f, clientSnapshot60.PositionY);

			// +++ Server: 100 / 80. Move command #3 received. Updated #3
			serverSimulator.AddClientSyncFrame(clientFrame60);

			var serverFrame100 = serverSimulator.Simulate(50);
			Assert.AreEqual(100, serverScene.Time);
			Assert.AreEqual(50, serverScene.DeltaTime);
			Assert.AreEqual(100, serverFrame100.Time);
			Assert.AreEqual(50, serverFrame100.DeltaTime);
			Assert.AreEqual(1, serverFrame100.Events.Count);

			var serverSnapshot100 = (ActorSnapshot)serverScene.Character.Snapshot;
			Assert.AreEqual(0.05f, serverSnapshot100.PositionX);
			Assert.AreEqual(0.05f, serverSnapshot100.PositionY);

			// --- Client: 100 / 80. Updated #5. Move command #4 sent
			var clientCommand4 = new MoveActorCommand() { DeltaX = 1, DeltaY = 1 };
			clientSimulator.AddCommand(clientScene.Character.Id, clientCommand4);

			var clientFrame80 = clientSimulator.Simulate(20);
			Assert.AreEqual(80, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);
			Assert.AreEqual(80, clientFrame80.Time);
			Assert.AreEqual(1, clientFrame80.Commands.Count);

			var clientSnapshot80 = (ActorSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0.08f, clientSnapshot80.PositionX);
			Assert.AreEqual(0.08f, clientSnapshot80.PositionY);

			// +++ Server: 120 / 100. Move command #4 received.
			serverSimulator.AddClientSyncFrame(clientFrame80);

			// --- Client: 120 / 100. Update #3 received. Move command #3 confirmed. Move command #5 sent
			clientSimulator.AddServerSyncFrame(serverFrame100);

			var clientCommand5 = new MoveActorCommand() { DeltaX = 1, DeltaY = 1 };
			clientSimulator.AddCommand(clientScene.Character.Id, clientCommand5);

			var clientFrame100 = clientSimulator.Simulate(20);
			Assert.AreEqual(100, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);
			Assert.AreEqual(100, clientFrame100.Time);
			Assert.AreEqual(1, clientFrame100.Commands.Count);

			var clientSnapshot100 = (ActorSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0.09f, clientSnapshot100.PositionX);
			Assert.AreEqual(0.09f, clientSnapshot100.PositionY);

			// +++ Server: 140 / 120. Move command #5 received
			serverSimulator.AddClientSyncFrame(clientFrame100);

			// --- Client: 140 / 120. 
			var clientFrame120 = clientSimulator.Simulate(20);
			Assert.AreEqual(120, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);
			Assert.AreEqual(120, clientFrame120.Time);
			Assert.AreEqual(0, clientFrame120.Commands.Count);

			var clientSnapshot120 = (ActorSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0.09f, clientSnapshot120.PositionX);
			Assert.AreEqual(0.09f, clientSnapshot120.PositionY);

			// +++ Server: 150 / 130. Updated #3
			var serverFrame150 = serverSimulator.Simulate(50);
			Assert.AreEqual(150, serverScene.Time);
			Assert.AreEqual(50, serverScene.DeltaTime);
			Assert.AreEqual(150, serverFrame150.Time);
			Assert.AreEqual(50, serverFrame150.DeltaTime);
			Assert.AreEqual(1, serverFrame150.Events.Count);

			var serverSnapshot150 = (ActorSnapshot)serverScene.Character.Snapshot;
			Assert.AreEqual(0.1f, serverSnapshot150.PositionX);
			Assert.AreEqual(0.1f, serverSnapshot150.PositionY);

			// --- Client: 160 / 140.
			var clientFrame140 = clientSimulator.Simulate(20);
			Assert.AreEqual(140, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);
			Assert.AreEqual(140, clientFrame140.Time);
			Assert.AreEqual(0, clientFrame140.Commands.Count);

			var clientSnapshot140 = (ActorSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0.09f, clientSnapshot140.PositionX);
			Assert.AreEqual(0.09f, clientSnapshot140.PositionY);

			// --- Client: 170 / 150. Update #3 received
			clientSimulator.AddServerSyncFrame(serverFrame150);

			// --- Client: 180 / 160. 
			var clientFrame160 = clientSimulator.Simulate(20);
			Assert.AreEqual(160, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);
			Assert.AreEqual(160, clientFrame160.Time);
			Assert.AreEqual(0, clientFrame160.Commands.Count);

			var clientSnapshot160 = (ActorSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0.1f, clientSnapshot160.PositionX);
			Assert.AreEqual(0.1f, clientSnapshot160.PositionY);
		}
	}
}
