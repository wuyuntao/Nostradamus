using NLog;
using Nostradamus.Client;
using Nostradamus.Networking;
using Nostradamus.Server;
using System;
using System.Net;
using System.Threading;

namespace Nostradamus.Examples
{
    class StepByStepExample
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly AutoResetEvent serverStep = new AutoResetEvent(false);
        private static readonly AutoResetEvent clientStep = new AutoResetEvent(false);
        private static int elapsedTime;
        private static bool stopRequest;

        public static void Run()
        {
            ThreadPool.QueueUserWorkItem(RunServer);
            ThreadPool.QueueUserWorkItem(RunClient);

            while (true)
            {
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.Spacebar)
                {
                    elapsedTime += 10;
                    logger.Info("Time elapsed {0}ms", elapsedTime);

                    if (elapsedTime % 50 == 0)
                        serverStep.Set();

                    if (elapsedTime % 20 == 0)
                        clientStep.Set();
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    stopRequest = true;
                    break;
                }
            }
        }

        private static void RunServer(object state)
        {
            var simulator = new ServerSimulator();
            var scene = new SimplePhysicsScene(simulator);
            using (var server = new ReliableUdpServer(simulator, 50, 9000))
            {
                server.Start();

                var nextTime = 0;
                while (!stopRequest)
                {
                    WaitUntilTime(serverStep, nextTime);
                    logger.Info("Server update at {0}ms", nextTime);
                    server.Update();
                    nextTime += 50;
                }

                server.Stop();
            }
        }

        private static void RunClient(object state)
        {
            var clientId = new ClientId(1);
            var simulator = new ClientSimulator(clientId, 50);
            var scene = new SimplePhysicsScene(simulator);
            var serverAddress = new IPEndPoint(IPAddress.Loopback, 9000);
            using (var client = new ReliableUdpClient(simulator, 20, serverAddress))
            {
                var nextTime = 100;
                WaitUntilTime(clientStep, nextTime);
                client.Start();

                while (!stopRequest)
                {
                    WaitUntilTime(clientStep, nextTime);
                    logger.Info("Client update at {0}ms", nextTime);
                    client.Update();
                    nextTime += 20;
                }

                client.Stop();
            }
        }

        private static void WaitUntilTime(AutoResetEvent step, int targetTime)
        {
            while (elapsedTime < targetTime)
            {
                step.WaitOne();
            }
        }
    }
}
