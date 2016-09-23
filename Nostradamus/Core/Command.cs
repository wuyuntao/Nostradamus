namespace Nostradamus
{
	class Command
	{
		public readonly int Time;
		public readonly int Sequence;
		public readonly ICommandArgs Args;

		public Command(int time, int sequence, ICommandArgs args)
		{
			Time = time;
			Sequence = sequence;
			Args = args;
		}
	}

	public interface ICommandArgs
	{
	}
}