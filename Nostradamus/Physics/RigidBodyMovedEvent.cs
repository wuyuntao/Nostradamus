using BulletSharp.Math;
using FlatBuffers;
using FlatBuffers.Schema;
using Nostradamus.Networking;
using System;

namespace Nostradamus.Physics
{
    public class RigidBodyMovedEvent : IEventArgs
    {
        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public Vector3 LinearVelocity { get; set; }

        public Vector3 AngularVelocity { get; set; }
    }

    public class Vector3Serializer : Serializer<Vector3, Schema.Vector3>
    {
        public static readonly Vector3Serializer Instance = SerializerSet.Instance.CreateSerializer<Vector3Serializer, Vector3, Schema.Vector3>();

        public override Vector3 Deserialize(Schema.Vector3 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public override Offset<Schema.Vector3> Serialize(FlatBufferBuilder fbb, Vector3 vector)
        {
            return Schema.Vector3.CreateVector3(fbb, vector.X, vector.Y, vector.Z);
        }

        protected override Schema.Vector3 GetRootAs(ByteBuffer buffer)
        {
            return Schema.Vector3.GetRootAsVector3(buffer);
        }
    }

    public class QuaternionSerializer : Serializer<Quaternion, Schema.Quaternion>
    {
        public static readonly QuaternionSerializer Instance = SerializerSet.Instance.CreateSerializer<QuaternionSerializer, Quaternion, Schema.Quaternion>();

        public override Quaternion Deserialize(Schema.Quaternion quaternion)
        {
            return new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

        public override Offset<Schema.Quaternion> Serialize(FlatBufferBuilder fbb, Quaternion quaternion)
        {
            return Schema.Quaternion.CreateQuaternion(fbb, quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

        protected override Schema.Quaternion GetRootAs(ByteBuffer buffer)
        {
            return Schema.Quaternion.GetRootAsQuaternion(buffer);
        }
    }

    public class RigidBodyMovedEventSerializer : Serializer<RigidBodyMovedEvent, Schema.RigidBodyMovedEvent>
    {
        public static readonly RigidBodyMovedEventSerializer Instance = new RigidBodyMovedEventSerializer();

        public override Offset<Schema.RigidBodyMovedEvent> Serialize(FlatBufferBuilder fbb, RigidBodyMovedEvent e)
        {
            var oPosition = Vector3Serializer.Instance.Serialize(fbb, e.Position);
            var oRotation = QuaternionSerializer.Instance.Serialize(fbb, e.Rotation);
            var oLinearVelocity = Vector3Serializer.Instance.Serialize(fbb, e.LinearVelocity);
            var oAngularVelocity = Vector3Serializer.Instance.Serialize(fbb, e.AngularVelocity);

            return Schema.RigidBodyMovedEvent.CreateRigidBodyMovedEvent(fbb, oPosition, oRotation, oLinearVelocity, oAngularVelocity);
        }

        public override RigidBodyMovedEvent Deserialize(Schema.RigidBodyMovedEvent e)
        {
            var position = Vector3Serializer.Instance.Deserialize(e.Position);
            var rotation = QuaternionSerializer.Instance.Deserialize(e.Rotation);
            var linearVelocity = Vector3Serializer.Instance.Deserialize(e.LinearVelocity);
            var angularVelocity = Vector3Serializer.Instance.Deserialize(e.AngularVelocity);

            return new RigidBodyMovedEvent() { Position = position, Rotation = rotation, LinearVelocity = linearVelocity, AngularVelocity = angularVelocity };
        }

        protected override Schema.RigidBodyMovedEvent GetRootAs(ByteBuffer buffer)
        {
            return Schema.RigidBodyMovedEvent.GetRootAsRigidBodyMovedEvent(buffer);
        }
    }
}
