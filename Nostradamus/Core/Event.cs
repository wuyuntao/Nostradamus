namespace Nostradamus
{
	public interface IEventArgs
	{
	}

	public sealed class Event
	{
		public readonly ActorId ActorId;
		public readonly int Time;
		public readonly int LastCommandSeq;
		public readonly IEventArgs Args;

		public Event(ActorId actorId, int time, int lastCommandSeq, IEventArgs args)
		{
			ActorId = actorId;
			Time = time;
			LastCommandSeq = lastCommandSeq;
			Args = args;
		}
	}
}
