using Nostradamus.Networking;
using Nostradamus.Tests.Actors;
using Nostradamus.Tests.Commnads;
using Nostradamus.Tests.Events;
using Nostradamus.Tests.Snapshots;
using NUnit.Framework;

namespace Nostradamus.Tests
{
	public class ActorTest
	{
		[Test]
		public void TestServerActor()
		{
			var sceneContext = new ServerSceneContext();
			var scene = new Scene( sceneContext );

			var actorContext = new ServerActorContext();
			var actorId = scene.CreateActorId( nameof( SimpleCharacter ) );
			var actorSnapshot0 = new ActorSnapshot();
			var actor = new SimpleCharacter( scene, actorId, 0, actorSnapshot0, actorContext );
			scene.AddActor( actor );

			actorContext.EnqueueCommand( 30, new MoveActorCommand() { DeltaX = 0.1f, DeltaY = 0.1f } );

			// Time: 0 - 20
			sceneContext.Update( 20 );
			Assert.AreEqual( scene.Time, 20 );

			var actorSnapshot20 = (ActorSnapshot)actor.CreateSnapshot( 20 );
			Assert.AreEqual( 0, actorSnapshot20.PositionX );
			Assert.AreEqual( 0, actorSnapshot20.PositionY );

			// Time: 20 - 40
			sceneContext.Update( 20 );

			Assert.AreEqual( scene.Time, 40 );
			var actorSnapshot40 = (ActorSnapshot)actor.CreateSnapshot( 40 );
			Assert.AreEqual( 0.1f, actorSnapshot40.PositionX );
			Assert.AreEqual( 0.1f, actorSnapshot40.PositionY );
		}

		[Test]
		public void TestClientActor()
		{
			var sceneContext = new ClientSceneContext();
			var scene = new Scene( sceneContext );

			var actorContext = new ClientActorContext();
			var actorId = new ActorId( 1 );
			var actorSnapshot0 = new ActorSnapshot();
			var actor = new SimpleCharacter( scene, actorId, 0, actorSnapshot0, actorContext );
			scene.AddActor( actor );

			actorContext.EnqueueAuthoritativeEvent( 40, 1, new ActorMovedEvent() { PositionX = 1.1f, PositionY = 1.1f } );
			actorContext.EnqueueCommand( 30, new MoveActorCommand() { DeltaX = 0.1f, DeltaY = 0.1f } );

			// Time: 0 - 20
			sceneContext.Update( 20 );
			Assert.AreEqual( scene.Time, 20 );

			var actorSnapshot20 = (ActorSnapshot)actor.CreateSnapshot( 20 );
			Assert.AreEqual( 0, actorSnapshot20.PositionX );
			Assert.AreEqual( 0, actorSnapshot20.PositionY );

			// Time: 20 - 40
			sceneContext.Update( 20 );
			Assert.AreEqual( scene.Time, 40 );

			var actorSnapshot40 = (ActorSnapshot)actor.CreateSnapshot( 40 );
			Assert.AreEqual( 0.1f, actorSnapshot40.PositionX );
			Assert.AreEqual( 0.1f, actorSnapshot40.PositionY );

			// Time: 40 - 60
			sceneContext.Update( 20 );
			Assert.AreEqual( scene.Time, 60 );

            var actorSnapshot60 = (ActorSnapshot)actor.CreateSnapshot( 60 );
			Assert.AreEqual( 0.1f, actorSnapshot60.PositionX );
			Assert.AreEqual( 0.1f, actorSnapshot60.PositionY );
		}
	}
}
