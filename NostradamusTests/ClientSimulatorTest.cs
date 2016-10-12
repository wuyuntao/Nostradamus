using Nostradamus.Client;
using Nostradamus.Examples;
using Nostradamus.Physics;
using Nostradamus.Server;
using NUnit.Framework;

namespace Nostradamus.Tests
{
    public class ClientSimulatorTest
    {
        [Test]
        public void TestExampleScene()
        {
            // Server tick freq: 20Hz. Client tick freq: 50Hz. Client latency: 40ms

            var serverSimulator = new ServerSimulator();
            RegisterActorFactories(serverSimulator);
            var serverSceneDesc = new ExampleSceneDesc(50, 50);
            var serverScene = serverSimulator.CreateScene<ExampleScene>(serverSceneDesc);

            var clientId = new ClientId(1);
            var clientSimulator = new ClientSimulator(clientId);
            RegisterActorFactories(clientSimulator);
            var clientSceneDesc = new ExampleSceneDesc(20, 50);
            var clientScene = clientSimulator.CreateScene<ExampleScene>(clientSceneDesc);

            // +++ Server: 0. Update #1 sent
            Server_Update0(serverSimulator, serverScene);
            var fullSyncFrame0 = serverSimulator.FullSyncFrame;

            // --- Client: 40 / 0. Update #1 received
            clientSimulator.ReceiveFullSyncFrame(fullSyncFrame0);
            Client_Update0(clientSimulator, clientScene);

            // +++ Server: 50 / 10. Update #2 sent
            Server_Update50(serverSimulator, serverScene);
            var deltaSyncFrame50 = serverSimulator.DeltaSyncFrame;

            // --- Client: 60 / 20. Update #2
            Client_Update20(clientSimulator, clientScene);

            // --- Client: 80 / 40. Updated #3. Move command #1 sent
            Client_Update40_Command1(clientSimulator, clientScene);
            var commandFrame40 = clientSimulator.CommandFrame;

            // --- Client: 90 / 50. Update #2 received
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame50);

            // +++ Server: 100 / 60. Update #3 sent
            Server_Update100(serverSimulator, serverScene);
            var deltaSyncFrame100 = serverSimulator.DeltaSyncFrame;

            // --- Client: 100 / 60. Updated #3. Move command #2 sent.
            Client_Update60_Command2(clientSimulator, clientScene);
            var commandFrame60 = clientSimulator.CommandFrame;

            // +++ Server: 120 / 80. Move command #1 received
            serverSimulator.ReceiveCommandFrame(commandFrame40);

            // --- Client: 120 / 80. Updated #4. Move command #3 sent.
            Client_Update80_Command3(clientSimulator, clientScene);
            var commandFrame80 = clientSimulator.CommandFrame;

            // +++ Server: 140 / 100. Move command #2 received
            serverSimulator.ReceiveCommandFrame(commandFrame60);

            // --- Client: 140 / 100. Update #3 received. Updated #5. Move command #4 sent.
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame100);

            Client_Update100_Command4(clientSimulator, clientScene);
            var commandFrame100 = clientSimulator.CommandFrame;

            // +++ Server: 150 / 110. Update #4 sent
            Server_Update150(serverSimulator, serverScene, clientId);
            var deltaSyncFrame150 = serverSimulator.DeltaSyncFrame;

            // +++ Server: 160 / 120. Move command #3 received
            serverSimulator.ReceiveCommandFrame(commandFrame80);

