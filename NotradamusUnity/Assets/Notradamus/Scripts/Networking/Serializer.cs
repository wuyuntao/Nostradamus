using BulletSharp.Math;
using FlatBuffers;
using Nostradamus.Client;
using Nostradamus.Examples;
using Nostradamus.Physics;
using Nostradamus.Server;
using System;
using System.Collections.Generic;

namespace Nostradamus.Networking
{
    public abstract class Serializer
    {
        private static Dictionary<string, Serializer> serializers = new Dictionary<string, Serializer>();

        public static void Initialize()
        {
            // Common
            AddSerializer(MessageEnvelopeSerializer.Instance);
            AddSerializer(ActorIdSerializer.Instance);
            AddSerializer(ClientIdSerializer.Instance);
            AddSerializer(CommandSerializer.Instance);
            AddSerializer(EventSerializer.Instance);
            AddSerializer(ActorSnapshotSerializer.Instance);
            AddSerializer(SimulatorSnapshotSerializer.Instance);

            // Client
            AddSerializer(CommandFrameSerializer.Instance);

            // Server
            AddSerializer(FullSyncFrameSerializer.Instance);
            AddSerializer(CommandSeqSerializer.Instance);
            AddSerializer(DeltaSyncFrameSerializer.Instance);
            AddSerializer(LoginSerializer.Instance);

            // Physics
            AddSerializer(Vector3Serializer.Instance);
            AddSerializer(QuaternionSerializer.Instance);
            AddSerializer(RigidBodyMovedEventSerializer.Instance);
            AddSerializer(RigidBodySnapshotSerializer.Instance);

            // Examples
            AddSerializer(BallDescSerializer.Instance);
            AddSerializer(CubeDescSerializer.Instance);
            AddSerializer(KickBallCommandSerializer.Instance);
            AddSerializer(SceneInitializedEventSerializer.Instance);
        }

        public abstract byte[] Serialize(object obj);

        public abstract object Deserialize(byte[] data);

        #region Serialization

        public static void AddSerializer<TObject, TFlatBufferObject>(Serializer<TObject, TFlatBufferObject> serializer)
            where TFlatBufferObject : struct, IFlatbufferObject
        {
            serializers.Add(GetTypeKey(typeof(TObject)), serializer);
        }

        public static Serializer GetSerializer(string key)
        {
            Serializer serializer;
            if (!serializers.TryGetValue(key, out serializer))
                throw new KeyNotFoundException(key.ToString());

            return serializer;
        }

        public static string GetTypeKey(Type type)
        {
            return type.FullName;
        }

        public static byte[] Serialize<T>(T obj)
        {
            var key = GetTypeKey(typeof(T));
            var serializer = GetSerializer(key);

            return serializer.Serialize((object)obj);
        }

        public static T Deserialize<T>(byte[] data)
        {
            var key = GetTypeKey(typeof(T));
            var serializer = GetSerializer(key);

            return (T)serializer.Deserialize(data);
        }

        #endregion
    }

    public abstract class Serializer<TObject, TFlatBufferObject> : Serializer
        where TFlatBufferObject : struct, IFlatbufferObject
    {
        public override byte[] Serialize(object obj)
        {
            var fbb = new FlatBufferBuilder(1024);
            var offset = Serialize(fbb, (TObject)obj);

            return ToBytes(fbb, offset);
        }

        public override object Deserialize(byte[] data)
        {
            var fbObj = ToFlatBufferObject(new ByteBuffer(data));

            return Deserialize(fbObj);
        }

        public abstract Offset<TFlatBufferObject> Serialize(FlatBufferBuilder fbb, TObject obj);

        public IEnumerable<Offset<TFlatBufferObject>> Serialize(FlatBufferBuilder fbb, IEnumerable<TObject> objects)
        {
            foreach (var obj in objects)
            {
                yield return Serialize(fbb, obj);
            }
        }

        public byte[] ToBytes(FlatBufferBuilder fbb, Offset<TFlatBufferObject> offset)
        {
            fbb.Finish(offset.Value);

            return fbb.SizedByteArray();
        }

        public TObject Deserialize(TFlatBufferObject? obj)
        {
            return obj.HasValue ? Deserialize(obj.Value) : default(TObject);
        }

        public abstract TObject Deserialize(TFlatBufferObject obj);

        public abstract TFlatBufferObject ToFlatBufferObject(ByteBuffer buffer);
    }
}