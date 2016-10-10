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

        private Timeline authoritativeTimeline;
        private Timeline predictiveTimeline;
        private float predictivePriority;

        public ClientActorContext(Actor actor, ISnapshotArgs snapshot)
            : base(actor, snapshot)
        {
            authoritativeTimeline = new Timeline();
            authoritativeTimeline.AddPoint(actor.Scene.Time, snapshot);
        }

        internal void ApplyAuthoritativeEvents(IEnumerable<IEventArgs> events)
        {
            ActorSnapshot = authoritativeTimeline.InterpolatePoint(Actor.Scene.Time).Snapshot;

            foreach (var e in events)
            {
                ApplyEvent(e);
            }

            authoritativeTimeline.AddPoint(Actor.Scene.Time + Actor.Scene.DeltaTime, Actor.Snapshot);
        }

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

            ActorSnapshot = snapshot.Clone();

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

                ActorSnapshot = point.Snapshot.Clone();
            }
            else if (!isReplay && (predictivePriority == 0 || Timeline.Last.Snapshot.IsApproximate(Actor.Snapshot)))
            {
                var point = Timeline.Last;

                ActorSnapshot = point.Snapshot.Clone();

                predictiveTimeline = null;
            }
            else
            {
                var snapshot = Timeline.Last.Snapshot.Interpolate(Actor.Snapshot, predictivePriority);

                predictiveTimeline.AddPoint(time, snapshot);

                ActorSnapshot = snapshot.Clone();
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