            // --- Client: 160 / 120. Updated #4. Move command #5 sent.
            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(0.5f, 0, 1f));
            clientSimulator.Simulate();
            var commandFrame120 = clientSimulator.CommandFrame;

            // +++ Server: 180 / 140. Move command #3 received
            serverSimulator.ReceiveCommandFrame(commandFrame100);

            // --- Client: 180 / 140. Updated #4. Move command #6 sent.
            Client_Update140_Command6(clientSimulator, clientScene);
            var commandFrame140 = clientSimulator.CommandFrame;

            // --- Client: 190 / 150. Update #4 received
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame150);

            // +++ Server: 200 / 160. Move command #4 received. Update
            serverSimulator.ReceiveCommandFrame(commandFrame120);
            Server_Update200(serverSimulator, serverScene, clientId);
            var deltaSyncFrame200 = serverSimulator.DeltaSyncFrame;

            // --- Client: 200 / 160. Update #8
            Client_Update160_Command7(clientSimulator, clientScene);
            var commandFrame160 = clientSimulator.CommandFrame;

            // +++ Server: 220 / 180. Move command #5 received
            serverSimulator.ReceiveCommandFrame(commandFrame140);

            // +++ Server: 240 / 200. Move command #6 received
            serverSimulator.ReceiveCommandFrame(commandFrame160);

            // --- Client: 240 / 200. Update #4 received
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame200);
        }

        private static void RegisterActorFactories(Simulator simulator)
        {
            simulator.RegisterActorFactory<ExampleSceneDesc, ExampleScene>(desc => new ExampleScene());
            simulator.RegisterActorFactory<BallDesc, Ball>(desc => new Ball());
            simulator.RegisterActorFactory<CubeDesc, Cube>(desc => new Cube());
        }

        private static void Server_Update0(ServerSimulator simulator, ExampleScene scene)
        {
            simulator.Simulate();
            Assert.AreEqual(50, simulator.Time);

            var frame = simulator.DeltaSyncFrame;
            Assert.AreEqual(0, frame.Time);
            Assert.AreEqual(50, frame.DeltaTime);
            Assert.AreEqual(3, frame.Events.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.6f, ball.Position.X);
            AssertHelper.AreApproximate(2.597275f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.6f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.09727502f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);
        }

        private static void Server_Update50(ServerSimulator simulator, ExampleScene scene)
        {
            simulator.Simulate();
            Assert.AreEqual(100, simulator.Time);

            var frame = simulator.DeltaSyncFrame;
            Assert.AreEqual(50, frame.Time);
            Assert.AreEqual(50, frame.DeltaTime);
            Assert.AreEqual(2, frame.Events.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.6f, ball.Position.X);
            AssertHelper.AreApproximate(2.59182501f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.6f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.09182501f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);
        }

        private static void Server_Update100(ServerSimulator simulator, ExampleScene scene)
        {
            simulator.Simulate();
            Assert.AreEqual(150, simulator.Time);

            var frame = simulator.DeltaSyncFrame;
            Assert.AreEqual(100, frame.Time);
            Assert.AreEqual(50, frame.DeltaTime);
            Assert.AreEqual(2, frame.Events.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.6f, ball.Position.X);
            AssertHelper.AreApproximate(2.58365011f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.6f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.08364999f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);
        }

        private static void Server_Update150(ServerSimulator simulator, ExampleScene scene, ClientId clientId)
        {
            simulator.Simulate();
            Assert.AreEqual(200, simulator.Time);

            var frame = simulator.DeltaSyncFrame;
            Assert.AreEqual(150, frame.Time);
            Assert.AreEqual(50, frame.DeltaTime);
            Assert.AreEqual(2, frame.Events.Count);
            Assert.AreEqual(1, frame.LastCommandSeqs.Count);
            Assert.AreEqual(2, frame.LastCommandSeqs[clientId]);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.57222223f, ball.Position.X);
            AssertHelper.AreApproximate(2.57275009f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.57222223f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.07274997f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);
        }

        private static void Server_Update200(ServerSimulator simulator, ExampleScene scene, ClientId clientId)
        {
            simulator.Simulate();
            Assert.AreEqual(250, simulator.Time);

            var frame = simulator.DeltaSyncFrame;
            Assert.AreEqual(200, frame.Time);
            Assert.AreEqual(50, frame.DeltaTime);
            Assert.AreEqual(2, frame.Events.Count);
            Assert.AreEqual(1, frame.LastCommandSeqs.Count);
            Assert.AreEqual(5, frame.LastCommandSeqs[clientId]);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.53055549f, ball.Position.X);
            AssertHelper.AreApproximate(2.55912519f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.51666665f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.05912495f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);
        }

        private static void Client_Update0(ClientSimulator simulator, ExampleScene scene)
        {
            simulator.Simulate();
            Assert.AreEqual(20, simulator.Time);

            Assert.IsNull(simulator.CommandFrame);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.6f, ball.Position.X);
            AssertHelper.AreApproximate(2.60218f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.6f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.10218f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);
        }

        private static void Client_Update20(ClientSimulator simulator, ExampleScene scene)
        {
            simulator.Simulate();
            Assert.AreEqual(40, simulator.Time);

            Assert.IsNull(simulator.CommandFrame);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.6f, ball.Position.X);
            AssertHelper.AreApproximate(2.59891009f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.6f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.09890997f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);
        }

        private static void Client_Update40_Command1(ClientSimulator simulator, ExampleScene scene)
        {
            var command = new KickBallCommand(1, 0, 1);
            simulator.ReceiveCommand(scene.Ball, command);

            simulator.Simulate();
            Assert.AreEqual(60, simulator.Time);

            var frame = simulator.CommandFrame;
            Assert.AreEqual(1, frame.Commands.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.57222223f, ball.Position.X);
            AssertHelper.AreApproximate(2.59346008f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.57222223f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.09345996f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);
        }

        private static void Client_Update60_Command2(ClientSimulator simulator, ExampleScene scene)
        {
            var command = new KickBallCommand(1, 0, 0.5f);
            simulator.ReceiveCommand(scene.Ball, command);

            simulator.Simulate();
            Assert.AreEqual(80, simulator.Time);

            var frame = simulator.CommandFrame;
            Assert.AreEqual(1, frame.Commands.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.51666665f, ball.Position.X);
            AssertHelper.AreApproximate(2.58528519f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.53055549f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.08528495f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);
        }

        private static void Client_Update80_Command3(ClientSimulator simulator, ExampleScene scene)
        {
            var command = new KickBallCommand(0.5f, 0, 1f);
            simulator.ReceiveCommand(scene.Ball, command);

            simulator.Simulate();
            Assert.AreEqual(100, simulator.Time);

            var frame = simulator.CommandFrame;
            Assert.AreEqual(1, frame.Commands.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.44722223f, ball.Position.X);
            AssertHelper.AreApproximate(2.57438517f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.46111107f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.07438493f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);
        }

        private static void Client_Update100_Command4(ClientSimulator simulator, ExampleScene scene)
        {
            var command = new KickBallCommand(1f, 0, 1f);
            simulator.ReceiveCommand(scene.Ball, command);

            simulator.Simulate();
            Assert.AreEqual(120, simulator.Time);

            var frame = simulator.CommandFrame;
            Assert.AreEqual(1, frame.Commands.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.3499999f, ball.Position.X);
            AssertHelper.AreApproximate(2.56076026f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.36388874f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.0607599f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);
        }

        private static void Client_Update140_Command6(ClientSimulator simulator, ExampleScene scene)
        {
            var command = new KickBallCommand(1f, 0, 0.5f);
            simulator.ReceiveCommand(scene.Ball, command);

            simulator.Simulate();
            Assert.AreEqual(160, simulator.Time);

            var frame = simulator.CommandFrame;
            Assert.AreEqual(1, frame.Commands.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.0999999f, ball.Position.X);
            AssertHelper.AreApproximate(2.52533531f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.0999999f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.02533484f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);
        }

        private static void Client_Update160_Command7(ClientSimulator simulator, ExampleScene scene)
        {
            var command = new KickBallCommand(1f, 0, 1f);
            simulator.ReceiveCommand(scene.Ball, command);

            simulator.Simulate();
            Assert.AreEqual(180, simulator.Time);

            var frame = simulator.CommandFrame;
            Assert.AreEqual(1, frame.Commands.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.13158202f, ball.Position.X);
            AssertHelper.AreApproximate(2.49999809f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.04862928f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(0.977374852f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);
        }
    }
}
