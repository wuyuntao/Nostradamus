using System;
using System.Collections.Generic;

namespace Nostradamus.Networking
{
	class ServerActorContext : ActorContext
	{
		private Timeline authoritativeTimeline;
		private Queue<IEventArgs> eventQueue = new Queue<IEventArgs>();

		internal override void Initialize(Actor actor, int time, ISnapshotArgs snapshot)
		{
			base.Initialize( actor, time, snapshot );

			authoritativeTimeline = new Timeline( "Authoritative" );
			authoritativeTimeline.AddPoint( time, snapshot );
		}

		internal override ISnapshotArgs CreateSnapshot(int time)
		{
			var point = authoritativeTimeline.InterpolatePoint( time );
			if( point == null )
				throw new ArgumentException( string.Format( "Cannot find snapshot at {0}", time ) );

			return point.Snapshot;
		}

		internal override void Update()
		{
			var snapshot = authoritativeTimeline.Last.Snapshot;

			var timeAfterUpdate = Actor.Scene.Time + Actor.Scene.DeltaTime;

			foreach( var command in Actor.CommandQueue.DequeueBefore( timeAfterUpdate ) )
			{
				Actor.OnCommand( snapshot, command.Args );

				while( eventQueue.Count > 0 )
				{
					Actor.OnEvent( snapshot, eventQueue.Dequeue() );
				}
			}

			Actor.OnUpdate();

			authoritativeTimeline.AddPoint( timeAfterUpdate, snapshot );
		}

		internal override void ApplyEvent(IEventArgs @event)
		{
			eventQueue.Enqueue( @event );
		}
	}
}