using Nostradamus.Tests.Client;
using Nostradamus.Tests.Server;

namespace NostradamusExamples
{
	class Program
	{
		static void Main(string[] args)
		{
			//var t = new ServerSimulatorTest();
			//t.TestSimpleScene();

			var t = new ClientSimulatorTest();
			t.TestSimpleScene();
		}
	}
}
