using FlatBuffers;
using FlatBuffers.Schema;
using Nostradamus.Networking;

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

        public object ClientId { get; internal set; }

        public override string ToString()
        {
            return string.Format("{0} ({1})", GetType().Name, ActorId);
        }
    }

    class EventSerializer : Serializer<Event, Schema.Event>
    {
        public static readonly EventSerializer Instance = new EventSerializer();


        public override Offset<Schema.Event> Serialize(FlatBufferBuilder fbb, Event e)
        {
            var oActorIdDesc = string.IsNullOrEmpty(e.ActorId.Description) ? default(StringOffset) : fbb.CreateString(e.ActorId.Description);
            var oActorId = Schema.ActorId.CreateActorId(fbb, e.ActorId.Value, oActorIdDesc);

            var oArgs = MessageEnvelopeSerializer.Instance.Serialize(fbb, new Networking.MessageEnvelope(e.Args));

            return Schema.Event.CreateEvent(fbb, oActorId, oArgs);
        }

        public override Event Deserialize(Schema.Event e)
        {
            var actorId = ActorIdSerializer.Instance.Deserialize(e.ActorId.Value);
            var args = MessageEnvelopeSerializer.Instance.Deserialize(e.Args.Value);

            return new Event(actorId, (IEventArgs)args.Message);
        }

        protected override Schema.Event GetRootAs(ByteBuffer buffer)
        {
            return Schema.Event.GetRootAsEvent(buffer);
        }
    }
}
