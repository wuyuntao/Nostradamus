using ProtoBuf;

namespace Nostradamus.Examples
{
	[ProtoContract]
	public class ActorMovedEvent : IEventArgs
	{
		[ProtoMember(1)]
		public float PositionX { get; set; }

		[ProtoMember(2)]
		public float PositionY { get; set; }
	}
}