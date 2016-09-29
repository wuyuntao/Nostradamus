using ProtoBuf;

namespace Nostradamus.Tests.Events
{
	[ProtoContract]
	class ActorMovedEvent : IEventArgs
	{
		[ProtoMember(1)]
		public float PositionX { get; set; }

		[ProtoMember(2)]
		public float PositionY { get; set; }
	}
}