using BulletSharp.Math;
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
            Server_Simulate(serverSimulator, serverScene, 0, 3, null, new Vector3(-2.6f, 2.57547498f, -2.6f), new Vector3(1.1f, 1.07547498f, 1.1f));
            var fullSyncFrame0 = serverSimulator.FullSyncFrame;

            // --- Client: 40 / 0. Update #1 received
            clientSimulator.ReceiveFullSyncFrame(fullSyncFrame0);
            Client_Simulate(clientSimulator, clientScene, 0, null, new Vector3(-2.6f, 2.59018993f, -2.6f), new Vector3(1.1f, 1.09018993f, 1.1f));

            // +++ Server: 50 / 10. Update #2 sent
            var deltaSyncFrame50 = Server_Simulate(serverSimulator, serverScene, 50, 2, null, new Vector3(-2.6f, 2.52642488f, -2.6f), new Vector3(1.1f, 1.026425f, 1.1f));

            // --- Client: 60 / 20. Update #2
            Client_Simulate(clientSimulator, clientScene, 20, null, new Vector3(-2.6f, 2.58037996f, -2.6f), new Vector3(1.1f, 1.08037996f, 1.1f));

            // --- Client: 80 / 40. Updated #3. Move command #1 sent
            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(1, 0, 1));
            var commandFrame40 = Client_Simulate(clientSimulator, clientScene, 40, 1, new Vector3(-2.55999994f, 2.56664586f, -2.55999994f), new Vector3(1.1f, 1.06664598f, 1.1f));

            // --- Client: 90 / 50. Update #2 received
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame50);

            // +++ Server: 100 / 60. Update #3 sent
            // --- Client: 100 / 60. Updated #3. Move command #2 sent.
            var deltaSyncFrame100 = Server_Simulate(serverSimulator, serverScene, 100, 2, null, new Vector3(-2.6f, 2.5f, -2.6f), new Vector3(1.1f, 0.952849984f, 1.1f));

            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(1, 0, 0.5f));
            var commandFrame60 = Client_Simulate(clientSimulator, clientScene, 60, 2, new Vector3(-2.48000002f, 2.54898787f, -2.5f), new Vector3(1.1f, 1.04898798f, 1.1f));

            // +++ Server: 120 / 80. Move command #1 received
            // --- Client: 120 / 80. Updated #4. Move command #3 sent.
            serverSimulator.ReceiveCommandFrame(commandFrame40);

            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(0.5f, 0, 1f));
            var commandFrame80 = Client_Simulate(clientSimulator, clientScene, 80, 3, new Vector3(-2.38000011f, 2.52740598f, -2.4000001f), new Vector3(1.1f, 1.02740598f, 1.1f));

            // +++ Server: 140 / 100. Move command #2 received
            // --- Client: 140 / 100. Update #3 received. Updated #5. Move command #4 sent.
            serverSimulator.ReceiveCommandFrame(commandFrame60);

            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame100);
            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(1, 0, 1));
            var commandFrame100 = Client_Simulate(clientSimulator, clientScene, 100, 4, new Vector3(-2.24000001f, 2.50189996f, -2.25999999f), new Vector3(1.1f, 1.00189996f, 1.1f));

            // +++ Server: 150 / 110. Update #4 sent
            var deltaSyncFrame150 = Server_Simulate(serverSimulator, serverScene, 150, 2, 2, new Vector3(-2.36018968f, 2.49999952f, -2.42014217f), new Vector3(1.1f, 0.990574121f, 1.1f));

            // +++ Server: 160 / 120. Move command #3 received
            // --- Client: 160 / 120. Updated #4. Move command #5 sent.
            serverSimulator.ReceiveCommandFrame(commandFrame80);

            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(0.5f, 0, 1f));
            var commandFrame120 = Client_Simulate(clientSimulator, clientScene, 120, 5);

            // +++ Server: 180 / 140. Move command #3 received
            // --- Client: 180 / 140. Updated #4. Move command #6 sent.
            serverSimulator.ReceiveCommandFrame(commandFrame100);

            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(1, 0, 0.5f));
            var commandFrame140 = Client_Simulate(clientSimulator, clientScene, 140, 6, new Vector3(-1.89017844f, 2.49999475f, -1.89131224f), new Vector3(1.1f, 0.977977574f, 1.1f));

            // --- Client: 190 / 150. Update #4 received
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame150);

            // +++ Server: 200 / 160. Move command #4 received. Update
            // --- Client: 200 / 160. Update #8
            serverSimulator.ReceiveCommandFrame(commandFrame120);
            var deltaSyncFrame200 = Server_Simulate(serverSimulator, serverScene, 200, 2, 5, new Vector3(-1.95788312f, 2.50000167f, -1.99478006f), new Vector3(1.1f, 0.992456913f, 1.1f));

            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(1, 0, 1));
            var commandFrame160 = Client_Simulate(clientSimulator, clientScene, 160, 7, new Vector3(-1.45090246f, 2.49999809f, -1.51089561f), new Vector3(1.1f, 0.99691087f, 1.1f));

            // +++ Server: 220 / 180. Move command #5 received
            // --- Client: 220 / 180. Update #9
            serverSimulator.ReceiveCommandFrame(commandFrame140);

            Client_Simulate(clientSimulator, clientScene, 180, null, new Vector3(-1.22768235f, 2.51132655f, -1.29394102f), new Vector3(1.35487795f, 0.977353811f, 1.41498196f));

            // +++ Server: 240 / 200. Move command #6 received
            // --- Client: 240 / 200. Update #4 received. Update #10
            serverSimulator.ReceiveCommandFrame(commandFrame160);

            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame200);
            Client_Simulate(clientSimulator, clientScene, 200, null, new Vector3(-1.08297396f, 2.51228547f, -1.17307234f), new Vector3(1.47495699f, 1.00047791f, 1.65562773f));

            // +++ Server: 250 / 210. Update
            var deltaSyncFrame250 = Server_Simulate(serverSimulator, serverScene, 250, 2, 7, new Vector3(-1.31003165f, 2.49999928f, -1.38612843f), new Vector3(1.1f, 0.993965566f, 1.1f));

            // --- Client: 260 / 220. Update #11
            Client_Simulate(clientSimulator, clientScene, 220, null, new Vector3(-0.859388888f, 2.51063299f, -0.972703457f), new Vector3(1.59915924f, 1.07109916f, 1.93974078f));

            // --- Client: 280 / 240. Update #12
            Client_Simulate(clientSimulator, clientScene, 240, null, new Vector3(-0.635803819f, 2.50505662f, -0.772334576f), new Vector3(1.72312438f, 1.13557923f, 2.22354245f));

            // --- Client: 290 / 250. Update #5 received
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame250);

            // +++ Server: 300 / 260. Update #6 sent
            // --- Client: 300 / 260. Update #13
            var deltaSyncFrame300 = Server_Simulate(serverSimulator, serverScene, 300, 2, null, new Vector3(-0.7320382f, 2.51120472f, -0.854788482f), new Vector3(1.70734727f, 0.949980617f, 1.86764681f));

            Client_Simulate(clientSimulator, clientScene, 260, null, new Vector3(-0.565702319f, 2.5039165f, -0.721153975f), new Vector3(1.82367539f, 1.0947907f, 2.22697306f));

            // --- Client: 320 / 280. Update #14
            Client_Simulate(clientSimulator, clientScene, 280, null, new Vector3(-0.625910521f, 2.5f, -0.771388412f), new Vector3(1.68042278f, 1.08080566f, 2.02833533f));

            // --- Client: 340 / 300. Update #6 received. Update #15
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame300);
            Client_Simulate(clientSimulator, clientScene, 300, null, new Vector3(-0.597975135f, 2.50154448f, -0.74189055f), new Vector3(1.66584289f, 1.0770092f, 1.98309708f));

            // +++ Server: 350 / 310. Update #7 sent
            var deltaSyncFrame350 = Server_Simulate(serverSimulator, serverScene, 350, 2, null, new Vector3(-0.146978855f, 2.50327706f, -0.327411592f), new Vector3(2.22478914f, 1.00451052f, 2.65568161f));

            // --- Client: 360 / 320. Update #16
            Client_Simulate(clientSimulator, clientScene, 320, null, new Vector3(-0.508801579f, 2.50313115f, -0.65755105f), new Vector3(1.74411011f, 1.06711984f, 2.05903816f));

            // --- Client: 380 / 340. Update #17
            Client_Simulate(clientSimulator, clientScene, 340, null, new Vector3(-0.376911879f, 2.50444794f, -0.535113215f), new Vector3(1.88332546f, 1.07341897f, 2.21947169f));

            // --- Client: 390 / 350. Update #7 received
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame350);

            // +++ Server: 400 / 360. Update #8 sent
            // --- Client: 400 / 360. Update #18
            var deltaSyncFrame400 = Server_Simulate(serverSimulator, serverScene, 400, 2, null, new Vector3(0.44053477f, 2.5000093f, 0.20445317f), new Vector3(2.64987993f, 1.07651877f, 3.33675551f));

            Client_Simulate(clientSimulator, clientScene, 360, null, new Vector3(-0.21414946f, 2.50232458f, -0.385905981f), new Vector3(2.04752374f, 1.05890346f, 2.43156719f));

            // --- Client: 420 / 380. Update #19
            Client_Simulate(clientSimulator, clientScene, 380, null, new Vector3(-0.0305711217f, 2.50141001f, -0.219277292f), new Vector3(2.22963047f, 1.07745433f, 2.68564701f));

            // --- Client: 440 / 400. Update #8 received. Update #20
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame400);
            Client_Simulate(clientSimulator, clientScene, 400, null, new Vector3(0.169435292f, 2.50058842f, -0.0373911075f), new Vector3(2.39641023f, 1.08326018f, 2.9297502f));

            // +++ Server: 450 / 410. Update #9 sent
            var deltaSyncFrame450 = Server_Simulate(serverSimulator, serverScene, 450, 2, null, new Vector3(1.02287209f, 2.50000739f, 0.731683254f), new Vector3(3.07375026f, 1.12338817f, 4.01623106f));

            // --- Client: 460 / 420. Update #21
            Client_Simulate(clientSimulator, clientScene, 420, null, new Vector3(0.379583418f, 2.50020289f, 0.152928486f), new Vector3(2.56465197f, 1.09716523f, 3.1880331f));

            // --- Client: 480 / 440. Update #22
            Client_Simulate(clientSimulator, clientScene, 440, null, new Vector3(0.596808672f, 2.49980998f, 0.349286288f), new Vector3(2.7337172f, 1.11266732f, 3.45449305f));

            // --- Client: 490 / 450. Update #9 received
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame450);

            // +++ Server: 500 / 460. Update #10 sent
            // --- Client: 500 / 460. Update #23
            var deltaSyncFrame500 = Server_Simulate(serverSimulator, serverScene, 500, 2, null, new Vector3(1.60064721f, 2.49997926f, 1.2548281f), new Vector3(3.49634385f, 1.14749622f, 4.69400597f));

            Client_Simulate(clientSimulator, clientScene, 460, null, new Vector3(0.818096817f, 2.49989629f, 0.549179733f), new Vector3(2.90302205f, 1.12838721f, 3.72518826f));

            // --- Client: 520 / 480. Update #24
            Client_Simulate(clientSimulator, clientScene, 480, null, new Vector3(1.04243374f, 2.5f, 0.751805663f), new Vector3(3.07245421f, 1.1424191f, 3.99803948f));

            // --- Client: 540 / 500. Update #10 received. Update #25
            clientSimulator.ReceiveDeltaSyncFrame(deltaSyncFrame500);
            Client_Simulate(clientSimulator, clientScene, 500, null, new Vector3(1.26833737f, 2.49997139f, 0.955887437f), new Vector3(3.24176168f, 1.15240836f, 4.27158785f));
        }

        private static void RegisterActorFactories(Simulator simulator)
        {
            simulator.RegisterActorFactory<ExampleSceneDesc, ExampleScene>(desc => new ExampleScene());
            simulator.RegisterActorFactory<BallDesc, Ball>(desc => new Ball());
            simulator.RegisterActorFactory<CubeDesc, Cube>(desc => new Cube());
        }

        private static DeltaSyncFrame Server_Simulate(ServerSimulator simulator, ExampleScene scene, int time, int expectedEventCount, int? expectedCommandSeq = null, Vector3? expectedBallPos = null, Vector3? expectedCubePos = null)
        {
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

            if (expectedBallPos != null)
            {
                var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
                AssertHelper.AreApproximate(expectedBallPos.Value.X, ball.Position.X);
                AssertHelper.AreApproximate(expectedBallPos.Value.Y, ball.Position.Y);
                AssertHelper.AreApproximate(expectedBallPos.Value.Z, ball.Position.Z);
            }

            if (expectedCubePos != null)
            {
                var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
                AssertHelper.AreApproximate(expectedCubePos.Value.X, cube.Position.X);
                AssertHelper.AreApproximate(expectedCubePos.Value.Y, cube.Position.Y);
                AssertHelper.AreApproximate(expectedCubePos.Value.Z, cube.Position.Z);
            }

            return frame;
        }

        private static CommandFrame Client_Simulate(ClientSimulator simulator, ExampleScene scene, int time, int? expectedCommandSeq = null, Vector3? expectedBallPos = null, Vector3? expectedCubePos = null)
        {
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

            if (expectedBallPos != null)
            {
                var ball = (RigidBodySnapshot)scene.Ball.Snapshot;
                AssertHelper.AreApproximate(expectedBallPos.Value.X, ball.Position.X);
                AssertHelper.AreApproximate(expectedBallPos.Value.Y, ball.Position.Y);
                AssertHelper.AreApproximate(expectedBallPos.Value.Z, ball.Position.Z);
            }

            if (expectedCubePos != null)
            {
                var cube = (RigidBodySnapshot)scene.Cube.Snapshot;
                AssertHelper.AreApproximate(expectedCubePos.Value.X, cube.Position.X);
                AssertHelper.AreApproximate(expectedCubePos.Value.Y, cube.Position.Y);
                AssertHelper.AreApproximate(expectedCubePos.Value.Z, cube.Position.Z);
            }

            return frame;
        }
    }
}
