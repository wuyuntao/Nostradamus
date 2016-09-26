using Nostradamus.Networking;
using Nostradamus.Tests.Actors;
using Nostradamus.Tests.Commnads;
using Nostradamus.Tests.Events;
using Nostradamus.Tests.Scenes;
using Nostradamus.Tests.Snapshots;
using NUnit.Framework;

namespace Nostradamus.Tests
{
	public class ActorTest
	{
		[Test]
		public void TestServerActor()
		{
			var scene = new SimpleScene();
			var sceneContext = new ServerSceneContext(scene);

			// Time: 0 - 20
			var serverFrame20 = sceneContext.Update(20);
			Assert.AreEqual(0, scene.Time);
			Assert.AreEqual(20, scene.DeltaTime);
			Assert.AreEqual(20, serverFrame20.Time);
			Assert.AreEqual(0, serverFrame20.Events.Length);

			var actorSnapshot20 = (ActorSnapshot)scene.Character.CreateSnapshot(20);
			Assert.AreEqual(0, actorSnapshot20.PositionX);
			Assert.AreEqual(0, actorSnapshot20.PositionY);

			var command = new Command(scene.Character.Id, 30, 1, new MoveActorCommand() { DeltaX = 0.1f, DeltaY = 0.1f });
			var clientFrame30 = new ClientSynchronizationFrame(1, 30, new[] { command });
			sceneContext.OnSynchronization(clientFrame30);

			// Time: 20 - 40
			var serverFrame40 = sceneContext.Update(20);
			Assert.AreEqual(20, scene.Time);
			Assert.AreEqual(20, scene.DeltaTime);
			Assert.AreEqual(40, serverFrame40.Time);
			Assert.AreEqual(1, serverFrame40.Events.Length);

			var actorSnapshot40 = (ActorSnapshot)scene.Character.CreateSnapshot(40);
			Assert.AreEqual(0.1f, actorSnapshot40.PositionX);
			Assert.AreEqual(0.1f, actorSnapshot40.PositionY);
		}

		[Test]
		public void TestClientActor()
		{
			var scene = new SimpleScene();
			var sceneContext = new ClientSceneContext(scene, 1);

			// Time: 0 - 20
			var clientFrame0 = sceneContext.Update(20);
			Assert.AreEqual(0, scene.Time);
			Assert.AreEqual(20, scene.DeltaTime);

			var actorSnapshot20 = (ActorSnapshot)scene.Character.CreateSnapshot(20);
			Assert.AreEqual(0, actorSnapshot20.PositionX);
			Assert.AreEqual(0, actorSnapshot20.PositionY);

			var @event = new Event(scene.Character.Id, 40, 1, new ActorMovedEvent() { PositionX = 1.1f, PositionY = 1.1f });
			var serverFrame40 = new ServerSynchronizationFrame(40, new[] { @event });
			sceneContext.OnSynchronization(serverFrame40);

			sceneContext.EnqueueCommand(scene.Character.Id, 30, new MoveActorCommand() { DeltaX = 0.1f, DeltaY = 0.1f });

			// Time: 20 - 40
			sceneContext.Update(20);
			Assert.AreEqual(scene.Time, 40);

			var actorSnapshot40 = (ActorSnapshot)scene.Character.CreateSnapshot(40);
			Assert.AreEqual(0.1f, actorSnapshot40.PositionX);
			Assert.AreEqual(0.1f, actorSnapshot40.PositionY);

			// Time: 40 - 60
			sceneContext.Update(20);
			Assert.AreEqual(scene.Time, 60);

			var actorSnapshot60 = (ActorSnapshot)scene.Character.CreateSnapshot(60);
			Assert.AreEqual(0.1f, actorSnapshot60.PositionX);
			Assert.AreEqual(0.1f, actorSnapshot60.PositionY);
		}
	}
}
