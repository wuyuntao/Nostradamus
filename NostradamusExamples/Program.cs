using Nostradamus.Tests.Client;
using Nostradamus.Tests.Physics;
using Nostradamus.Tests.Server;

namespace NostradamusExamples
{
	class Program
	{
		static void Main(string[] args)
		{
			//var t1 = new ServerSimulatorTest();
			//t1.TestSimpleScene();

			//var t2 = new ClientSimulatorTest();
			//t2.TestSimpleScene();
			//t2.TestSimpleSceneWithServer();

			var t3 = new PhysicsSceneTest();
			t3.TestSimplePhysicsScene();
		}
	}
}
