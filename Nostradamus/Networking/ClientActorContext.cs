using System;
using System.Collections.Generic;

namespace Nostradamus.Networking
{
	class ClientActorContext : ActorContext
	{
		private const float PredictivePriorityIncreaseSpeed = 0.1f;
		private const float PredictivePriorityDecreaseSpeed = 0.02f;

		private ClientSceneContext sceneContext;
		private Timeline authoritativeTimeline;
		private Queue<Event> authoritativeEventQueue = new Queue<Event>();
		private Timeline predictiveTimeline;
		private Queue<IEventArgs> predictiveEventQueue = new Queue<IEventArgs>();
		private float predictivePriority;

		public ClientActorContext(ClientSceneContext sceneContext, Actor actor, int time, ISnapshotArgs snapshot)
			: base(sceneContext, actor, time, snapshot)
		{
			this.sceneContext = sceneContext;

			authoritativeTimeline = new Timeline("Authoritative");
			authoritativeTimeline.AddPoint(time, snapshot);
		}

		internal override ISnapshotArgs CreateSnapshot(int time)
		{
			var authoritativePoint = authoritativeTimeline.InterpolatePoint(time);
			if (authoritativePoint == null)
				throw new ArgumentException(string.Format("Cannot find authoritative snapshot at {0}", time));

			if (predictiveTimeline == null)
				return authoritativePoint.Snapshot;

			var predictivePoint = predictiveTimeline.InterpolatePoint(time);
			if (predictivePoint == null)
				throw new ArgumentException(string.Format("Cannot find predictive snapshot at {0}", time));

			return authoritativePoint.Snapshot.Interpolate(predictivePoint.Snapshot, predictivePriority);
		}

		internal void EnqueueAuthoritativeEvent(Event e)
		{
			authoritativeEventQueue.Enqueue(e);
		}

		internal override void ApplyEvent(IEventArgs @event)
		{
			predictiveEventQueue.Enqueue(@event);
		}

		internal override void Update()
		{
			var timeAfterUpdate = Actor.Scene.Time + Actor.Scene.DeltaTime;

			ISnapshotArgs snapshot = null;
			foreach (var command in Actor.CommandQueue.DequeueBefore(timeAfterUpdate))
			{
				if (predictiveTimeline == null)
				{
					var authoritativePoint = authoritativeTimeline.InterpolatePoint(Actor.Scene.Time);
					if (authoritativePoint == null)
						throw new InvalidOperationException(string.Format("Cannot find authoritative snapshot at {0}", timeAfterUpdate));

					predictiveTimeline = new Timeline(string.Format("Predictive-{0}", command.Sequence));
					predictiveTimeline.AddPoint(Actor.Scene.Time, authoritativePoint.Snapshot);

					snapshot = authoritativePoint.Snapshot;
				}
				else if (snapshot == null)
				{
					snapshot = predictiveTimeline.Last.Snapshot.Clone();
				}

				Actor.OnCommand(snapshot, command.Args);
				predictivePriority += PredictivePriorityIncreaseSpeed;

				while (predictiveEventQueue.Count > 0)
				{
					Actor.OnEvent(snapshot, predictiveEventQueue.Dequeue());
				}
			}

			Actor.OnUpdated();

			if (snapshot != null)
			{
				predictiveTimeline.AddPoint(timeAfterUpdate, snapshot);
			}

			predictivePriority -= PredictivePriorityDecreaseSpeed;

			if (predictivePriority <= 0)
			{
				predictiveTimeline = null;
				predictivePriority = 0;
			}
		}

		internal ISnapshotArgs ApplyAuthoritativeEvents(int time)
		{
			var lastTimepoint = authoritativeTimeline.Last;

			ISnapshotArgs snapshot;
			if (authoritativeEventQueue.Count > 0)
			{
				snapshot = lastTimepoint.Snapshot.Clone();
				var lastCommandSequence = -1;

				while (authoritativeEventQueue.Count > 0)
				{
					var e = authoritativeEventQueue.Dequeue();

					if (lastCommandSequence < e.LastCommandSequence)
						lastCommandSequence = e.LastCommandSequence;

					Actor.OnEvent(snapshot, e.Args);
				}
			}
			else
			{
				snapshot = lastTimepoint.Snapshot.Extrapolate(time - lastTimepoint.Time);
			}

			authoritativeTimeline.AddPoint(time, snapshot);

			return snapshot;
		}
	}
}
