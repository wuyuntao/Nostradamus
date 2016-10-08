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
            var simulator = new ServerSimulator();
            var scene = new SimplePhysicsScene(simulator);
            using (var server = new ReliableUdpServer(simulator, 50, 9000))
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
