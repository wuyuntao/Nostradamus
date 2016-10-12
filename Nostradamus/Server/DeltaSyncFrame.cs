using System.Collections.Generic;

namespace Nostradamus.Server
{
    public sealed class DeltaSyncFrame
    {
        public readonly int Time;

        public readonly int DeltaTime;

        public readonly List<Event> Events;

        public readonly SortedList<ClientId, int> LastCommandSeqs;

        public DeltaSyncFrame(int time, int deltaTime)
        {
            Time = time;
            DeltaTime = deltaTime;
            Events = new List<Event>();
            LastCommandSeqs = new SortedList<ClientId, int>();
        }
    }
}