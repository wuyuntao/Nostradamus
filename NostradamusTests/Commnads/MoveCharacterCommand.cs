using ProtoBuf;

namespace Nostradamus.Tests.Commnads
{
	[ProtoContract]
	public class MoveCharacterCommand : ICommandArgs
	{
		[ProtoMember(1)]
		public float DeltaX { get; set; }

		[ProtoMember(1)]
		public float DeltaY { get; set; }
	}
}
