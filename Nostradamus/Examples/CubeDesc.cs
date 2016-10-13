using BulletSharp;
using BulletSharp.Math;
using FlatBuffers;
using Nostradamus.Networking;
using Nostradamus.Physics;

namespace Nostradamus.Examples
{
    public class CubeDesc : RigidBodyDesc
    {
        public CubeDesc(ActorId id, Vector3 initialPosition)
            : base(id, 1, new BoxShape(1), Matrix.Identity, Matrix.Translation(initialPosition), false, 2, 2, 0.5f, 0.2f, 0.2f)
        { }

        protected internal override ISnapshotArgs InitSnapshot()
        {
            return new RigidBodySnapshot()
            {
                Position = StartTransform.Origin,
                Rotation = Quaternion.Identity,
            };
        }
    }

    class CubeDescSerializer : Serializer<CubeDesc, Schema.CubeDesc>
    {
        public static readonly CubeDescSerializer Instance = new CubeDescSerializer();

        public override Offset<Schema.CubeDesc> Serialize(FlatBufferBuilder fbb, CubeDesc obj)
        {
            var oId = ActorIdSerializer.Instance.Serialize(fbb, obj.Id);
            var oPosition = Vector3Serializer.Instance.Serialize(fbb, obj.StartTransform.Origin);

            return Schema.CubeDesc.CreateCubeDesc(fbb, oId, oPosition);
        }

        public override CubeDesc Deserialize(Schema.CubeDesc desc)
        {
            var id = ActorIdSerializer.Instance.Deserialize(desc.Id);
            var position = Vector3Serializer.Instance.Deserialize(desc.InitialPosition);

            return new CubeDesc(id, position);
        }

        public override Schema.CubeDesc ToFlatBufferObject(ByteBuffer buffer)
        {
            return Schema.CubeDesc.GetRootAsCubeDesc(buffer);
        }
    }
}
