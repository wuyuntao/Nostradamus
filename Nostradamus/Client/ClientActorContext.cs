using NLog;
using System;
using System.Collections.Generic;

namespace Nostradamus.Client
{
    public sealed class ClientActorContext : ActorContext
    {
        private const float PredictivePriorityIncreaseSpeed = 0.1f;
        private const float PredictivePriorityDecreaseSpeed = 0.05f;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private Timeline predictiveTimeline;
        private float predictivePriority;

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

            if (predictivePriority < 1)
            {
                var lastPredictivePriority = predictivePriority;
                predictivePriority = Math.Min(1, predictivePriority + PredictivePriorityIncreaseSpeed);

                logger.Debug("Predictive priority of {0} increased {1} -> {2}", Actor, lastPredictivePriority, predictivePriority);
            }
        }

        public void CreatePredictiveTimepoint(int time, bool isReplay)
        {
            if (predictiveTimeline == null)
            {
                var point = Timeline.InterpolatePoint(time);

                Actor.Snapshot = point.Snapshot.Clone();
            }
            else if (!isReplay && (predictivePriority == 0 || Timeline.Last.Snapshot.IsApproximate(Actor.Snapshot)))
            {
                var point = Timeline.Last;

                Actor.Snapshot = point.Snapshot.Clone();

                predictiveTimeline = null;
            }
            else
            {
                var snapshot = Timeline.Last.Snapshot.Interpolate(Actor.Snapshot, predictivePriority);

                predictiveTimeline.AddPoint(time, snapshot);

                Actor.Snapshot = snapshot.Clone();
            }

            if (predictivePriority > 0)
            {
                var lastPredictivePriority = predictivePriority;
                predictivePriority = Math.Max(0, predictivePriority - PredictivePriorityDecreaseSpeed);

                logger.Debug("Predictive priority of {0} decreased {1} -> {2}", Actor, lastPredictivePriority, predictivePriority);
            }
        }
    }
}
