using ProtoBuf;
using System.Collections.Generic;

namespace Nostradamus.Server
{
	[ProtoContract]
	public sealed class ServerSyncFrame
	{
		[ProtoMember(1)]
		public int Time { get; set; }

		[ProtoMember(2)]
		public int DeltaTime { get; set; }

		[ProtoMember(3, IsRequired = false)]
		public List<Event> Events { get; set; }

		[ProtoMember(4, IsRequired = false)]
		public SortedList<ClientId, int> LastCommandSeqs { get; set; }

		public ServerSyncFrame(int time, int deltaTime)
		{
			Time = time;
			DeltaTime = deltaTime;
			Events = new List<Event>();
			LastCommandSeqs = new SortedList<ClientId, int>();
		}

		public ServerSyncFrame() { }

		public int GetLastCommandSeq(ClientId clientId)
		{
			int sequence;
			LastCommandSeqs.TryGetValue(clientId, out sequence);
			return sequence;
		}
	}
}
