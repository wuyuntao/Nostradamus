using Nostradamus.Tests.Commnads;
using Nostradamus.Tests.Events;
using NUnit.Framework;

namespace Nostradamus.Tests
{
	public class ActorTest
	{
		class SimplePredictiveActor : PredictiveActor
		{
			public SimplePredictiveActor(Scene scene)
				: base( scene )
			{ }
		}

		[Test]
		public void TestServerClientSyncSimulation()
		{
			// Server tick freq: 20Hz. Client tick freq: 50Hz. Client latency: 20ms

			// +++ Server: 0. Updated #1

			// --- Client: 20 / 0. Update #1 received.
			var scene = new Scene();
			var actor = new SimplePredictiveActor( scene );
			actor.OnAuthoritativeEvent( 0, 0, new ActorSpawnedEvent() { X = 1, Y = 1 } );
			actor.OnAuthoritativeEvent( 50, 0, new ActorMovedEvent() { X = 1, Y = 1 } );

			// --- Client: 40 / 20. Updated #2. Move command #1 sent
			actor.OnPredictiveCommand( 20, new MoveActorCommand() { X = 1, Y = 0 } );
			actor.OnPredictiveEvent( 40, new ActorMovedEvent() { X = 1.2f, Y = 1 } );

			// +++ Server: 50 / 30. Updated #2.

			// +++ Server: 60 / 40. Move command #1 received

			// --- Client: 60 / 40. Updated #3. Move command #2 sent.
			actor.OnPredictiveCommand( 40, new MoveActorCommand() { X = 1, Y = 0 } );
			actor.OnPredictiveEvent( 60, new ActorMovedEvent() { X = 1.4f, Y = 1 } );

			// --- Client: 70 / 50. Update #2 received.
			actor.OnAuthoritativeEvent( 100, 0 );

			// +++ Server: 80 / 60. Move command #2 received.

			// --- Client: 80 / 60. Updated #4. Move command #3 sent.
			actor.OnPredictiveCommand( 60, new MoveActorCommand() { X = 1, Y = 0 } );
			actor.OnPredictiveEvent( 80, new ActorMovedEvent() { X = 1.6f, Y = 1 } );

			// +++ Server: 100 / 80. Move command #3 received. Updated #3

			// --- Client: 100 / 80. Updated #5. Move command #4 sent
			actor.OnPredictiveCommand( 80, new MoveActorCommand() { X = 1, Y = 0 } );
			actor.OnPredictiveEvent( 100, new ActorMovedEvent() { X = 1.8f, Y = 1 } );

			// +++ Server: 120 / 100. Move command #4 received.

			// --- Client: 120 / 100. Update #3 received. Move command #3 confirmed. Move command #5 sent
			actor.OnAuthoritativeEvent( 150, 3, new ActorMovedEvent() { X = 1.5f, Y = 1 } );
		}
	}
}
