using BulletSharp.Math;
using NLog;
using Nostradamus.Client;
using Nostradamus.Examples;
using Nostradamus.Physics;
using Nostradamus.Server;
using NUnit.Framework;

namespace Nostradamus.Tests
{
    public class ClientSimulatorTest
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

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
            Server_Simulate(serverSimulator, serverScene, 0, 3, null);
            var fullSyncFrame0 = serverSimulator.FullSyncFrame;

            // --- Client: 40 / 0. Update #1 received
            clientSimulator.ReceiveFullSyncFrame(fullSyncFrame0);
            Client_Simulate(clientSimulator, clientScene, 0, null);

            // +++ Server: 50 / 10. Update #2 sent
            var deltaSyncFrame50 = Server_Simulate(serverSimulator, serverScene, 50, 2, null);

            // --- Client: 60 / 20. Update #2
            Client_Simulate(clientSimulator, clientScene, 20, null);

            // --- Client: 80 / 40. Updated #3. Move command #1 sent
            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(1, 0, 1));
            var commandFrame40 = Client_Simulate(clientSimulator, clientScene, 40, 1);

            // --- Client: 90 / 50. Update #2 received
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame50);

            // +++ Server: 100 / 60. Update #3 sent
            // --- Client: 100 / 60. Updated #3. Move command #2 sent.
            var deltaSyncFrame100 = Server_Simulate(serverSimulator, serverScene, 100, 2, null);

            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(1, 0, 0.5f));
            var commandFrame60 = Client_Simulate(clientSimulator, clientScene, 60, 2);

            // +++ Server: 120 / 80. Move command #1 received
            // --- Client: 120 / 80. Updated #4. Move command #3 sent.
            serverSimulator.ReceiveCommandFrame(commandFrame40);

            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(0.5f, 0, 1f));
            var commandFrame80 = Client_Simulate(clientSimulator, clientScene, 80, 3);

            // +++ Server: 140 / 100. Move command #2 received
            // --- Client: 140 / 100. Update #3 received. Updated #5. Move command #4 sent.
            serverSimulator.ReceiveCommandFrame(commandFrame60);

            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame100);
            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(1, 0, 1));
            var commandFrame100 = Client_Simulate(clientSimulator, clientScene, 100, 4);

            // +++ Server: 150 / 110. Update #4 sent
            var deltaSyncFrame150 = Server_Simulate(serverSimulator, serverScene, 150, 2, 2);

            // +++ Server: 160 / 120. Move command #3 received
            // --- Client: 160 / 120. Updated #4. Move command #5 sent.
            serverSimulator.ReceiveCommandFrame(commandFrame80);

            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(0.5f, 0, 1f));
            var commandFrame120 = Client_Simulate(clientSimulator, clientScene, 120, 5);

            // +++ Server: 180 / 140. Move command #3 received
            // --- Client: 180 / 140. Updated #4. Move command #6 sent.
            serverSimulator.ReceiveCommandFrame(commandFrame100);

            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(1, 0, 0.5f));
            var commandFrame140 = Client_Simulate(clientSimulator, clientScene, 140, 6);

            // --- Client: 190 / 150. Update #4 received
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame150);

            // +++ Server: 200 / 160. Move command #4 received. Update
            // --- Client: 200 / 160. Update #8
            serverSimulator.ReceiveCommandFrame(commandFrame120);
            var deltaSyncFrame200 = Server_Simulate(serverSimulator, serverScene, 200, 2, 5);

            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(1, 0, 1));
            var commandFrame160 = Client_Simulate(clientSimulator, clientScene, 160, 7);

            // +++ Server: 220 / 180. Move command #5 received
            // --- Client: 220 / 180. Update #9
            serverSimulator.ReceiveCommandFrame(commandFrame140);

            Client_Simulate(clientSimulator, clientScene, 180, null);

            // +++ Server: 240 / 200. Move command #6 received
            // --- Client: 240 / 200. Update #4 received. Update #10
            serverSimulator.ReceiveCommandFrame(commandFrame160);

            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame200);
            Client_Simulate(clientSimulator, clientScene, 200, null);

            // +++ Server: 250 / 210. Update
            var deltaSyncFrame250 = Server_Simulate(serverSimulator, serverScene, 250, 2, 7);

            // --- Client: 260 / 220. Update #11
            Client_Simulate(clientSimulator, clientScene, 220, null);

            // --- Client: 280 / 240. Update #12
            Client_Simulate(clientSimulator, clientScene, 240, null);

            // --- Client: 290 / 250. Update #5 received
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame250);

            // +++ Server: 300 / 260. Update #6 sent
            // --- Client: 300 / 260. Update #13
            var deltaSyncFrame300 = Server_Simulate(serverSimulator, serverScene, 300, 2, null);

            Client_Simulate(clientSimulator, clientScene, 260, null);

            // --- Client: 320 / 280. Update #14
            Client_Simulate(clientSimulator, clientScene, 280, null);

            // --- Client: 340 / 300. Update #6 received. Update #15
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame300);
            Client_Simulate(clientSimulator, clientScene, 300, null);

            // +++ Server: 350 / 310. Update #7 sent
            var deltaSyncFrame350 = Server_Simulate(serverSimulator, serverScene, 350, 2, null);

            // --- Client: 360 / 320. Update #16
            Client_Simulate(clientSimulator, clientScene, 320, null);

            // --- Client: 380 / 340. Update #17
            Client_Simulate(clientSimulator, clientScene, 340, null);

            // --- Client: 390 / 350. Update #7 received
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame350);

            // +++ Server: 400 / 360. Update #8 sent
            // --- Client: 400 / 360. Update #18
            var deltaSyncFrame400 = Server_Simulate(serverSimulator, serverScene, 400, 2, null);

            Client_Simulate(clientSimulator, clientScene, 360, null);

            // --- Client: 420 / 380. Update #19
            Client_Simulate(clientSimulator, clientScene, 380, null);

            // --- Client: 440 / 400. Update #8 received. Update #20
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame400);
            Client_Simulate(clientSimulator, clientScene, 400, null);

            // +++ Server: 450 / 410. Update #9 sent
            var deltaSyncFrame450 = Server_Simulate(serverSimulator, serverScene, 450, 2, null);

            // --- Client: 460 / 420. Update #21
            Client_Simulate(clientSimulator, clientScene, 420, null);

            // --- Client: 480 / 440. Update #22
            Client_Simulate(clientSimulator, clientScene, 440, null);

            // --- Client: 490 / 450. Update #9 received
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame450);

            // +++ Server: 500 / 460. Update #10 sent
            // --- Client: 500 / 460. Update #23
            var deltaSyncFrame500 = Server_Simulate(serverSimulator, serverScene, 500, 2, null);

            Client_Simulate(clientSimulator, clientScene, 460, null);

            // --- Client: 520 / 480. Update #24
            Client_Simulate(clientSimulator, clientScene, 480, null);

            // --- Client: 540 / 500. Update #10 received. Update #25
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame500);
            Client_Simulate(clientSimulator, clientScene, 500, null);
        }

        private static void RegisterActorFactories(Simulator simulator)
        {
            simulator.RegisterActorFactory<ExampleSceneDesc, ExampleScene>(desc => new ExampleScene());
            simulator.RegisterActorFactory<BallDesc, Ball>(desc => new Ball());
            simulator.RegisterActorFactory<CubeDesc, Cube>(desc => new Cube());
        }

        private static DeltaSyncFrame Server_Simulate(ServerSimulator simulator, ExampleScene scene, int time, int expectedEventCount, int? expectedCommandSeq = null)
        {
            logger.Error("Begin server simulation: {0}ms", time);

            simulator.Simulate();
            Assert.AreEqual(time + scene.Desc.SimulationDeltaTime, simulator.Time);

            var frame = simulator.DeltaSyncFrame;
            Assert.AreEqual(time, frame.Time);
            Assert.AreEqual(scene.Desc.SimulationDeltaTime, frame.DeltaTime);
            Assert.AreEqual(expectedEventCount, frame.Events.Count);

            if (expectedCommandSeq == null)
            {
                Assert.AreEqual(0, frame.LastCommandSeqs.Count);
            }
            else
            {
                var clientId = new ClientId(1);
                Assert.AreEqual(1, frame.LastCommandSeqs.Count);
                Assert.AreEqual(expectedCommandSeq.Value, frame.LastCommandSeqs[clientId]);
            }

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            logger.Debug("Ball position: {1}f, {2}f, {3}f", time, ball.Position.X, ball.Position.Y, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            logger.Debug("Cube position: {1}f, {2}f, {3}f", time, cube.Position.X, cube.Position.Y, cube.Position.Z);

            logger.Error("End server simulation: {0}ms", time);

            return frame;
        }

        private static CommandFrame Client_Simulate(ClientSimulator simulator, ExampleScene scene, int time, int? expectedCommandSeq = null)
        {
            logger.Warn("Begin client simulation: {0}ms", time);

            simulator.Simulate();
            Assert.AreEqual(time + scene.Desc.SimulationDeltaTime, simulator.Time);

            var frame = simulator.CommandFrame;
            if (expectedCommandSeq == null)
            {
                Assert.IsNull(frame);
            }
            else
            {
                Assert.AreEqual(1, frame.Commands.Count);
                Assert.AreEqual(expectedCommandSeq.Value, frame.Commands[0].Sequence);
            }

            var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
            logger.Debug("Ball position: {1}f, {2}f, {3}f", time, ball.Position.X, ball.Position.Y, ball.Position.Z);

            var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
            logger.Debug("Cube position: {1}f, {2}f, {3}f", time, cube.Position.X, cube.Position.Y, cube.Position.Z);

            logger.Warn("End client simulation: {0}ms", time);

            return frame;
        }
    }
}
