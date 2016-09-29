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
	class LoginRequest
	{
		[ProtoMember(1)]
		public ClientId ClientId { get; set; }
	}

	[ProtoContract]
	class LoginResponse
	{
		[ProtoMember(1)]
		public bool Success { get; set; }
	}
}
