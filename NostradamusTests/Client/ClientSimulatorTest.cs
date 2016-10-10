using Nostradamus.Client;
using Nostradamus.Examples;
using Nostradamus.Server;
using NUnit.Framework;

namespace Nostradamus.Tests.Client
{
    public class ClientSimulatorTest
    {
        const float FloatAppromiateThreshold = 0.0001f;

        [Test]
        public void TestSimpleScene()
        {
            var clientId = new ClientId(1);
            var sceneDesc = new SceneDesc()
            {
                Mode = SceneMode.Client,
                ClientId = clientId,
                SimulationDeltaTime = 20,
                ReconciliationDeltaTime = 50
            };
            var scene = new SimpleScene(sceneDesc);
            var sceneContext = (ClientSceneContext)scene.Context;

            // Time: 0 - 20
            var serverFrame0 = new DeltaSyncFrame(0, 50);
            sceneContext.ReceiveDeltaSyncFrame(serverFrame0);

            sceneContext.Simulate();
            Assert.AreEqual(0, scene.Time);
            Assert.AreEqual(20, scene.DeltaTime);

            var frame0 = sceneContext.FetchCommandFrame();
            Assert.AreEqual(null, frame0);

            var snapshot20 = (CharacterSnapshot)scene.Character.Snapshot;
            Assert.AreEqual(0f, snapshot20.PositionX);
            Assert.AreEqual(0f, snapshot20.PositionY);

            // Time : 20 - 40
            var command1 = new MoveCharacterCommand() { DeltaX = 1, DeltaY = 1 };
            sceneContext.ReceiveCommand(scene.Character.Id, command1);

            sceneContext.Simulate();
            Assert.AreEqual(20, scene.Time);
            Assert.AreEqual(20, scene.DeltaTime);

            var frame20 = sceneContext.FetchCommandFrame();
            Assert.AreEqual(1, frame20.Commands.Count);

            var snapshot40 = (CharacterSnapshot)scene.Character.Snapshot;
            Assert.That(snapshot40.PositionX, Is.EqualTo(0.02f).Within(FloatAppromiateThreshold));
            Assert.That(snapshot40.PositionY, Is.EqualTo(0.02f).Within(FloatAppromiateThreshold));

            // Time: 40 - 60
            var serverFrame50 = new DeltaSyncFrame(50, 50);
            var eventArgs = new CharacterMovedEvent() { PositionX = 0.05f, PositionY = 0.05f };
            var @event = new Event(scene.Character.Id, eventArgs);
            serverFrame50.Events.Add(@event);
            serverFrame50.LastCommandSeqs.Add(clientId, 1);
            sceneContext.ReceiveDeltaSyncFrame(serverFrame50);

            var command2 = new MoveCharacterCommand() { DeltaX = 1, DeltaY = 1 };
            sceneContext.ReceiveCommand(scene.Character.Id, command2);

            sceneContext.Simulate();
            Assert.AreEqual(40, scene.Time);
            Assert.AreEqual(20, scene.DeltaTime);

            var frame40 = sceneContext.FetchCommandFrame();
            Assert.AreEqual(1, frame40.Commands.Count);

            var snapshot60 = (CharacterSnapshot)scene.Character.Snapshot;
            Assert.That(snapshot60.PositionX, Is.EqualTo(0.07f).Within(FloatAppromiateThreshold));
            Assert.That(snapshot60.PositionY, Is.EqualTo(0.07f).Within(FloatAppromiateThreshold));
        }

