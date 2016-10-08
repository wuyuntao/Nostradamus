using Nostradamus.Networking;
using Nostradamus.Tests.Client;
using Nostradamus.Tests.Physics;
using Nostradamus.Tests.Server;
using System;
using System.Threading;

namespace Nostradamus.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Serializer.Initialize();

            //var t1 = new ServerSimulatorTest();
            //t1.TestSimpleScene();

            //var t2 = new ClientSimulatorTest();
            //t2.TestSimpleScene();
            //t2.TestSimpleSceneWithServer();

            //var t3 = new PhysicsSceneTest();
            //t3.TestSimplePhysicsScene();

            //ThreadPool.QueueUserWorkItem(s =>
            //{
            //    SimpleServerExample.Run();
            //});

            //ThreadPool.QueueUserWorkItem(s =>
            //{
            //    Thread.Sleep(3000);
            //    SimpleClientExample.Run();
            //});

            StepByStepExample.Run();

            Console.WriteLine("Press any key to stop...");
            Console.ReadLine();
        }
    }
}
