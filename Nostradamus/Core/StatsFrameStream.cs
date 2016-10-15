using System;
using System.Collections.Generic;
using System.Linq;

namespace Nostradamus
{
    public sealed class StatsFrameStream
    {
        private LinkedList<StatsFrame> Frames = new LinkedList<StatsFrame>();

        internal StatsFrameStream()
        {
            CreateNextFrame();
        }

        public void CreateNextFrame()
        {
            var frame = new StatsFrame(this);
            var node = Frames.AddLast(frame);

            frame.SetNode(node);
        }

        public string ToCsv()
        {
            return string.Join(Environment.NewLine,
                    (from f in Frames
                     select f.ToCsv()).ToArray());
        }

        public StatsFrame Current
        {
            get { return Frames.Last.Value; }
        }
    }

    public sealed class StatsFrame
    {
        private StatsFrameStream stream;
        private LinkedListNode<StatsFrame> node;

        private List<Stats> stats = new List<Stats>();

        internal StatsFrame(StatsFrameStream stream)
        {
            this.stream = stream;
        }

        internal void SetNode(LinkedListNode<StatsFrame> node)
        {
            this.node = node;
        }

        public T GetStats<T>()
            where T : Stats
        {
            return stats.Find(s => s is T) as T;
        }

        public T GetOrAddStats<T>()
            where T : Stats, new()
        {
            var stats = GetStats<T>();
            if (stats == null)
                AddStats<T>();

            return stats;
        }

        public T GetOrAddStats<T>(Func<T> createStats)
            where T : Stats, new()
        {
            var stats = GetStats<T>();
            if (stats == null)
            {
                stats = createStats();
                AddStats(stats);
            }

            return stats;
        }

        public T AddStats<T>()
            where T : Stats, new()
        {
            var stats = new T();

            this.stats.Add(stats);

            return stats;
        }

        public void AddStats(Stats stats)
        {
            this.stats.Add(stats);
        }

        public StatsFrame FindPrevious(Func<StatsFrame, bool> predicate)
        {
            for (var node = this.node.Previous; node != null; node = node.Previous)
            {
                if (predicate(node.Value))
                    return node.Value;
            }

            return null;
        }

        public StatsFrame FindNext(Func<StatsFrame, bool> predicate)
        {
            for (var node = this.node.Next; node != null; node = node.Next)
            {
                if (predicate(node.Value))
                    return node.Value;
            }

            return null;
        }

        public string ToCsv()
        {
            return string.Join(",",
                    (from s in stats
                     select s.ToCsv()).ToArray());
        }
    }

    public abstract class Stats
    {
        public abstract string ToCsv();
    }
}