        [Test]
        public void TestSimpleSceneWithServer()
        {
            // Server tick freq: 20Hz. Client tick freq: 50Hz. Client latency: 20ms

            var serverSceneDesc = new SceneDesc()
            {
                Mode = SceneMode.Server,
                SimulationDeltaTime = 50,
            };
            var serverScene = new SimpleScene(serverSceneDesc);
            var serverSceneContext = (ServerSceneContext)serverScene.Context;

            var clientId = new ClientId(1);
            var clientSceneDesc = new SceneDesc()
            {
                Mode = SceneMode.Client,
                SimulationDeltaTime = 20,
                ReconciliationDeltaTime = 50
            };
            var clientScene = new SimpleScene(clientSceneDesc);
            var clientSceneContext = (ClientSceneContext)clientScene.Context;

            // +++ Server: 0. Updated #1
            serverSceneContext.Simulate();
            var serverFrame0 = serverSceneContext.FetchDeltaSyncFrame();
            Assert.AreEqual(0, serverScene.Time);
            Assert.AreEqual(50, serverScene.DeltaTime);
            Assert.AreEqual(0, serverFrame0.Time);
            Assert.AreEqual(50, serverFrame0.DeltaTime);
            Assert.AreEqual(0, serverFrame0.Events.Count);

            var serverSnapshot0 = (CharacterSnapshot)serverScene.Character.Snapshot;
            Assert.AreEqual(0, serverSnapshot0.PositionX);
            Assert.AreEqual(0, serverSnapshot0.PositionY);

            // --- Client: 20 / 0. Update #1 received.
            clientSceneContext.ReceiveDeltaSyncFrame(serverFrame0);

            clientSceneContext.Simulate();
            Assert.AreEqual(0, clientScene.Time);
            Assert.AreEqual(20, clientScene.DeltaTime);

            var clientFrame0 = clientSceneContext.FetchCommandFrame();
            Assert.AreEqual(null, clientFrame0);

            var clientSnapshot0 = (CharacterSnapshot)clientScene.Character.Snapshot;
            Assert.AreEqual(0, clientSnapshot0.PositionX);
            Assert.AreEqual(0, clientSnapshot0.PositionY);

            // --- Client: 40 / 20. Updated #2. Move command #1 sent
            var clientCommand1 = new MoveCharacterCommand() { DeltaX = 1, DeltaY = 1 };
            clientSceneContext.ReceiveCommand(clientScene.Character.Id, clientCommand1);

            clientSceneContext.Simulate();
            var clientFrame20 = clientSceneContext.FetchCommandFrame();
            Assert.AreEqual(20, clientScene.Time);
            Assert.AreEqual(20, clientScene.DeltaTime);
            Assert.AreEqual(1, clientFrame20.Commands.Count);

            var clientSnapshot20 = (CharacterSnapshot)clientScene.Character.Snapshot;
            Assert.That(clientSnapshot20.PositionX, Is.EqualTo(0.02f).Within(FloatAppromiateThreshold));
            Assert.That(clientSnapshot20.PositionY, Is.EqualTo(0.02f).Within(FloatAppromiateThreshold));

            // +++ Server: 50 / 30. Updated #2.
            serverSceneContext.Simulate();
            var serverFrame50 = serverSceneContext.FetchDeltaSyncFrame();
            Assert.AreEqual(50, serverScene.Time);
            Assert.AreEqual(50, serverScene.DeltaTime);
            Assert.AreEqual(50, serverFrame50.Time);
            Assert.AreEqual(50, serverFrame50.DeltaTime);
            Assert.AreEqual(0, serverFrame50.Events.Count);

            var serverSnapshot50 = (CharacterSnapshot)serverScene.Character.Snapshot;
            Assert.AreEqual(0, serverSnapshot50.PositionX);
            Assert.AreEqual(0, serverSnapshot50.PositionY);

            // +++ Server: 60 / 40. Move command #1 received
            serverSceneContext.ReceiveCommandFrame(clientFrame20);

            // --- Client: 60 / 40. Updated #3. Move command #2 sent.
            var clientCommand2 = new MoveCharacterCommand() { DeltaX = 1, DeltaY = 1 };
            clientSceneContext.ReceiveCommand(clientScene.Character.Id, clientCommand2);

            clientSceneContext.Simulate();
            var clientFrame40 = clientSceneContext.FetchCommandFrame();
            Assert.AreEqual(40, clientScene.Time);
            Assert.AreEqual(20, clientScene.DeltaTime);
            Assert.AreEqual(1, clientFrame40.Commands.Count);

            var clientSnapshot40 = (CharacterSnapshot)clientScene.Character.Snapshot;
            Assert.That(clientSnapshot40.PositionX, Is.EqualTo(0.04f).Within(FloatAppromiateThreshold));
            Assert.That(clientSnapshot40.PositionY, Is.EqualTo(0.04f).Within(FloatAppromiateThreshold));

            // --- Client: 70 / 50. Update #2 received.
            clientSceneContext.ReceiveDeltaSyncFrame(serverFrame50);

            // +++ Server: 80 / 60. Move command #2 received.
            serverSceneContext.ReceiveCommandFrame(clientFrame40);

            // --- Client: 80 / 60. Updated #4. Move command #3 sent.
            var clientCommand3 = new MoveCharacterCommand() { DeltaX = 1, DeltaY = 1 };
            clientSceneContext.ReceiveCommand(clientScene.Character.Id, clientCommand3);

            clientSceneContext.Simulate();
            var clientFrame60 = clientSceneContext.FetchCommandFrame();
            Assert.AreEqual(60, clientScene.Time);
            Assert.AreEqual(20, clientScene.DeltaTime);
            Assert.AreEqual(1, clientFrame60.Commands.Count);

            var clientSnapshot60 = (CharacterSnapshot)clientScene.Character.Snapshot;
            Assert.That(clientSnapshot60.PositionX, Is.EqualTo(0.06f).Within(FloatAppromiateThreshold));
            Assert.That(clientSnapshot60.PositionY, Is.EqualTo(0.06f).Within(FloatAppromiateThreshold));

            // +++ Server: 100 / 80. Move command #3 received. Updated #3
            serverSceneContext.ReceiveCommandFrame(clientFrame60);

            serverSceneContext.Simulate();
            var serverFrame100 = serverSceneContext.FetchDeltaSyncFrame();
            Assert.AreEqual(100, serverScene.Time);
            Assert.AreEqual(50, serverScene.DeltaTime);
            Assert.AreEqual(100, serverFrame100.Time);
            Assert.AreEqual(50, serverFrame100.DeltaTime);
            Assert.AreEqual(1, serverFrame100.Events.Count);

            var serverSnapshot100 = (CharacterSnapshot)serverScene.Character.Snapshot;
            Assert.That(serverSnapshot100.PositionX, Is.EqualTo(0.05f).Within(FloatAppromiateThreshold));
            Assert.That(serverSnapshot100.PositionY, Is.EqualTo(0.05f).Within(FloatAppromiateThreshold));

            // --- Client: 100 / 80. Updated #5. Move command #4 sent
            var clientCommand4 = new MoveCharacterCommand() { DeltaX = 1, DeltaY = 1 };
            clientSceneContext.ReceiveCommand(clientScene.Character.Id, clientCommand4);

            clientSceneContext.Simulate();
            var clientFrame80 = clientSceneContext.FetchCommandFrame();
            Assert.AreEqual(80, clientScene.Time);
            Assert.AreEqual(20, clientScene.DeltaTime);
            Assert.AreEqual(1, clientFrame80.Commands.Count);

            var clientSnapshot80 = (CharacterSnapshot)clientScene.Character.Snapshot;
            Assert.AreEqual(0.08f, clientSnapshot80.PositionX);
            Assert.AreEqual(0.08f, clientSnapshot80.PositionY);

            // +++ Server: 120 / 100. Move command #4 received.
            serverSceneContext.ReceiveCommandFrame(clientFrame80);

            // --- Client: 120 / 100. Update #3 received. Move command #3 confirmed. Move command #5 sent
            clientSceneContext.ReceiveDeltaSyncFrame(serverFrame100);

            var clientCommand5 = new MoveCharacterCommand() { DeltaX = 1, DeltaY = 1 };
            clientSceneContext.ReceiveCommand(clientScene.Character.Id, clientCommand5);

            clientSceneContext.Simulate();
            var clientFrame100 = clientSceneContext.FetchCommandFrame();
            Assert.AreEqual(100, clientScene.Time);
            Assert.AreEqual(20, clientScene.DeltaTime);
            Assert.AreEqual(1, clientFrame100.Commands.Count);

            var clientSnapshot100 = (CharacterSnapshot)clientScene.Character.Snapshot;
            Assert.That(clientSnapshot100.PositionX, Is.EqualTo(0.09f).Within(FloatAppromiateThreshold));
            Assert.That(clientSnapshot100.PositionY, Is.EqualTo(0.09f).Within(FloatAppromiateThreshold));

            // +++ Server: 140 / 120. Move command #5 received
            serverSceneContext.ReceiveCommandFrame(clientFrame100);

            // --- Client: 140 / 120. 
            clientSceneContext.Simulate();
            Assert.AreEqual(120, clientScene.Time);
            Assert.AreEqual(20, clientScene.DeltaTime);

            var clientFrame120 = clientSceneContext.FetchCommandFrame();
            Assert.AreEqual(null, clientFrame120);

            var clientSnapshot120 = (CharacterSnapshot)clientScene.Character.Snapshot;
            Assert.That(clientSnapshot120.PositionX, Is.EqualTo(0.09f).Within(FloatAppromiateThreshold));
            Assert.That(clientSnapshot120.PositionY, Is.EqualTo(0.09f).Within(FloatAppromiateThreshold));

            // +++ Server: 150 / 130. Updated #3
            serverSceneContext.Simulate();
            var serverFrame150 = serverSceneContext.FetchDeltaSyncFrame();
            Assert.AreEqual(150, serverScene.Time);
            Assert.AreEqual(50, serverScene.DeltaTime);
            Assert.AreEqual(150, serverFrame150.Time);
            Assert.AreEqual(50, serverFrame150.DeltaTime);
            Assert.AreEqual(1, serverFrame150.Events.Count);

            var serverSnapshot150 = (CharacterSnapshot)serverScene.Character.Snapshot;
            Assert.That(serverSnapshot150.PositionX, Is.EqualTo(0.1f).Within(FloatAppromiateThreshold));
            Assert.That(serverSnapshot150.PositionY, Is.EqualTo(0.1f).Within(FloatAppromiateThreshold));

            // --- Client: 160 / 140.
            clientSceneContext.Simulate();
            Assert.AreEqual(140, clientScene.Time);
            Assert.AreEqual(20, clientScene.DeltaTime);
            var clientFrame140 = clientSceneContext.FetchCommandFrame();
            Assert.AreEqual(null, clientFrame140);

            var clientSnapshot140 = (CharacterSnapshot)clientScene.Character.Snapshot;
            Assert.That(clientSnapshot140.PositionX, Is.EqualTo(0.09f).Within(FloatAppromiateThreshold));
            Assert.That(clientSnapshot140.PositionY, Is.EqualTo(0.09f).Within(FloatAppromiateThreshold));

            // --- Client: 170 / 150. Update #3 received
            clientSceneContext.ReceiveDeltaSyncFrame(serverFrame150);

            // --- Client: 180 / 160. 
            clientSceneContext.Simulate();
            Assert.AreEqual(160, clientScene.Time);
            Assert.AreEqual(20, clientScene.DeltaTime);
            var clientFrame160 = clientSceneContext.FetchCommandFrame();
            Assert.AreEqual(null, clientFrame160);

            var clientSnapshot160 = (CharacterSnapshot)clientScene.Character.Snapshot;
            Assert.That(clientSnapshot160.PositionX, Is.EqualTo(0.1f).Within(FloatAppromiateThreshold));
            Assert.That(clientSnapshot160.PositionY, Is.EqualTo(0.1f).Within(FloatAppromiateThreshold));
        }
    }
}
