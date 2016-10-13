using FlatBuffers;
using Nostradamus.Networking;

namespace Nostradamus
{
    public interface ISnapshotArgs
    {
        ISnapshotArgs Clone();

        ISnapshotArgs Interpolate(ISnapshotArgs snapshot, float factor);

        ISnapshotArgs Extrapolate(int deltaTime);

        bool IsApproximate(ISnapshotArgs snapshot);
    }

    public sealed class ActorSnapshot
    {
        public readonly ActorDesc Desc;

        public readonly ISnapshotArgs Args;

        public ActorSnapshot(ActorDesc actorDesc, ISnapshotArgs args)
        {
            Desc = actorDesc;
            Args = args;
        }
    }

    class ActorSnapshotSerializer : Serializer<ActorSnapshot, Schema.ActorSnapshot>
    {
        public static readonly ActorSnapshotSerializer Instance = new ActorSnapshotSerializer();

        public override Offset<Schema.ActorSnapshot> Serialize(FlatBufferBuilder fbb, ActorSnapshot e)
        {
            var oDesc = MessageEnvelopeSerializer.Instance.Serialize(fbb, new MessageEnvelope(e.Desc));
            var oArgs = MessageEnvelopeSerializer.Instance.Serialize(fbb, new MessageEnvelope(e.Args));

            return Schema.ActorSnapshot.CreateActorSnapshot(fbb, oDesc, oArgs);
        }

        public override ActorSnapshot Deserialize(Schema.ActorSnapshot e)
        {
            var desc = MessageEnvelopeSerializer.Instance.Deserialize(e.Desc.Value);
            var args = MessageEnvelopeSerializer.Instance.Deserialize(e.Args.Value);

            return new ActorSnapshot((ActorDesc)desc.Message, (ISnapshotArgs)args.Message);
        }

        public override Schema.ActorSnapshot ToFlatBufferObject(ByteBuffer buffer)
        {
            return Schema.ActorSnapshot.GetRootAsActorSnapshot(buffer);
        }
    }
}
