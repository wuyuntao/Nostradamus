using ProtoBuf;
using System.Collections.Generic;

namespace Nostradamus.Client
{
	[ProtoContract]
	public sealed class ClientSyncFrame
	{
		[ProtoMember(1)]
		public ClientId ClientId { get; set; }

		[ProtoMember(2)]
		public int Time { get; set; }

		[ProtoMember(3)]
		public List<Command> Commands { get; set; }

		public ClientSyncFrame(ClientId clientId, int time)
		{
			ClientId = clientId;
			Time = time;
			Commands = new List<Command>();
		}

		public ClientSyncFrame() { }
	}
}
