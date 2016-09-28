using System.Collections.Generic;

namespace Nostradamus.Client
{
	class ActorContext
	{
		private const float PredictivePriorityIncreaseSpeed = 0.1f;
		private const float PredictivePriorityDecreaseSpeed = 0.02f;

		private Actor actor;

		private Timeline authoritativeTimeline;
		private Queue<Event> authoritativeEventQueue = new Queue<Event>();

		private Timeline predictiveTimeline;
		private Queue<IEventArgs> predictiveEventQueue = new Queue<IEventArgs>();

		public ActorContext(Actor actor, int time, ISnapshotArgs snapshot)
		{
			this.actor = actor;

			authoritativeTimeline = new Timeline("Server");
			authoritativeTimeline.AddPoint(time, snapshot);
		}

		public void AddAuthoritativeEvent(Event @event)
		{
			authoritativeEventQueue.Enqueue(@event);
		}

		public void AddAuthoritativeTimepoint()
		{
			var currentSnapshot = actor.Snapshot;

			actor.SetSnapshot(authoritativeTimeline.Last.Snapshot);

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

			actor.SetSnapshot(currentSnapshot);
		}

		public bool IsSynchronized(int time)
		{
			if (predictiveTimeline == null)
				return true;

			var authoritativeSnapshot = authoritativeTimeline.Last.Snapshot;
			var predictiveSnapshot = predictiveTimeline.InterpolatePoint(time).Snapshot;

			return authoritativeSnapshot.IsApproximate(predictiveSnapshot);
		}

		public bool Rewind(int time)
		{
			if (predictiveTimeline == null)
				return false;

			var snapshot = authoritativeTimeline.Last.Snapshot;
			predictiveTimeline = new Timeline(string.Format("Predictive-{0}", time));
			predictiveTimeline.AddPoint(time, snapshot.Clone());

			actor.SetSnapshot(snapshot.Clone());
			return true;
		}

		public void AddPredictiveTimepoint(int time, bool isReplay)
		{
			if (predictiveTimeline == null)
			{
				var point = authoritativeTimeline.InterpolatePoint(time);

				actor.SetSnapshot(point.Snapshot.Clone());
			}
			else if (!isReplay && authoritativeTimeline.Last.Snapshot.IsApproximate(actor.Snapshot))
			{
				var point = authoritativeTimeline.Last;

				actor.SetSnapshot(point.Snapshot.Clone());

				predictiveTimeline = null;
			}
			else
			{
				predictiveTimeline.AddPoint(time, actor.Snapshot);
			}
		}

		public void AddPredictiveCommand(Command command)
		{
			if (predictiveTimeline == null)
			{
				predictiveTimeline = new Timeline(string.Format("Predictive-{0}-{1}", command.Sequence, command.Time));
			}

			actor.OnCommandReceived(command.Args);
		}
	}
}
