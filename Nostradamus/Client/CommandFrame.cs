using ProtoBuf;
using System.Collections.Generic;

namespace Nostradamus.Client
{
	[ProtoContract]
	public sealed class CommandFrame
	{
		[ProtoMember(1)]
		public ClientId ClientId { get; set; }

		[ProtoMember(2)]
		public List<Command> Commands { get; set; }

		public CommandFrame(ClientId clientId)
		{
			ClientId = clientId;
			Commands = new List<Command>();
		}

		public CommandFrame() { }
	}
}