
namespace Nostradamus
{
	public interface IEventArgs
	{
	}

	public sealed class Event
	{
		public ActorId ActorId { get; set; }

		public IEventArgs Args { get; set; }

		public Event(ActorId actorId, IEventArgs args)
		{
			ActorId = actorId;
			Args = args;
		}

		public override string ToString()
		{
			return string.Format("{0} ({1})", GetType().Name, ActorId);
		}
	}
}
