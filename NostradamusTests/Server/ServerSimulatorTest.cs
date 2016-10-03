using Nostradamus.Client;
using Nostradamus.Examples;
using Nostradamus.Server;
using NUnit.Framework;

namespace Nostradamus.Tests.Server
{
	public class ServerSimulatorTest
	{
		[Test]
		public void TestSimpleScene()
		{
			var simulator = new ServerSimulator();
			var scene = new SimpleScene(simulator);

			// Time: 0 - 20
			simulator.Simulate(20);
			var frame0 = simulator.FetchDeltaSyncFrame();
			Assert.AreEqual(0, scene.Time);
			Assert.AreEqual(20, scene.DeltaTime);
			Assert.AreEqual(0, frame0.Time);
			Assert.AreEqual(20, frame0.DeltaTime);
			Assert.AreEqual(0, frame0.Events.Count);

			var snapshot20 = (CharacterSnapshot)scene.Character.Snapshot;
			Assert.AreEqual(0, snapshot20.PositionX);
			Assert.AreEqual(0, snapshot20.PositionY);

			var commandArgs = new MoveCharacterCommand() { DeltaX = 1, DeltaY = 1 };
			var clientId = new ClientId(1);
			var command = new Command(clientId, scene.Character.Id, 1, 20, 20, commandArgs);
			var clientFrame = new CommandFrame(clientId);
			clientFrame.Commands.Add(command);
			simulator.ReceiveCommandFrame(clientFrame);

			// Time: 20 - 40
			simulator.Simulate(20);
			var frame20 = simulator.FetchDeltaSyncFrame();
			Assert.AreEqual(20, scene.Time);
			Assert.AreEqual(20, scene.DeltaTime);
			Assert.AreEqual(20, frame20.Time);
			Assert.AreEqual(20, frame20.DeltaTime);
			Assert.AreEqual(1, frame20.Events.Count);

			var snapshot40 = (CharacterSnapshot)scene.Character.Snapshot;
			Assert.AreEqual(0.02f, snapshot40.PositionX);
			Assert.AreEqual(0.02f, snapshot40.PositionY);
		}
	}
}
