
namespace Nostradamus
{
    public interface ICommandArgs
    {
    }

    public sealed class Command
    {
        public readonly ActorId ActorId;

        public readonly int Sequence;

        public readonly ICommandArgs Args;

        public int Time;

        public int DeltaTime;

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