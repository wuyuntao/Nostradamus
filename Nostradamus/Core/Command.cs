namespace Nostradamus
{
	public interface ICommandArgs
	{
	}

	public sealed class Command
	{
		public readonly ActorId ActorId;
		public readonly int Time;
		public readonly int DeltaTime;
		public readonly int Sequence;
		public readonly ICommandArgs Args;

		public Command(ActorId actorId, int time, int deltaTime, int sequence, ICommandArgs args)
		{
			ActorId = actorId;
			Time = time;
			DeltaTime = deltaTime;
			Sequence = sequence;
			Args = args;
		}
	}
}