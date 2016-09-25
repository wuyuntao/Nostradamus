namespace Nostradamus
{
	public interface IEventArgs
	{
	}

	class Event
	{
		public readonly ActorId ActorId;
		public readonly int Time;
		public readonly int LastCommandSequence;
		public readonly IEventArgs Args;

		public Event(ActorId actorId, int time, int lastCommandSequence, IEventArgs args)
		{
			ActorId = actorId;
			Time = time;
			LastCommandSequence = lastCommandSequence;
			Args = args;
		}
	}

}
