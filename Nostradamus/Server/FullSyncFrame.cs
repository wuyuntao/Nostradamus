using FlatBuffers;
using FlatBuffers.Schema;
using Nostradamus.Networking;

namespace Nostradamus.Server
{
    public sealed class FullSyncFrame
    {
        public readonly int Time;

        public readonly int DeltaTime;

        public readonly SimulatorSnapshot Snapshot;

        public FullSyncFrame(int time, int deltaTime)
            : this(time, deltaTime, new SimulatorSnapshot())
        { }

        internal FullSyncFrame(int time, int deltaTime, SimulatorSnapshot snapshot)
        {
            Time = time;
            DeltaTime = deltaTime;
            Snapshot = snapshot;
        }

        public override string ToString()
        {
            return string.Format("{0} (Time: {1}, DeltaTime: {2}, Actors: {3})", GetType().Name, Time, DeltaTime, Snapshot.Actors.Count);
        }
    }

    class FullSyncFrameSerializer : Serializer<FullSyncFrame, Schema.FullSyncFrame>
    {
        public static readonly FullSyncFrameSerializer Instance = new FullSyncFrameSerializer();

        public override Offset<Schema.FullSyncFrame> Serialize(FlatBufferBuilder fbb, FullSyncFrame frame)
        {
            var oSnapshot = SimulatorSnapshotSerializer.Instance.Serialize(fbb, frame.Snapshot);

            return Schema.FullSyncFrame.CreateFullSyncFrame(fbb, frame.Time, frame.DeltaTime, oSnapshot);
        }

        public override FullSyncFrame Deserialize(Schema.FullSyncFrame frame)
        {
            var snapshot = SimulatorSnapshotSerializer.Instance.Deserialize(frame.Snapshot.Value);

            return new FullSyncFrame(frame.Time, frame.DeltaTime, snapshot);
        }

        protected override Schema.FullSyncFrame GetRootAs(ByteBuffer buffer)
        {
            return Schema.FullSyncFrame.GetRootAsFullSyncFrame(buffer);
        }
    }
}