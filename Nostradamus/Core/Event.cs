namespace Nostradamus
{
	class Event
	{
		public readonly int Time;
		public readonly IEventArgs Args;

		public Event(int time, IEventArgs args)
		{
			Time = time;
			Args = args;
		}
	}

	public interface IEventArgs
	{
	}
}
