using System;
using System.Collections.Generic;

namespace Nostradamus
{
    public sealed class Timeline
    {
        private readonly LinkedList<Timepoint> points = new LinkedList<Timepoint>();

        public Timepoint AddPoint(int time, ISnapshotArgs snapshot)
        {
            if (points.Last != null)
            {
                if (time <= points.Last.Value.Time)
                    throw new ArgumentException(string.Format("'time' must > {0}", points.Last.Value.Time));
            }

            var point = new Timepoint(this, time, snapshot);

            points.AddLast(point);

            return point;
        }

        public Timepoint InterpolatePoint(int time)
        {
            Timepoint previous = null;
            Timepoint next = null;

            for (var point = points.First; point != null; point = point.Next)
            {
                if (point.Value.Time < time)
                {
                    previous = point.Value;
                }
                else if (point.Value.Time == time)
                {
                    previous = next = point.Value;
                    break;
                }
                else
                {
                    next = point.Value;
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

        internal Timepoint FindPoint(int time)
        {
            for (var point = points.First; point != null; point = point.Next)
            {
                if (point.Value.Time == time)
                    return point.Value;
            }

            return null;
        }

        public Timepoint FindBefore(int time)
        {
            for (var node = points.Last; node != null; node = node.Previous)
            {
                if (node.Value.Time <= time)
                    return node.Value;
            }

            return null;
        }

        public Timepoint FindAfter(int time)
        {
            for (var point = points.First; point != null; point = point.Next)
            {
                if (point.Value.Time > time)
                    return point.Value;
            }

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
