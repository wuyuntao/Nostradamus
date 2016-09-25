using Nostradamus.Tests;

namespace NostradamusExamples
{
	class Program
	{
		static void Main(string[] args)
		{
			var t = new ActorTest();
			t.TestServerActor();
			t.TestClientActor();
		}
	}
}
