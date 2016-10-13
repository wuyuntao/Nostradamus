using System;
using BulletSharp;
using BulletSharp.Math;
using FlatBuffers;
using Nostradamus.Networking;
using Nostradamus.Physics;

namespace Nostradamus.Examples
{
    public class BallDesc : RigidBodyDesc
    {
        public readonly float HorizontalForceFactor = 1000;

        public readonly float VerticalForceFactor = 1000;

        public BallDesc(ActorId id, Vector3 initialPosition)
            : base(id, 10, new SphereShape(2.5f), Matrix.Identity, Matrix.Translation(initialPosition), false, 1, 1, 0.8f, 0.2f, 0.2f)
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

    class BallDescSerializer : Serializer<BallDesc, Schema.BallDesc>
    {
        public static readonly BallDescSerializer Instance = new BallDescSerializer();

        public override Offset<Schema.BallDesc> Serialize(FlatBufferBuilder fbb, BallDesc obj)
        {
            var oId = ActorIdSerializer.Instance.Serialize(fbb, obj.Id);
            var oPosition = Vector3Serializer.Instance.Serialize(fbb, obj.StartTransform.Origin);

            return Schema.BallDesc.CreateBallDesc(fbb, oId, oPosition);
        }

        public override BallDesc Deserialize(Schema.BallDesc desc)
        {
            var id = ActorIdSerializer.Instance.Deserialize(desc.Id);
            var position = Vector3Serializer.Instance.Deserialize(desc.InitialPosition);

            return new BallDesc(id, position);
        }

        public override Schema.BallDesc ToFlatBufferObject(ByteBuffer buffer)
        {
            return Schema.BallDesc.GetRootAsBallDesc(buffer);
        }
    }
}
