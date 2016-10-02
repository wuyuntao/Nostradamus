using Nostradamus.Server;
using ProtoBuf;

namespace Nostradamus.Networking
{
	[ProtoContract]
	class MessageEnvelope
	{
		[ProtoMember(1)]
		public object Message;
	}

	[ProtoContract]
	class Login
	{
		[ProtoMember(1)]
		public ClientId ClientId { get; set; }
	}
}
