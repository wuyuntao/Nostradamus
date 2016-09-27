namespace Nostradamus
{
	public interface IEventArgs
	{
	}

	public sealed class Event
	{
		public readonly ActorId ActorId;
		public readonly ClientId ClientId;
		public readonly int Time;
		public readonly int LastCommandSeq;
		public readonly IEventArgs Args;

		public Event(ActorId actorId, ClientId clientId, int time, int lastCommandSeq, IEventArgs args)
		{
			ActorId = actorId;
			ClientId = clientId;
			Time = time;
			LastCommandSeq = lastCommandSeq;
			Args = args;
		}
	}
}
