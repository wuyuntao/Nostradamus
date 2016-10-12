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
            var fullSyncFrame0 = Server_Update0(serverSimulator, serverScene);

            // --- Client: 40 / 0. Update #1 received
            clientSimulator.ReceiveFullSyncFrame(fullSyncFrame0);
            Client_Update0(clientSimulator, clientScene);

            // +++ Server: 50 / 10. Update #2 sent
            var deltaSyncFrame50 = Server_Update50(serverSimulator, serverScene);

            // --- Client: 60 / 20. Update #2
            Client_Update20(clientSimulator, clientScene);

            // --- Client: 80 / 40. Updated #3. Move command #1 sent
            var commandFrame40 = Client_Update40_Command1(clientSimulator, clientScene);

            // --- Client: 90 / 50. Update #2 received
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame50);

            // +++ Server: 100 / 60. Update #3 sent
            // --- Client: 100 / 60. Updated #3. Move command #2 sent.
            var deltaSyncFrame100 = Server_Update100(serverSimulator, serverScene);

            var commandFrame60 = Client_Update60_Command2(clientSimulator, clientScene);

            // +++ Server: 120 / 80. Move command #1 received
            // --- Client: 120 / 80. Updated #4. Move command #3 sent.
            serverSimulator.ReceiveCommandFrame(commandFrame40);

            var commandFrame80 = Client_Update80_Command3(clientSimulator, clientScene);

            // +++ Server: 140 / 100. Move command #2 received
            // --- Client: 140 / 100. Update #3 received. Updated #5. Move command #4 sent.
            serverSimulator.ReceiveCommandFrame(commandFrame60);

            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame100);
            var commandFrame100 = Client_Update100_Command4(clientSimulator, clientScene);

            // +++ Server: 150 / 110. Update #4 sent
            var deltaSyncFrame150 = Server_Update150(serverSimulator, serverScene, clientId);

            // +++ Server: 160 / 120. Move command #3 received
            // --- Client: 160 / 120. Updated #4. Move command #5 sent.
            serverSimulator.ReceiveCommandFrame(commandFrame80);

            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(0.5f, 0, 1f));
            clientSimulator.Simulate();
            var commandFrame120 = clientSimulator.CommandFrame;

            // +++ Server: 180 / 140. Move command #3 received
            // --- Client: 180 / 140. Updated #4. Move command #6 sent.
            serverSimulator.ReceiveCommandFrame(commandFrame100);

            var commandFrame140 = Client_Update140_Command6(clientSimulator, clientScene);

            // --- Client: 190 / 150. Update #4 received
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame150);

            // +++ Server: 200 / 160. Move command #4 received. Update
            serverSimulator.ReceiveCommandFrame(commandFrame120);
            var deltaSyncFrame200 = Server_Update200(serverSimulator, serverScene, clientId);

            // --- Client: 200 / 160. Update #8
            var commandFrame160 = Client_Update160_Command7(clientSimulator, clientScene);

            // +++ Server: 220 / 180. Move command #5 received
            // --- Client: 220 / 180. Update #9
            serverSimulator.ReceiveCommandFrame(commandFrame140);
            Client_Update180(clientSimulator, clientScene);

            // +++ Server: 240 / 200. Move command #6 received
            // --- Client: 240 / 200. Update #4 received. Update #10
            serverSimulator.ReceiveCommandFrame(commandFrame160);
            Client_Update200(clientSimulator, clientScene);

            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame200);

            // +++ Server: 250 / 210. Update
            var deltaSyncFrame250 = Server_Update250(serverSimulator, serverScene, clientId);
        }

        private static void RegisterActorFactories(Simulator simulator)
        {
            simulator.RegisterActorFactory<ExampleSceneDesc, ExampleScene>(desc => new ExampleScene());
            simulator.RegisterActorFactory<BallDesc, Ball>(desc => new Ball());
            simulator.RegisterActorFactory<CubeDesc, Cube>(desc => new Cube());
        }

        private static FullSyncFrame Server_Update0(ServerSimulator simulator, ExampleScene scene)
        {
            simulator.Simulate();
            Assert.AreEqual(50, simulator.Time);

            var frame = simulator.DeltaSyncFrame;
            Assert.AreEqual(0, frame.Time);
            Assert.AreEqual(50, frame.DeltaTime);
            Assert.AreEqual(3, frame.Events.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.6f, ball.Position.X);
            AssertHelper.AreApproximate(2.57547498f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.6f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.07547498f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);

            return simulator.FullSyncFrame;
        }

        private static DeltaSyncFrame Server_Update50(ServerSimulator simulator, ExampleScene scene)
        {
            simulator.Simulate();
            Assert.AreEqual(100, simulator.Time);

            var frame = simulator.DeltaSyncFrame;
            Assert.AreEqual(50, frame.Time);
            Assert.AreEqual(50, frame.DeltaTime);
            Assert.AreEqual(2, frame.Events.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.6f, ball.Position.X);
            AssertHelper.AreApproximate(2.52642488f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.6f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.026425f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);

            return simulator.DeltaSyncFrame;
        }

        private static DeltaSyncFrame Server_Update100(ServerSimulator simulator, ExampleScene scene)
        {
            simulator.Simulate();
            Assert.AreEqual(150, simulator.Time);

            var frame = simulator.DeltaSyncFrame;
            Assert.AreEqual(100, frame.Time);
            Assert.AreEqual(50, frame.DeltaTime);
            Assert.AreEqual(2, frame.Events.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.6f, ball.Position.X);
            AssertHelper.AreApproximate(2.5f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.6f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(0.952849984f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);

            return simulator.DeltaSyncFrame;
        }

        private static DeltaSyncFrame Server_Update150(ServerSimulator simulator, ExampleScene scene, ClientId clientId)
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
            AssertHelper.AreApproximate(-2.36018968f, ball.Position.X);
            AssertHelper.AreApproximate(2.49999952f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.42014217f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(0.990574121f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);

            return simulator.DeltaSyncFrame;
        }

        private static DeltaSyncFrame Server_Update200(ServerSimulator simulator, ExampleScene scene, ClientId clientId)
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
            AssertHelper.AreApproximate(-1.95788312f, ball.Position.X);
            AssertHelper.AreApproximate(2.50000167f, ball.Position.Y);
            AssertHelper.AreApproximate(-1.99478006f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(0.992456913f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);

            return simulator.DeltaSyncFrame;
        }

        private static DeltaSyncFrame Server_Update250(ServerSimulator simulator, ExampleScene scene, ClientId clientId)
        {
            simulator.Simulate();
            Assert.AreEqual(300, simulator.Time);

            var frame = simulator.DeltaSyncFrame;
            Assert.AreEqual(250, frame.Time);
            Assert.AreEqual(50, frame.DeltaTime);
            Assert.AreEqual(2, frame.Events.Count);
            Assert.AreEqual(1, frame.LastCommandSeqs.Count);
            Assert.AreEqual(7, frame.LastCommandSeqs[clientId]);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-1.31003165f, ball.Position.X);
            AssertHelper.AreApproximate(2.49999928f, ball.Position.Y);
            AssertHelper.AreApproximate(-1.38612843f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(0.993965566f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);

            return simulator.DeltaSyncFrame;
        }

        private static void Client_Update0(ClientSimulator simulator, ExampleScene scene)
        {
            simulator.Simulate();
            Assert.AreEqual(20, simulator.Time);

            Assert.IsNull(simulator.CommandFrame);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.6f, ball.Position.X);
            AssertHelper.AreApproximate(2.59018993f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.6f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.09018993f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);
        }

        private static void Client_Update20(ClientSimulator simulator, ExampleScene scene)
        {
            simulator.Simulate();
            Assert.AreEqual(40, simulator.Time);

            Assert.IsNull(simulator.CommandFrame);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.6f, ball.Position.X);
            AssertHelper.AreApproximate(2.58037996f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.6f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.08037996f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);
        }

        private static CommandFrame Client_Update40_Command1(ClientSimulator simulator, ExampleScene scene)
        {
            var command = new KickBallCommand(1, 0, 1);
            simulator.ReceiveCommand(scene.Ball, command);

            simulator.Simulate();
            Assert.AreEqual(60, simulator.Time);

            var frame = simulator.CommandFrame;
            Assert.AreEqual(1, frame.Commands.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.55999994f, ball.Position.X);
            AssertHelper.AreApproximate(2.56664586f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.55999994f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.06664598f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);

            return simulator.CommandFrame;
        }

        private static CommandFrame Client_Update60_Command2(ClientSimulator simulator, ExampleScene scene)
        {
            var command = new KickBallCommand(1, 0, 0.5f);
            simulator.ReceiveCommand(scene.Ball, command);

            simulator.Simulate();
            Assert.AreEqual(80, simulator.Time);

            var frame = simulator.CommandFrame;
            Assert.AreEqual(1, frame.Commands.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.48000002f, ball.Position.X);
            AssertHelper.AreApproximate(2.54898787f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.5f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.04898798f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);

            return simulator.CommandFrame;
        }

        private static CommandFrame Client_Update80_Command3(ClientSimulator simulator, ExampleScene scene)
        {
            var command = new KickBallCommand(0.5f, 0, 1f);
            simulator.ReceiveCommand(scene.Ball, command);

            simulator.Simulate();
            Assert.AreEqual(100, simulator.Time);

            var frame = simulator.CommandFrame;
            Assert.AreEqual(1, frame.Commands.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.38000011f, ball.Position.X);
            AssertHelper.AreApproximate(2.52740598f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.4000001f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.02740598f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);

            return simulator.CommandFrame;
        }

        private static CommandFrame Client_Update100_Command4(ClientSimulator simulator, ExampleScene scene)
        {
            var command = new KickBallCommand(1f, 0, 1f);
            simulator.ReceiveCommand(scene.Ball, command);

            simulator.Simulate();
            Assert.AreEqual(120, simulator.Time);

            var frame = simulator.CommandFrame;
            Assert.AreEqual(1, frame.Commands.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-2.24000001f, ball.Position.X);
            AssertHelper.AreApproximate(2.50189996f, ball.Position.Y);
            AssertHelper.AreApproximate(-2.25999999f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(1.00189996f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);

            return simulator.CommandFrame;
        }

        private static CommandFrame Client_Update140_Command6(ClientSimulator simulator, ExampleScene scene)
        {
            var command = new KickBallCommand(1f, 0, 0.5f);
            simulator.ReceiveCommand(scene.Ball, command);

            simulator.Simulate();
            Assert.AreEqual(160, simulator.Time);

            var frame = simulator.CommandFrame;
            Assert.AreEqual(1, frame.Commands.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-1.89017844f, ball.Position.X);
            AssertHelper.AreApproximate(2.49999475f, ball.Position.Y);
            AssertHelper.AreApproximate(-1.89131224f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(0.977977574f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);

            return simulator.CommandFrame;
        }

        private static CommandFrame Client_Update160_Command7(ClientSimulator simulator, ExampleScene scene)
        {
            var command = new KickBallCommand(1f, 0, 1f);
            simulator.ReceiveCommand(scene.Ball, command);

            simulator.Simulate();
            Assert.AreEqual(180, simulator.Time);

            var frame = simulator.CommandFrame;
            Assert.AreEqual(1, frame.Commands.Count);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-1.45090246f, ball.Position.X);
            AssertHelper.AreApproximate(2.49999809f, ball.Position.Y);
            AssertHelper.AreApproximate(-1.51089561f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.1f, cube.Position.X);
            AssertHelper.AreApproximate(0.99691087f, cube.Position.Y);
            AssertHelper.AreApproximate(1.1f, cube.Position.Z);

            return simulator.CommandFrame;
        }

        private static void Client_Update180(ClientSimulator simulator, ExampleScene scene)
        {
            simulator.Simulate();
            Assert.AreEqual(200, simulator.Time);

            Assert.IsNull(simulator.CommandFrame);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-1.22768235f, ball.Position.X);
            AssertHelper.AreApproximate(2.51132655f, ball.Position.Y);
            AssertHelper.AreApproximate(-1.29394102f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.35487795f, cube.Position.X);
            AssertHelper.AreApproximate(0.977353811f, cube.Position.Y);
            AssertHelper.AreApproximate(1.41498196f, cube.Position.Z);
        }

        private static void Client_Update200(ClientSimulator simulator, ExampleScene scene)
        {
            simulator.Simulate();
            Assert.AreEqual(220, simulator.Time);

            Assert.IsNull(simulator.CommandFrame);

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            AssertHelper.AreApproximate(-0.998183846f, ball.Position.X);
            AssertHelper.AreApproximate(2.51692462f, ball.Position.Y);
            AssertHelper.AreApproximate(-1.07656932f, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            AssertHelper.AreApproximate(1.54598451f, cube.Position.X);
            AssertHelper.AreApproximate(0.979288876f, cube.Position.Y);
            AssertHelper.AreApproximate(1.72425747f, cube.Position.Z);
        }
    }
}
