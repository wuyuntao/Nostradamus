using Nostradamus.Tests.Events;
using NUnit.Framework;

namespace Nostradamus.Tests
{
	public class WorldLineTest
	{
		[Test]
		public void TestServerClientSyncSimulation()
		{
			// Server tick freq: 20Hz. Client tick freq: 50Hz. Client latency: 20ms

			// +++ Server: 0. Updated #1

			// --- Client: 20 / 0. Update #1 received.
			var worldLine = new WorldLine();
			var server = worldLine.CreateBranch( "server" );

			var sn0 = server.CreateNode( 0 );
			sn0.AddEvent( new ActorSpawnedEvent() { X = 1, Y = 1 } );

			var sn50 = server.CreateNode( 50 );
			sn50.AddEvent( new ActorMovedEvent() { X = 1, Y = 1 } );

			// --- Client: 40 / 20. Updated #2. Move command #1 sent
			var client_c0 = worldLine.CreateBranch( "client-1", sn0 );
			var cn20 = client_c0.CreateNode( 20 );

			var cn40 = client_c0.CreateNode( 40 );
			cn40.AddEvent( new ActorMovedEvent() { X = 1.2f, Y = 1 } );

			// +++ Server: 50 / 30. Updated #2.

			// +++ Server: 60 / 40. Move command #1 received

			// --- Client: 60 / 40. Updated #3. Move command #2 sent.
			var cn60 = client_c0.CreateNode( 60 );
			cn60.AddEvent( new ActorMovedEvent() { X = 1.4f, Y = 1 } );

			// --- Client: 70 / 50. Update #2 received.
			var sn100 = server.CreateNode( 100 );
			sn100.AddEvent( new ActorMovedEvent() { X = 1, Y = 1 } );

			// +++ Server: 80 / 60. Move command #2 received.

			// --- Client: 80 / 60. Updated #4. Move command #3 sent.
			var cn80 = client_c0.CreateNode( 80 );
			cn40.AddEvent( new ActorMovedEvent() { X = 1.6f, Y = 1 } );

			// +++ Server: 100 / 80. Move command #3 received. Updated #3

			// --- Client: 100 / 80. Updated #5. Move command #4 sent
			var cn100 = client_c0.CreateNode( 100 );
			cn40.AddEvent( new ActorMovedEvent() { X = 1.8f, Y = 1 } );

			// +++ Server: 120 / 100. Move command #4 received.

			// --- Client: 120 / 100. Update #3 received. Move command #3 confirmed. Move command #5 sent
			var sn150 = server.CreateNode( 150 );
			sn150.AddEvent( new ActorMovedEvent() { X = 1.5f, Y = 1 } );

			var client_c3 = worldLine.CreateBranch( "client-3", cn20 );       // Create branch from last synchronized client branch
			var cn80_c3 = client_c3.CreateNode( 80 );
			cn80_c3.AddEvent( new ActorMovedEvent() { X = 1.5f, Y = 1 } );   // Set current server status to time after command #3 is used

			var cn100_c3 = client_c3.CreateNode( 100 );
			cn100_c3.AddEvent( new ActorMovedEvent() { X = 1.7f, Y = 1 } );  // Replay command #4

			var cn120_c3 = client_c3.CreateNode( 120 );
			cn120_c3.AddEvent( new ActorMovedEvent() { X = 1.9f, Y = 1 } );  // Replay command #5

			// +++ Server: 140 / 120. Move command #5 received

			// --- Client: 140 / 120. 
			var cn140_c3 = client_c3.CreateNode( 140 );

			// +++ Server: 150 / 130. Updated #3

			// --- Client: 160 / 140. 
			var cn160_c3 = client_c3.CreateNode( 160 );

			// --- Client: 170 / 150. Update #3 received
			var sn200 = server.CreateNode( 200 );
			sn200.AddEvent( new ActorMovedEvent() { X = 2f, Y = 1 } );

			// --- Client: 180 / 160. 
			var client_c5 = worldLine.CreateBranch( "client-5", cn80_c3 );
			var cn120_c5 = client_c5.CreateNode( 120 );
			cn120_c5.AddEvent( new ActorMovedEvent() { X = 2f, Y = 1f } );

			var cn140_c5 = client_c5.CreateNode( 140 );
			var cn160_c5 = client_c5.CreateNode( 160 );

			client_c5 = null;               // Reset client state since client's state is same as server's
		}
	}
}
