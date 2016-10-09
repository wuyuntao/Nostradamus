using System.Collections.Generic;

namespace Nostradamus.Client
{
    class ClientActorContext : ActorContext
    {
        private const float PredictivePriorityIncreaseSpeed = 0.1f;
        private const float PredictivePriorityDecreaseSpeed = 0.02f;

        private Timeline predictiveTimeline;
        private Queue<IEventArgs> predictiveEventQueue = new Queue<IEventArgs>();

        public ClientActorContext(Actor actor, ISnapshotArgs snapshot)
            : base(actor, snapshot)
        { }

        public override ISnapshotArgs InterpolateSnapshot(int time)
        {
            if (predictiveTimeline != null)
            {
                var timepoint = predictiveTimeline.InterpolatePoint(time);
                if (timepoint != null)
                    return timepoint.Snapshot;
            }

            return base.InterpolateSnapshot(time);
        }

        public void CreateAuthoritativeTimepoint(IEnumerable<Event> events)
        {
            var currentSnapshot = Actor.Snapshot;

            Actor.Snapshot = Timeline.Last.Snapshot.Clone();

            foreach (var e in events)
            {
                Actor.ApplyEvent(e.Args);
            }

            Timeline.AddPoint(Actor.Scene.Time + Actor.Scene.DeltaTime, Actor.Snapshot);

            Actor.Snapshot = currentSnapshot;
        }

        public bool IsSynchronized(int authoritativeTimelienTime, int predictiveTimelineTime)
        {
            if (predictiveTimeline == null)
                return true;

            var authoritativeSnapshot = Timeline.InterpolatePoint(authoritativeTimelienTime).Snapshot;
            var predictiveSnapshot = predictiveTimeline.InterpolatePoint(predictiveTimelineTime).Snapshot;

            return authoritativeSnapshot.IsApproximate(predictiveSnapshot);
        }

        public bool Rollback(int authoritativeTime, int predictiveTime)
        {
            if (predictiveTimeline == null)
                return false;

            var snapshot = Timeline.InterpolatePoint(authoritativeTime).Snapshot;

            predictiveTimeline = new Timeline();
            predictiveTimeline.AddPoint(predictiveTime, snapshot.Clone());

            Actor.Snapshot = snapshot.Clone();

            return true;
        }

        public void EnqueuePredictiveCommand(Command command)
        {
            if (predictiveTimeline == null)
            {
                predictiveTimeline = new Timeline();
            }

            EnqueueCommand(command);
        }

        public void CreatePredictiveTimepoint(int time, bool isReplay)
        {
            if (predictiveTimeline == null)
            {
                var point = Timeline.InterpolatePoint(time);

                Actor.Snapshot = point.Snapshot.Clone();
            }
            else if (!isReplay && Timeline.Last.Snapshot.IsApproximate(Actor.Snapshot))
            {
                var point = Timeline.Last;

                Actor.Snapshot = point.Snapshot.Clone();

                predictiveTimeline = null;
            }
            else
            {
                predictiveTimeline.AddPoint(time, Actor.Snapshot.Clone());
            }
        }
    }
}
