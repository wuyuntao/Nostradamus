using ProtoBuf;

namespace Nostradamus
{
	public interface IEventArgs
	{
	}

	[ProtoContract]
	public sealed class Event
	{
		[ProtoMember(1)]
		public ActorId ActorId { get; set; }

		[ProtoMember(3)]
		public int Time { get; set; }

		[ProtoMember(5, DynamicType = true)]
		public object Args { get; set; }

		public Event(ActorId actorId, int time, IEventArgs args)
		{
			ActorId = actorId;
			Time = time;
			Args = args;
		}

		public Event() { }

		public IEventArgs GetArgs()
		{
			return (IEventArgs)Args;
		}

		public TArgs GetArgs<TArgs>()
			where TArgs : IEventArgs
		{
			return (TArgs)Args;
		}
	}
}
