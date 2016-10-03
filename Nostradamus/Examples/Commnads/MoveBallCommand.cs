using ProtoBuf;

namespace Nostradamus.Examples
{
	[ProtoContract]
	public class MoveBallCommand : ICommandArgs
	{
		[ProtoMember(1)]
		public float InputX { get; set; }

		[ProtoMember(2)]
		public float InputY { get; set; }

		[ProtoMember(3)]
		public float InputZ { get; set; }
	}
}
