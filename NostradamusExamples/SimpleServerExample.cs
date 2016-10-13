using Nostradamus.Networking;
using Nostradamus.Server;
using System.Diagnostics;
using System.Threading;

namespace Nostradamus.Examples
{
    class SimpleServerExample
    {
        public static void Run()
        {

            var serverSimulator = new ServerSimulator();
            RegisterActorFactories(serverSimulator);

            var serverSceneDesc = new ExampleSceneDesc(50, 50);
            var serverScene = serverSimulator.CreateScene<ExampleScene>(serverSceneDesc);

            using (var server = new ReliableUdpServer(serverSimulator, 9000))
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

        private static void RegisterActorFactories(Simulator simulator)
        {
            simulator.RegisterActorFactory<ExampleSceneDesc, ExampleScene>(desc => new ExampleScene());
            simulator.RegisterActorFactory<BallDesc, Ball>(desc => new Ball());
            simulator.RegisterActorFactory<CubeDesc, Cube>(desc => new Cube());
        }
    }
}
