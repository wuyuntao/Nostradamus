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
		public ClientId ClientId { get; set; }

		[ProtoMember(2)]
		public ActorId ActorId { get; set; }

		[ProtoMember(3)]
		public int Sequence { get; set; }

		[ProtoMember(4)]
		public int Time { get; set; }

		[ProtoMember(5)]
		public int DeltaTime { get; set; }

		[ProtoMember(6, DynamicType = true)]
		public object Args { get; set; }

		public Command(ClientId clientId, ActorId actorId, int sequence, int time, int deltaTime, ICommandArgs args)
		{
			ClientId = clientId;
			ActorId = actorId;
			Time = time;
			DeltaTime = deltaTime;
			Sequence = sequence;
			Args = args;
		}

		public Command() { }

		public override string ToString()
		{
			return string.Format("{0} ({1}, {2}, #{3})", GetType().Name, ClientId, ActorId, Sequence);
		}

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