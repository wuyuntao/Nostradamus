using Nostradamus.Tests.Snapshots;
using NUnit.Framework;

namespace Nostradamus.Tests
{
	/*
	public class TimelineTest
	{
		[Test]
		public void TestServerClientSyncSimulation()
		{
			// Server tick freq: 20Hz. Client tick freq: 50Hz. Client latency: 20ms

			// +++ Server: 0. Updated #1

			// --- Client: 20 / 0. Update #1 received.
			var server = new Timeline( "server" );

			var sn0 = server.AddPoint( 0, new ActorSnapshot() { PositionX = 1, PositionY = 1 } );
			var sn50 = server.AddPoint( 50, new ActorSnapshot() { PositionX = 1, PositionY = 1 } );

			var client_c0 = new Timeline( "client-0" );
			var cn0 = client_c0.AddPoint( 0, new ActorSnapshot() { PositionX = 1, PositionY = 1 } );

			// --- Client: 40 / 20. Updated #2. Move command #1 sent
			var cn20 = client_c0.AddPoint( 20, new ActorSnapshot() { PositionX = 1, PositionY = 1 } );

			var cn40 = client_c0.AddPoint( 40, new ActorSnapshot() { PositionX = 1.2f, PositionY = 1 } );

			// +++ Server: 50 / 30. Updated #2.

			// +++ Server: 60 / 40. Move command #1 received

			// --- Client: 60 / 40. Updated #3. Move command #2 sent.
			var cn60 = client_c0.AddPoint( 60, new ActorSnapshot() { PositionX = 1.4f, PositionY = 1 } );

			// --- Client: 70 / 50. Update #2 received.
			var sn100 = server.AddPoint( 100, new ActorSnapshot() { PositionX = 1, PositionY = 1 } );

			// +++ Server: 80 / 60. Move command #2 received.

			// --- Client: 80 / 60. Updated #4. Move command #3 sent.
			var cn80 = client_c0.AddPoint( 80, new ActorSnapshot() { PositionX = 1.6f, PositionY = 1 } );

			// +++ Server: 100 / 80. Move command #3 received. Updated #3

			// --- Client: 100 / 80. Updated #5. Move command #4 sent
			var cn100 = client_c0.AddPoint( 100, new ActorSnapshot() { PositionX = 1.8f, PositionY = 1 } );

			// +++ Server: 120 / 100. Move command #4 received.

			// --- Client: 120 / 100. Update #3 received. Move command #3 confirmed. Move command #5 sent
			var sn150 = server.AddPoint( 150, new ActorSnapshot() { PositionX = 1.5f, PositionY = 1 } );

			var client_c3 = new Timeline( "client-3" );       // Create branch from last synchronized client branch
			var cn80_c3 = client_c3.AddPoint( 80, new ActorSnapshot() { PositionX = 1.5f, PositionY = 1 } );

			var cn100_c3 = client_c3.AddPoint( 100, new ActorSnapshot() { PositionX = 1.7f, PositionY = 1 } );  // Replay command #4

			var cn120_c3 = client_c3.AddPoint( 120, new ActorSnapshot() { PositionX = 1.9f, PositionY = 1 } );  // Replay command #5

			// +++ Server: 140 / 120. Move command #5 received

			// --- Client: 140 / 120. 
			var cn140_c3 = client_c3.AddPoint( 140, new ActorSnapshot() { PositionX = 1.9f, PositionY = 1 } );

			// +++ Server: 150 / 130. Updated #3

			// --- Client: 160 / 140. 
			var cn160_c3 = client_c3.AddPoint( 160, new ActorSnapshot() { PositionX = 1.9f, PositionY = 1 } );

			// --- Client: 170 / 150. Update #3 received
			var sn200 = server.AddPoint( 200, new ActorSnapshot() { PositionX = 2f, PositionY = 1 } );

			// --- Client: 180 / 160. 
			var client_c5 = new Timeline( "client-5" );
			var cn120_c5 = client_c5.AddPoint( 120, new ActorSnapshot() { PositionX = 2f, PositionY = 1 } );

			var cn140_c5 = client_c5.AddPoint( 140, new ActorSnapshot() { PositionX = 2f, PositionY = 1 } );
			var cn160_c5 = client_c5.AddPoint( 160, new ActorSnapshot() { PositionX = 2f, PositionY = 1 } );

			// Reset client state since client's state is same as server's
		}
	}
	*/
}
