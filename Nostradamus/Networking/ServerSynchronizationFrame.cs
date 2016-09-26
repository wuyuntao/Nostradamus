namespace Nostradamus.Networking
{
	class ServerSynchronizationFrame
	{
		public readonly int Time;
		public readonly Event[] Events;

		public ServerSynchronizationFrame(int time, Event[] commands)
		{
			Time = time;
			Events = commands;
		}
	}
}
