using Nostradamus.Tests.Commnads;
using Nostradamus.Tests.Snapshots;
using NUnit.Framework;

namespace Nostradamus.Tests
{
	public class SceneTest
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
			actor.AddAuthoritativePoint( 0, 0, new ActorSnapshot() { X = 1, Y = 1 } );
			actor.AddAuthoritativePoint( 50, 0, new ActorSnapshot() { X = 1, Y = 1 } );

			// --- Client: 40 / 20. Updated #2. Move command #1 sent
			actor.CreatePredictiveTimeline( 20, 0 );
			actor.AddPredictiveCommand( 20, new MoveActorCommand() { X = 1, Y = 0 } );
			actor.AddPredictivePoint( 40, new ActorSnapshot() { X = 1.2f, Y = 1 } );

			// +++ Server: 50 / 30. Updated #2.

			// +++ Server: 60 / 40. Move command #1 received

			// --- Client: 60 / 40. Updated #3. Move command #2 sent.
			actor.AddPredictiveCommand( 40, new MoveActorCommand() { X = 1, Y = 0 } );
			actor.AddPredictivePoint( 60, new ActorSnapshot() { X = 1.4f, Y = 1 } );

			// --- Client: 70 / 50. Update #2 received.
			actor.AddAuthoritativePoint( 100, 0, new ActorSnapshot() { X = 1, Y = 1 } );

			// +++ Server: 80 / 60. Move command #2 received.

			// --- Client: 80 / 60. Updated #4. Move command #3 sent.
			actor.AddPredictiveCommand( 60, new MoveActorCommand() { X = 1, Y = 0 } );
			actor.AddPredictivePoint( 80, new ActorSnapshot() { X = 1.6f, Y = 1 } );

			// +++ Server: 100 / 80. Move command #3 received. Updated #3

			// --- Client: 100 / 80. Updated #5. Move command #4 sent
			actor.AddPredictiveCommand( 80, new MoveActorCommand() { X = 1, Y = 0 } );
			actor.AddPredictivePoint( 100, new ActorSnapshot() { X = 1.8f, Y = 1 } );

			// +++ Server: 120 / 100. Move command #4 received.

			// --- Client: 120 / 100. Update #3 received. Move command #3 confirmed. Move command #5 sent
			actor.AddAuthoritativePoint( 150, 3, new ActorSnapshot() { X = 1.5f, Y = 1 } );

			actor.CreatePredictiveTimeline( 80, 150 );       // Create branch from last synchronized client timeline
			actor.AddPredictivePoint( 100, new ActorSnapshot() { X = 1.7f, Y = 1 } );  // Replay command #4
			actor.AddPredictivePoint( 120, new ActorSnapshot() { X = 1.9f, Y = 1 } );  // Replay command #5

			// +++ Server: 140 / 120. Move command #5 received

			// --- Client: 140 / 120. 
			actor.AddPredictivePoint( 140, new ActorSnapshot() { X = 1.9f, Y = 1 } );

			// +++ Server: 150 / 130. Updated #3

			// --- Client: 160 / 140. 
			actor.AddPredictivePoint( 160, new ActorSnapshot() { X = 1.9f, Y = 1 } );

			// --- Client: 170 / 150. Update #3 received
			actor.AddAuthoritativePoint( 200, 5, new ActorSnapshot() { X = 2, Y = 1 } );

			// --- Client: 180 / 160. 
			actor.CreatePredictiveTimeline( 120, 200 );

			actor.AddPredictivePoint( 140, new ActorSnapshot() { X = 2, Y = 1 } );
			actor.AddPredictivePoint( 160, new ActorSnapshot() { X = 2, Y = 1 } );
		}
	}
}
