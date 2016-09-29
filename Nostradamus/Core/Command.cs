using ProtoBuf;

namespace Nostradamus
{
	public interface ICommandArgs
	{
	}

	[ProtoContract]
	public sealed class Command
	{
		[ProtoMember(1)]
		public ActorId ActorId { get; set; }

		[ProtoMember(2)]
		public int Time { get; set; }

		[ProtoMember(3)]
		public int DeltaTime { get; set; }

		[ProtoMember(4)]
		public int Sequence { get; set; }

		[ProtoMember(5, DynamicType = true)]
		public object Args { get; set; }

		public Command(ActorId actorId, int time, int deltaTime, int sequence, ICommandArgs args)
		{
			ActorId = actorId;
			Time = time;
			DeltaTime = deltaTime;
			Sequence = sequence;
			Args = args;
		}

		public Command() { }

		public ICommandArgs GetArgs()
		{
			return (ICommandArgs)Args;
		}

		public TArgs GetArgs<TArgs>()
			where TArgs : ICommandArgs
		{
			return (TArgs)Args;
		}
	}
}