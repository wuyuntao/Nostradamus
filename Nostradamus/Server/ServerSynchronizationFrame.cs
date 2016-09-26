using System.Collections.Generic;

namespace Nostradamus.Server
{
	public sealed class ServerSynchronizationFrame
	{
		public readonly int Time;
		public readonly int DeltaTime;
		public readonly List<Event> Events = new List<Event>();

		public ServerSynchronizationFrame(int time, int deltaTime, IEnumerable<Event> events = null)
		{
			Time = time;
			DeltaTime = deltaTime;

			if (events != null)
				Events.AddRange(events);
		}
	}
}
