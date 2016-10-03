using Nostradamus.Server;
using ProtoBuf;

namespace Nostradamus.Networking
{
	[ProtoContract]
	public class MessageEnvelope
	{
		[ProtoMember(1, DynamicType = true)]
		public object Message;
	}

	[ProtoContract]
	public class Login
	{
		[ProtoMember(1)]
		public ClientId ClientId { get; set; }
	}
}
