namespace Nostradamus
{
	public interface ICommandArgs
	{
	}

	public sealed class Command
	{
		public readonly ActorId ActorId;
		public readonly int Time;
		public readonly int Sequence;
		public readonly ICommandArgs Args;
		public bool IsDequeued;

		public Command(ActorId actorId, int time, int sequence, ICommandArgs args)
		{
			ActorId = actorId;
			Time = time;
			Sequence = sequence;
			Args = args;
		}
	}
}