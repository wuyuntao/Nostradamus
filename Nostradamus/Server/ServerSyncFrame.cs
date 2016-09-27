using System.Collections.Generic;

namespace Nostradamus.Server
{
	public sealed class ServerSyncFrame
	{
		public readonly int Time;
		public readonly int DeltaTime;
		public readonly List<Event> Events = new List<Event>();
		public readonly SortedList<ClientId, int> LastCommandSeqs = new SortedList<ClientId, int>();

		public ServerSyncFrame(int time, int deltaTime)
		{
			Time = time;
			DeltaTime = deltaTime;
		}

		public int GetLastCommandSeq(ClientId clientId)
		{
			int sequence;
			LastCommandSeqs.TryGetValue(clientId, out sequence);
			return sequence;
		}
	}
}
