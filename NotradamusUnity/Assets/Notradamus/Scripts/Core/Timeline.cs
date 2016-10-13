using System;
using System.Collections.Generic;

namespace Nostradamus
{
    sealed class Timeline
    {
        private LinkedList<Timepoint> points = new LinkedList<Timepoint>();

        public Timepoint AddPoint(int time, ISnapshotArgs snapshot)
        {
            if (points.Last != null)
            {
                if (time < points.Last.Value.Time)
                    throw new ArgumentException(string.Format("'time' must > {0}", points.Last.Value.Time));

                if (time == points.Last.Value.Time)
                {
                    points.Last.Value.Snapshot = snapshot;

                    return points.Last.Value;
                }
            }

            var point = new Timepoint(this, time, snapshot);

            points.AddLast(point);

            return point;
        }

        public Timepoint InterpolatePoint(int time)
        {
            Timepoint previous = null;
            Timepoint next = null;

            // Search in reversed order for better performance
            for (var point = points.Last; point != null; point = point.Previous)
            {
                if (point.Value.Time > time)
                {
                    next = point.Value;
                }
                else if (point.Value.Time == time)
                {
                    previous = next = point.Value;
                    break;
                }
                else
                {
                    previous = point.Value;
                    break;
                }
            }

            ISnapshotArgs snapshot;
            if (previous != null)
            {
                if (next != null)
                {
                    if (previous == next)
                    {
                        snapshot = previous.Snapshot.Clone();
                    }
                    else
                    {
                        var deltaTime = time - previous.Time;
                        var totalTime = next.Time - previous.Time;
                        var factor = 1f * deltaTime / totalTime;

                        snapshot = previous.Snapshot.Interpolate(next.Snapshot, factor);
                    }
                }
                else
                {
                    var deltaTime = time - previous.Time;

                    snapshot = previous.Snapshot.Extrapolate(deltaTime);
                }
            }
            else
            {
                if (next != null)
                {
                    var deltaTime = time - next.Time;

                    snapshot = next.Snapshot.Extrapolate(deltaTime);
                }
                else
                {
                    return null;
                }
            }

            if (snapshot != null)
                return new Timepoint(this, time, snapshot);
            else
                return null;
        }

        public Timepoint First
        {
            get { return points.First != null ? points.First.Value : null; }
        }

        public Timepoint Last
        {
            get { return points.Last != null ? points.Last.Value : null; }
        }
    }
}
