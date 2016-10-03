using System.Collections.Generic;

namespace Nostradamus.Server
{
	public sealed class DeltaSyncFrame
	{
		public int Time { get; set; }

		public int DeltaTime { get; set; }

		public List<Event> Events { get; set; }

		public SortedList<ClientId, int> LastCommandSeqs { get; set; }

		public DeltaSyncFrame(int time, int deltaTime)
		{
			Time = time;
			DeltaTime = deltaTime;
			Events = new List<Event>();
			LastCommandSeqs = new SortedList<ClientId, int>();
		}
	}
}