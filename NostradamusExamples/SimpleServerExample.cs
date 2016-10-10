using Nostradamus.Networking;
using System.Diagnostics;
using System.Threading;

namespace Nostradamus.Examples
{
    class SimpleServerExample
    {
        public static void Run()
        {
            var sceneDesc = SimplePhysicsScene.CreateSceneDesc();
            {
                sceneDesc.Mode = SceneMode.Server;
                sceneDesc.SimulationDeltaTime = 50;
            };
            var scene = new SimplePhysicsScene(sceneDesc);

            using (var server = new ReliableUdpServer(scene, 9000))
            {
                server.Start();

                var timer = Stopwatch.StartNew();

                for (var i = 0; ; i++)
                {
                    var simulateTime = 50 * i;
                    var waitTime = simulateTime - (int)timer.ElapsedMilliseconds;
                    if (waitTime > 0)
                        Thread.Sleep(waitTime);

                    server.Update();
                }

                server.Stop();
            }
        }
    }
}
