
namespace Nostradamus
{
    public interface IEventArgs
    {
    }

    public sealed class Event
    {
        public readonly ActorId ActorId;

        public readonly IEventArgs Args;

        public Event(ActorId actorId, IEventArgs args)
        {
            ActorId = actorId;
            Args = args;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", GetType().Name, ActorId);
        }
    }
}
