using FlatBuffers;
using Nostradamus.Networking;

namespace Nostradamus.Examples
{
    public class SceneInitializedEvent : IEventArgs
    { }

    public class SceneInitializedEventSerializer : Serializer<SceneInitializedEvent, Schema.SceneInitializedEvent>
    {
        public static readonly SceneInitializedEventSerializer Instance = new SceneInitializedEventSerializer();

        public override SceneInitializedEvent Deserialize(Schema.SceneInitializedEvent e)
        {
            return new SceneInitializedEvent();
        }

        public override Offset<Schema.SceneInitializedEvent> Serialize(FlatBufferBuilder fbb, SceneInitializedEvent command)
        {
            Schema.SceneInitializedEvent.StartSceneInitializedEvent(fbb);
            return Schema.SceneInitializedEvent.EndSceneInitializedEvent(fbb);
        }

        public override Schema.SceneInitializedEvent ToFlatBufferObject(ByteBuffer buffer)
        {
            return Schema.SceneInitializedEvent.GetRootAsSceneInitializedEvent(buffer);
        }
    }
}
