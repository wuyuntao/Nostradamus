using ProtoBuf;

namespace Nostradamus.Examples
{
	[ProtoContract]
	public class MoveCharacterCommand : ICommandArgs
	{
		[ProtoMember(1)]
		public float DeltaX { get; set; }

		[ProtoMember(2)]
		public float DeltaY { get; set; }
	}
}
