
namespace Nostradamus
{
	public interface ICommandArgs
	{
	}

	public sealed class Command
	{
		public ActorId ActorId { get; set; }

		public int Sequence { get; set; }

		public int Time { get; set; }

		public int DeltaTime { get; set; }

		public ICommandArgs Args { get; set; }

		public Command(ActorId actorId, int sequence, int time, int deltaTime, ICommandArgs args)
		{
			ActorId = actorId;
			Time = time;
			DeltaTime = deltaTime;
			Sequence = sequence;
			Args = args;
		}

		public override string ToString()
		{
			return string.Format("{0} ({1}, #{2})", GetType().Name, ActorId, Sequence);
		}
	}
}