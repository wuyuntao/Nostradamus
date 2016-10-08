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
            var simulator = new ClientSimulator(clientId, 50);
            var scene = new SimplePhysicsScene(simulator);
            var serverAddress = new IPEndPoint(IPAddress.Loopback, 9000);
            using (var client = new ReliableUdpClient(simulator, 20, serverAddress))
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
