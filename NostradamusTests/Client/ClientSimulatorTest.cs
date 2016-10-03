using Nostradamus.Client;
using Nostradamus.Examples;
using Nostradamus.Server;
using NUnit.Framework;

namespace Nostradamus.Tests.Client
{
	public class ClientSimulatorTest
	{
		[Test]
		public void TestSimpleScene()
		{
			var clientId = new ClientId(1);
			var simulator = new ClientSimulator(clientId, 50);
			var scene = new SimpleScene(simulator);

			// Time: 0 - 20
			var serverFrame0 = new DeltaSyncFrame(0, 50);
			simulator.ReceiveDeltaSyncFrame(serverFrame0);

			simulator.Simulate(20);
			Assert.AreEqual(0, scene.Time);
			Assert.AreEqual(20, scene.DeltaTime);

			var frame0 = simulator.FetchCommandFrame();
			Assert.AreEqual(null, frame0);

			var snapshot20 = (CharacterSnapshot)scene.Character.Snapshot;
			Assert.AreEqual(0f, snapshot20.PositionX);
			Assert.AreEqual(0f, snapshot20.PositionY);

			// Time : 20 - 40
			var command1 = new MoveCharacterCommand() { DeltaX = 1, DeltaY = 1 };
			simulator.ReceiveCommand(scene.Character.Id, command1);

			simulator.Simulate(20);
			Assert.AreEqual(20, scene.Time);
			Assert.AreEqual(20, scene.DeltaTime);

			var frame20 = simulator.FetchCommandFrame();
			Assert.AreEqual(1, frame20.Commands.Count);

			var snapshot40 = (CharacterSnapshot)scene.Character.Snapshot;
			Assert.AreEqual(0.02f, snapshot40.PositionX);
			Assert.AreEqual(0.02f, snapshot40.PositionY);

			// Time: 40 - 60
			var serverFrame50 = new DeltaSyncFrame(50, 50);
			var eventArgs = new CharacterMovedEvent() { PositionX = 0.05f, PositionY = 0.05f };
			var @event = new Event(scene.Character.Id, eventArgs);
			serverFrame50.Events.Add(@event);
			serverFrame50.LastCommandSeqs.Add(clientId, 1);
			simulator.ReceiveDeltaSyncFrame(serverFrame50);

			var command2 = new MoveCharacterCommand() { DeltaX = 1, DeltaY = 1 };
			simulator.ReceiveCommand(scene.Character.Id, command2);

			simulator.Simulate(20);
			Assert.AreEqual(40, scene.Time);
			Assert.AreEqual(20, scene.DeltaTime);

			var frame40 = simulator.FetchCommandFrame();
			Assert.AreEqual(1, frame40.Commands.Count);

			var snapshot60 = (CharacterSnapshot)scene.Character.Snapshot;
			Assert.AreEqual(0.07f, snapshot60.PositionX);
			Assert.AreEqual(0.07f, snapshot60.PositionY);
		}

