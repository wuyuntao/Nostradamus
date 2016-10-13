using NLog;
using Nostradamus.Client;
using Nostradamus.Networking;
using Nostradamus.Server;
using System;
using System.Net;

namespace Nostradamus.Examples
{
    class StepByStepExample
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static void Run()
        {
            var elapsedTime = 0;

            var serverSimulator = new ServerSimulator();
            RegisterActorFactories(serverSimulator);

            var serverSceneDesc = new ExampleSceneDesc(50, 50);
            var serverScene = serverSimulator.CreateScene<ExampleScene>(serverSceneDesc);

            var server = new ReliableUdpServer(serverSimulator, 9000);
            server.Start();
            var serverNextTime = 0;
            var serverAddress = new IPEndPoint(IPAddress.Loopback, 9000);

            var clientId = new ClientId(1);
            var clientSimulator = new ClientSimulator(clientId);
            RegisterActorFactories(clientSimulator);

            var clientSceneDesc = new ExampleSceneDesc(20, 50);
            var clientScene = clientSimulator.CreateScene<ExampleScene>(clientSceneDesc);

            var client = new ReliableUdpClient(clientSimulator, serverAddress);
            var clientNextTime = 60;
            var clientRunning = false;

            while (true)
            {
                var key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.Spacebar:
                        elapsedTime += 10;
                        logger.Info("Time elapsed {0}ms", elapsedTime);

                        if (serverNextTime <= elapsedTime)
                        {
                            logger.Info("++++ Begin server update at {0}ms", serverNextTime);
                            server.Update();
                            //PrintActorSnapshots(serverScene, serverScene.Time + serverScene.DeltaTime);
                            logger.Info("++++ End server update at {0}ms", serverNextTime);

                            serverNextTime += 50;
                        }

                        if (clientNextTime <= elapsedTime)
                        {
                            if (clientNextTime == 60)
                            {
                                client.Start();

                                logger.Info("Client started at {0}ms", clientNextTime);
                                clientNextTime += 20;
                            }
                            else
                            {
                                logger.Info("---- Begin Client update at {0}ms", clientNextTime);
                                client.Update();
                                clientRunning = true;
                                logger.Info("---- End Client update at {0}ms", clientNextTime);

                                //PrintActorSnapshots(clientScene, elapsedTime - 60);
                                clientNextTime += 20;
                            }
                        }
                        else if (clientRunning)
                        {
                            //PrintActorSnapshots(clientScene, elapsedTime - 60);
                        }

                        break;

                    case ConsoleKey.Escape:
                        goto End;

                    case ConsoleKey.LeftArrow:
                        if (clientSimulator != null && clientScene.Ball != null)
                            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(x: -1));
                        break;

                    case ConsoleKey.UpArrow:
                        if (clientSimulator != null && clientScene.Ball != null)
                            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(z: 1));
                        break;

                    case ConsoleKey.RightArrow:
                        if (clientSimulator != null && clientScene.Ball != null)
                            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(x: 1));
                        break;

                    case ConsoleKey.DownArrow:
                        if (clientSimulator != null && clientScene.Ball != null)
                            clientSimulator.ReceiveCommand(clientScene.Ball, new KickBallCommand(z: -1));
                        break;

                    default:
                        logger.Warn("Unsupported input: {0}", key.Key);
                        break;
                }
            }

            End:
            server.Stop();
            server.Dispose();

            client.Stop();
            client.Dispose();
        }

        private static void RegisterActorFactories(Simulator simulator)
        {
            simulator.RegisterActorFactory<ExampleSceneDesc, ExampleScene>(desc => new ExampleScene());
            simulator.RegisterActorFactory<BallDesc, Ball>(desc => new Ball());
            simulator.RegisterActorFactory<CubeDesc, Cube>(desc => new Cube());
        }
    }
}
