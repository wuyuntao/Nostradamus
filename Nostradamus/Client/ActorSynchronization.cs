using System;
using System.Collections.Generic;

namespace Nostradamus.Client
{
	class ActorSynchronization
	{
		private const float PredictivePriorityIncreaseSpeed = 0.1f;
		private const float PredictivePriorityDecreaseSpeed = 0.02f;

		private Actor actor;

		private Timeline authoritativeTimeline;
		private Queue<Event> authoritativeEventQueue = new Queue<Event>();

		private Timeline predictiveTimeline;
		private Queue<IEventArgs> predictiveEventQueue = new Queue<IEventArgs>();
		private float predictivePriority;

		public ActorSynchronization(Actor actor, int time, ISnapshotArgs snapshot)
		{
			this.actor = actor;

			authoritativeTimeline = new Timeline("Server");
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

		public void AddAuthoritativeEvent(Event @event)
		{
			authoritativeEventQueue.Enqueue(@event);
		}

		public void AddAuthoritativeTimepoint()
		{
			var currentSnapshot = actor.Snapshot;

			actor.RollbackSnapshot(authoritativeTimeline.Last.Snapshot);

			if (authoritativeEventQueue.Count > 0)
			{
				while (authoritativeEventQueue.Count > 0)
				{
					var @event = authoritativeEventQueue.Dequeue();

					actor.ApplyEvent(@event.Args);
				}

				authoritativeTimeline.AddPoint(actor.Scene.Time + actor.Scene.DeltaTime, actor.Snapshot);
			}
			else
			{
				authoritativeTimeline.AddPoint(actor.Scene.Time + actor.Scene.DeltaTime, actor.Snapshot.Extrapolate(actor.Scene.DeltaTime));
			}

			actor.RollbackSnapshot(currentSnapshot);
		}

		public bool CheckReconciliation()
		{
			throw new NotImplementedException();
		}

		internal void RollbackPredictiveSnapshot(int lastCommandTime)
		{
			throw new NotImplementedException();
		}
	}
}
