using FlatBuffers;
using FlatBuffers.Schema;

namespace Nostradamus.Examples
{
    public class SceneInitializedEvent : IEventArgs
    {
        public readonly byte CubeRows;

        public readonly byte CubeColumns;

        public SceneInitializedEvent(byte cubeRows, byte cubeColumns)
        {
            CubeRows = cubeRows;
            CubeColumns = cubeColumns;
        }
    }

    class SceneInitializedEventSerializer : Serializer<SceneInitializedEvent, Schema.SceneInitializedEvent>
    {
        public static readonly SceneInitializedEventSerializer Instance = new SceneInitializedEventSerializer();

        public override SceneInitializedEvent Deserialize(Schema.SceneInitializedEvent e)
        {
            return new SceneInitializedEvent(e.CubeRows, e.CubeColumns);
        }

        public override Offset<Schema.SceneInitializedEvent> Serialize(FlatBufferBuilder fbb, SceneInitializedEvent e)
        {
            return Schema.SceneInitializedEvent.CreateSceneInitializedEvent(fbb, e.CubeRows, e.CubeColumns);
        }

        protected override Schema.SceneInitializedEvent GetRootAs(ByteBuffer buffer)
        {
            return Schema.SceneInitializedEvent.GetRootAsSceneInitializedEvent(buffer);
        }
    }
}
