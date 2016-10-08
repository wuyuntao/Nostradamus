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
            authoritativeTimeline = new Timeline();
            authoritativeTimeline.AddPoint(actor.Scene.Time, actor.Snapshot.Clone());
        }

        public void CreateAuthoritativeTimepoint(IEnumerable<Event> events)
        {
            var currentSnapshot = Actor.Snapshot;

            Actor.Snapshot = authoritativeTimeline.Last.Snapshot.Clone();

            foreach (var e in events)
            {
                Actor.ApplyEvent(e.Args);
            }

            authoritativeTimeline.AddPoint(Actor.Scene.Time + Actor.Scene.DeltaTime, Actor.Snapshot);

            Actor.Snapshot = currentSnapshot;
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
                var point = authoritativeTimeline.InterpolatePoint(time);

                Actor.Snapshot = point.Snapshot.Clone();
            }
            else if (!isReplay && authoritativeTimeline.Last.Snapshot.IsApproximate(Actor.Snapshot))
            {
                var point = authoritativeTimeline.Last;

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
