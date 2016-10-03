
namespace Nostradamus
{
	public interface ICommandArgs
	{
	}

	public sealed class Command
	{
		public ClientId ClientId { get; set; }

		public ActorId ActorId { get; set; }

		public int Sequence { get; set; }

		public int Time { get; set; }

		public int DeltaTime { get; set; }

		public ICommandArgs Args { get; set; }

		public Command(ClientId clientId, ActorId actorId, int sequence, int time, int deltaTime, ICommandArgs args)
		{
			ClientId = clientId;
			ActorId = actorId;
			Time = time;
			DeltaTime = deltaTime;
			Sequence = sequence;
			Args = args;
		}

		public override string ToString()
		{
			return string.Format("{0} ({1}, {2}, #{3})", GetType().Name, ClientId, ActorId, Sequence);
		}
	}
}