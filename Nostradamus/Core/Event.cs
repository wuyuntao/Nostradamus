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

		[ProtoMember(2, DynamicType = true)]
		public object Args { get; set; }

		public Event(ActorId actorId, IEventArgs args)
		{
			ActorId = actorId;
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
