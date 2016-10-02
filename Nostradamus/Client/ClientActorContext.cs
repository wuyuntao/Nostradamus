using System.Collections.Generic;

namespace Nostradamus.Client
{
	class ClientActorContext : ActorContext
	{
		private const float PredictivePriorityIncreaseSpeed = 0.1f;
		private const float PredictivePriorityDecreaseSpeed = 0.02f;

		private Timeline authoritativeTimeline;

		private Timeline predictiveTimeline;
		private Queue<IEventArgs> predictiveEventQueue = new Queue<IEventArgs>();

		public ClientActorContext(Actor actor)
			: base(actor)
		{
			authoritativeTimeline = new Timeline("Server");
			authoritativeTimeline.AddPoint(actor.Scene.Time, actor.CreateSnapshot());
		}

		public void CreateAuthoritativeTimepoint(IEnumerable<Event> events)
		{
			var currentSnapshot = Actor.Snapshot;

			Actor.RecoverSnapshot(authoritativeTimeline.Last.Snapshot);

			foreach (var e in events)
			{
				Actor.ApplyEvent(e.GetArgs());
			}

			authoritativeTimeline.AddPoint(Actor.Scene.Time + Actor.Scene.DeltaTime, Actor.Snapshot);

			Actor.RecoverSnapshot(currentSnapshot);
		}

		public bool IsSynchronized(int authoritativeTimelienTime, int predictiveTimelineTime)
		{
			if (predictiveTimeline == null)
				return true;

			var authoritativeSnapshot = authoritativeTimeline.InterpolatePoint(authoritativeTimelienTime).Snapshot;
			var predictiveSnapshot = predictiveTimeline.InterpolatePoint(predictiveTimelineTime).Snapshot;

			return authoritativeSnapshot.IsApproximate(predictiveSnapshot);
		}

		public bool Rollback(int authoritativeTime, int predictiveTime)
		{
			if (predictiveTimeline == null)
				return false;

			var snapshot = authoritativeTimeline.InterpolatePoint(authoritativeTime).Snapshot;

			predictiveTimeline = new Timeline(string.Format("Predictive-{0}", predictiveTime));
			predictiveTimeline.AddPoint(predictiveTime, snapshot.Clone());

			Actor.RecoverSnapshot(snapshot.Clone());

			return true;
		}

		public void EnqueuePredictiveCommand(Command command)
		{
			if (predictiveTimeline == null)
			{
				predictiveTimeline = new Timeline(string.Format("Predictive-{0}", command.Time));
			}

			EnqueueCommand(command);
		}

		public void CreatePredictiveTimepoint(int time, bool isReplay)
		{
			if (predictiveTimeline == null)
			{
				var point = authoritativeTimeline.InterpolatePoint(time);

				Actor.RecoverSnapshot(point.Snapshot.Clone());
			}
			else if (!isReplay && authoritativeTimeline.Last.Snapshot.IsApproximate(Actor.Snapshot))
			{
				var point = authoritativeTimeline.Last;

				Actor.RecoverSnapshot(point.Snapshot.Clone());

				predictiveTimeline = null;
			}
			else
			{
				predictiveTimeline.AddPoint(time, Actor.Snapshot.Clone());
			}
		}
	}
}
