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

        private static SimplePhysicsScene clientScene;
        private static ClientSimulator clientSimulator;
        private static int elapsedTime;
        private static bool stopRequest;

        public static void Run()
        {
            ThreadPool.QueueUserWorkItem(RunServer);
            ThreadPool.QueueUserWorkItem(RunClient);

            while (true)
            {
                var key = Console.ReadKey();

                switch (key.Key)
                {

                    case ConsoleKey.Spacebar:
                        elapsedTime += 10;
                        logger.Info("Time elapsed {0}ms", elapsedTime);

                        serverStep.Set();
                        clientStep.Set();
                        break;

                    case ConsoleKey.Escape:
                        stopRequest = true;
                        return;

                    case ConsoleKey.LeftArrow:
                        if (clientSimulator != null)
                            clientSimulator.ReceiveCommand(clientScene.Ball.Id, new MoveBallCommand() { InputX = -1 });
                        break;

                    case ConsoleKey.UpArrow:
                        if (clientSimulator != null)
                            clientSimulator.ReceiveCommand(clientScene.Ball.Id, new MoveBallCommand() { InputY = 1 });
                        break;

                    case ConsoleKey.RightArrow:
                        if (clientSimulator != null)
                            clientSimulator.ReceiveCommand(clientScene.Ball.Id, new MoveBallCommand() { InputX = 1 });
                        break;

                    case ConsoleKey.DownArrow:
                        if (clientSimulator != null)
                            clientSimulator.ReceiveCommand(clientScene.Ball.Id, new MoveBallCommand() { InputY = -1 });
                        break;

                    default:
                        logger.Warn("Unsupported input: {0}", key);
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
                var nextTime = 10;
                WaitUntilTime(serverStep, nextTime);
                server.Start();

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
            clientSimulator = new ClientSimulator(clientId, 50);
            clientScene = new SimplePhysicsScene(clientSimulator);
            var serverAddress = new IPEndPoint(IPAddress.Loopback, 9000);
            using (var client = new ReliableUdpClient(clientSimulator, 20, serverAddress))
            {
                var nextTime = 40;
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