		[Test]
		public void TestSimpleSceneWithServer()
		{
			// Server tick freq: 20Hz. Client tick freq: 50Hz. Client latency: 20ms

			var serverSimulator = new ServerSimulator();
			var serverScene = new SimpleScene(serverSimulator);

			var clientId = new ClientId(1);
			var clientSimulator = new ClientSimulator(clientId, 50);
			var clientScene = new SimpleScene(clientSimulator);

			// +++ Server: 0. Updated #1
			serverSimulator.Simulate(50);
			var serverFrame0 = serverSimulator.FetchDeltaSyncFrame();
			Assert.AreEqual(0, serverScene.Time);
			Assert.AreEqual(50, serverScene.DeltaTime);
			Assert.AreEqual(0, serverFrame0.Time);
			Assert.AreEqual(50, serverFrame0.DeltaTime);
			Assert.AreEqual(0, serverFrame0.Events.Count);

			var serverSnapshot0 = (CharacterSnapshot)serverScene.Character.Snapshot;
			Assert.AreEqual(0, serverSnapshot0.PositionX);
			Assert.AreEqual(0, serverSnapshot0.PositionY);

			// --- Client: 20 / 0. Update #1 received.
			clientSimulator.ReceiveDeltaSyncFrame(serverFrame0);

			clientSimulator.Simulate(20);
			Assert.AreEqual(0, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);

			var clientFrame0 = clientSimulator.FetchCommandFrame();
			Assert.AreEqual(null, clientFrame0);

			var clientSnapshot0 = (CharacterSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0, clientSnapshot0.PositionX);
			Assert.AreEqual(0, clientSnapshot0.PositionY);

			// --- Client: 40 / 20. Updated #2. Move command #1 sent
			var clientCommand1 = new MoveCharacterCommand() { DeltaX = 1, DeltaY = 1 };
			clientSimulator.ReceiveCommand(clientScene.Character.Id, clientCommand1);

			clientSimulator.Simulate(20);
			var clientFrame20 = clientSimulator.FetchCommandFrame();
			Assert.AreEqual(20, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);
			Assert.AreEqual(1, clientFrame20.Commands.Count);

			var clientSnapshot20 = (CharacterSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0.02f, clientSnapshot20.PositionX);
			Assert.AreEqual(0.02f, clientSnapshot20.PositionY);

			// +++ Server: 50 / 30. Updated #2.
			serverSimulator.Simulate(50);
			var serverFrame50 = serverSimulator.FetchDeltaSyncFrame();
			Assert.AreEqual(50, serverScene.Time);
			Assert.AreEqual(50, serverScene.DeltaTime);
			Assert.AreEqual(50, serverFrame50.Time);
			Assert.AreEqual(50, serverFrame50.DeltaTime);
			Assert.AreEqual(0, serverFrame50.Events.Count);

			var serverSnapshot50 = (CharacterSnapshot)serverScene.Character.Snapshot;
			Assert.AreEqual(0, serverSnapshot50.PositionX);
			Assert.AreEqual(0, serverSnapshot50.PositionY);

			// +++ Server: 60 / 40. Move command #1 received
			serverSimulator.ReceiveCommandFrame(clientFrame20);

			// --- Client: 60 / 40. Updated #3. Move command #2 sent.
			var clientCommand2 = new MoveCharacterCommand() { DeltaX = 1, DeltaY = 1 };
			clientSimulator.ReceiveCommand(clientScene.Character.Id, clientCommand2);

			clientSimulator.Simulate(20);
			var clientFrame40 = clientSimulator.FetchCommandFrame();
			Assert.AreEqual(40, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);
			Assert.AreEqual(1, clientFrame40.Commands.Count);

			var clientSnapshot40 = (CharacterSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0.04f, clientSnapshot40.PositionX);
			Assert.AreEqual(0.04f, clientSnapshot40.PositionY);

			// --- Client: 70 / 50. Update #2 received.
			clientSimulator.ReceiveDeltaSyncFrame(serverFrame50);

			// +++ Server: 80 / 60. Move command #2 received.
			serverSimulator.ReceiveCommandFrame(clientFrame40);

			// --- Client: 80 / 60. Updated #4. Move command #3 sent.
			var clientCommand3 = new MoveCharacterCommand() { DeltaX = 1, DeltaY = 1 };
			clientSimulator.ReceiveCommand(clientScene.Character.Id, clientCommand3);

			clientSimulator.Simulate(20);
			var clientFrame60 = clientSimulator.FetchCommandFrame();
			Assert.AreEqual(60, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);
			Assert.AreEqual(1, clientFrame60.Commands.Count);

			var clientSnapshot60 = (CharacterSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0.06f, clientSnapshot60.PositionX);
			Assert.AreEqual(0.06f, clientSnapshot60.PositionY);

			// +++ Server: 100 / 80. Move command #3 received. Updated #3
			serverSimulator.ReceiveCommandFrame(clientFrame60);

			serverSimulator.Simulate(50);
			var serverFrame100 = serverSimulator.FetchDeltaSyncFrame();
			Assert.AreEqual(100, serverScene.Time);
			Assert.AreEqual(50, serverScene.DeltaTime);
			Assert.AreEqual(100, serverFrame100.Time);
			Assert.AreEqual(50, serverFrame100.DeltaTime);
			Assert.AreEqual(1, serverFrame100.Events.Count);

			var serverSnapshot100 = (CharacterSnapshot)serverScene.Character.Snapshot;
			Assert.AreEqual(0.05f, serverSnapshot100.PositionX);
			Assert.AreEqual(0.05f, serverSnapshot100.PositionY);

			// --- Client: 100 / 80. Updated #5. Move command #4 sent
			var clientCommand4 = new MoveCharacterCommand() { DeltaX = 1, DeltaY = 1 };
			clientSimulator.ReceiveCommand(clientScene.Character.Id, clientCommand4);

			clientSimulator.Simulate(20);
			var clientFrame80 = clientSimulator.FetchCommandFrame();
			Assert.AreEqual(80, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);
			Assert.AreEqual(1, clientFrame80.Commands.Count);

			var clientSnapshot80 = (CharacterSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0.08f, clientSnapshot80.PositionX);
			Assert.AreEqual(0.08f, clientSnapshot80.PositionY);

			// +++ Server: 120 / 100. Move command #4 received.
			serverSimulator.ReceiveCommandFrame(clientFrame80);

			// --- Client: 120 / 100. Update #3 received. Move command #3 confirmed. Move command #5 sent
			clientSimulator.ReceiveDeltaSyncFrame(serverFrame100);

			var clientCommand5 = new MoveCharacterCommand() { DeltaX = 1, DeltaY = 1 };
			clientSimulator.ReceiveCommand(clientScene.Character.Id, clientCommand5);

			clientSimulator.Simulate(20);
			var clientFrame100 = clientSimulator.FetchCommandFrame();
			Assert.AreEqual(100, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);
			Assert.AreEqual(1, clientFrame100.Commands.Count);

			var clientSnapshot100 = (CharacterSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0.09f, clientSnapshot100.PositionX);
			Assert.AreEqual(0.09f, clientSnapshot100.PositionY);

			// +++ Server: 140 / 120. Move command #5 received
			serverSimulator.ReceiveCommandFrame(clientFrame100);

			// --- Client: 140 / 120. 
			clientSimulator.Simulate(20);
			Assert.AreEqual(120, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);

			var clientFrame120 = clientSimulator.FetchCommandFrame();
			Assert.AreEqual(null, clientFrame120);

			var clientSnapshot120 = (CharacterSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0.09f, clientSnapshot120.PositionX);
			Assert.AreEqual(0.09f, clientSnapshot120.PositionY);

			// +++ Server: 150 / 130. Updated #3
			serverSimulator.Simulate(50);
			var serverFrame150 = serverSimulator.FetchDeltaSyncFrame();
			Assert.AreEqual(150, serverScene.Time);
			Assert.AreEqual(50, serverScene.DeltaTime);
			Assert.AreEqual(150, serverFrame150.Time);
			Assert.AreEqual(50, serverFrame150.DeltaTime);
			Assert.AreEqual(1, serverFrame150.Events.Count);

			var serverSnapshot150 = (CharacterSnapshot)serverScene.Character.Snapshot;
			Assert.AreEqual(0.1f, serverSnapshot150.PositionX);
			Assert.AreEqual(0.1f, serverSnapshot150.PositionY);

			// --- Client: 160 / 140.
			clientSimulator.Simulate(20);
			Assert.AreEqual(140, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);
			var clientFrame140 = clientSimulator.FetchCommandFrame();
			Assert.AreEqual(null, clientFrame140);

			var clientSnapshot140 = (CharacterSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0.09f, clientSnapshot140.PositionX);
			Assert.AreEqual(0.09f, clientSnapshot140.PositionY);

			// --- Client: 170 / 150. Update #3 received
			clientSimulator.ReceiveDeltaSyncFrame(serverFrame150);

			// --- Client: 180 / 160. 
			clientSimulator.Simulate(20);
			Assert.AreEqual(160, clientScene.Time);
			Assert.AreEqual(20, clientScene.DeltaTime);
			var clientFrame160 = clientSimulator.FetchCommandFrame();
			Assert.AreEqual(null, clientFrame160);

			var clientSnapshot160 = (CharacterSnapshot)clientScene.Character.Snapshot;
			Assert.AreEqual(0.1f, clientSnapshot160.PositionX);
			Assert.AreEqual(0.1f, clientSnapshot160.PositionY);
		}
	}
}
