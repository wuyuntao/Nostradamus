using System.Collections.Generic;

namespace Nostradamus.Server
{
	public sealed class ServerSynchronizationFrame
	{
		public readonly int Time;
		public readonly int DeltaTime;
		public readonly List<Event> Events = new List<Event>();
		public readonly SortedList<ClientId, int> LastCommandSequences = new SortedList<ClientId, int>();

		public ServerSynchronizationFrame(int time, int deltaTime)
		{
			Time = time;
			DeltaTime = deltaTime;
		}

		public int GetLastCommandSequence(ClientId clientId)
		{
			int sequence;
			LastCommandSequences.TryGetValue(clientId, out sequence);
			return sequence;
		}
	}
}
