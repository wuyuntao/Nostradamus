using Nostradamus.Client;
using Nostradamus.Networking;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Nostradamus.Examples
{
    class SimpleClientExample
    {
        public static void Run()
        {
            var clientId = new ClientId(1);
            var sceneDesc = SimplePhysicsScene.CreateSceneDesc();
            {
                sceneDesc.Mode = SceneMode.Client;
                sceneDesc.SimulationDeltaTime = 20;
                sceneDesc.ReconciliationDeltaTime = 50;
            };
            var scene = new SimplePhysicsScene(sceneDesc);
            var serverAddress = new IPEndPoint(IPAddress.Loopback, 9000);
            using (var client = new ReliableUdpClient(scene, serverAddress))
            {
                client.Start();

                var timer = Stopwatch.StartNew();

                for (var i = 0; ; i++)
                {
                    var simulateTime = 20 * i;
                    var waitTime = simulateTime - (int)timer.ElapsedMilliseconds;
                    if (waitTime > 0)
                        Thread.Sleep(waitTime);

                    client.Update();
                }

                client.Stop();
            }
        }
    }
}
