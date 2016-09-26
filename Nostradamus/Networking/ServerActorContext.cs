using System;
using System.Collections.Generic;

namespace Nostradamus.Networking
{
	class ServerActorContext : ActorContext
	{
		private ServerSceneContext sceneContext;
		private Timeline authoritativeTimeline;
		private Queue<IEventArgs> eventQueue = new Queue<IEventArgs>();
		private int lastCommandSequence;
		private ISnapshotArgs snapshot;

		public ServerActorContext(ServerSceneContext sceneContext, Actor actor, int time, ISnapshotArgs snapshot)
			: base(sceneContext, actor, time, snapshot)
		{
			this.sceneContext = sceneContext;

			authoritativeTimeline = new Timeline("Authoritative");
			authoritativeTimeline.AddPoint(time, snapshot);
		}

		internal override ISnapshotArgs CreateSnapshot(int time)
		{
			var point = authoritativeTimeline.InterpolatePoint(time);
			if (point == null)
				throw new ArgumentException(string.Format("Cannot find snapshot at {0}", time));

			return point.Snapshot;
		}

		internal override void Update()
		{
			snapshot = authoritativeTimeline.Last.Snapshot;

			var timeAfterUpdate = Actor.Scene.Time + Actor.Scene.DeltaTime;

			foreach (var command in Actor.CommandQueue.DequeueBefore(timeAfterUpdate))
			{
				if (lastCommandSequence < command.Sequence)
					lastCommandSequence = command.Sequence;

				Actor.OnCommand(snapshot, command.Args);
			}

			Actor.OnUpdated();

			authoritativeTimeline.AddPoint(timeAfterUpdate, snapshot);

			snapshot = null;
		}

		internal override void ApplyEvent(IEventArgs @event)
		{
			if (snapshot == null)
				throw new InvalidOperationException("Cannot apply event");

			Actor.OnEvent(snapshot, @event);

			var e = new Event(Actor.Id, Actor.Scene.Time, lastCommandSequence, @event);
			sceneContext.EnqueueEvent(e);
		}
	}
}