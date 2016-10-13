using FlatBuffers;
using Nostradamus.Networking;
using Nostradamus.Schema;
using System.Collections.Generic;
using System.Linq;

namespace Nostradamus.Server
{
    public sealed class DeltaSyncFrame
    {
        public readonly int Time;

        public readonly int DeltaTime;

        public readonly List<Event> Events;

        public readonly SortedList<ClientId, int> LastCommandSeqs;

        public DeltaSyncFrame(int time, int deltaTime)
        {
            Time = time;
            DeltaTime = deltaTime;
            Events = new List<Event>();
            LastCommandSeqs = new SortedList<ClientId, int>();
        }
    }

    class CommandSeqSerializer : Serializer<KeyValuePair<ClientId, int>, Schema.CommandSeq>
    {
        public static readonly CommandSeqSerializer Instance = new CommandSeqSerializer();

        public override Offset<CommandSeq> Serialize(FlatBufferBuilder fbb, KeyValuePair<ClientId, int> pair)
        {
            var oClientId = ClientIdSerializer.Instance.Serialize(fbb, pair.Key);

            return CommandSeq.CreateCommandSeq(fbb, oClientId, pair.Value);
        }

        public override KeyValuePair<ClientId, int> Deserialize(CommandSeq seq)
        {
            var clientId = ClientIdSerializer.Instance.Deserialize(seq.ClientId.Value);

            return new KeyValuePair<ClientId, int>(clientId, seq.Sequence);
        }

        public override CommandSeq ToFlatBufferObject(ByteBuffer buffer)
        {
            return CommandSeq.GetRootAsCommandSeq(buffer);
        }
    }

    class DeltaSyncFrameSerializer : Serializer<DeltaSyncFrame, Schema.DeltaSyncFrame>
    {
        public static readonly DeltaSyncFrameSerializer Instance = new DeltaSyncFrameSerializer();

        public override Offset<Schema.DeltaSyncFrame> Serialize(FlatBufferBuilder fbb, DeltaSyncFrame frame)
        {
            var oEvents = EventSerializer.Instance.Serialize(fbb, frame.Events).ToArray();
            var voEvents = Schema.DeltaSyncFrame.CreateEventsVector(fbb, oEvents);

            var oCommandSeqs = CommandSeqSerializer.Instance.Serialize(fbb, frame.LastCommandSeqs).ToArray();
            var voCommandSeqs = Schema.DeltaSyncFrame.CreateLastCommandSeqsVector(fbb, oCommandSeqs);

            return Schema.DeltaSyncFrame.CreateDeltaSyncFrame(fbb, frame.Time, frame.DeltaTime, voEvents, voCommandSeqs);
        }

        public override DeltaSyncFrame Deserialize(Schema.DeltaSyncFrame frame)
        {
            var newFrame = new DeltaSyncFrame(frame.Time, frame.DeltaTime);

            for (int i = 0; i < frame.EventsLength; i++)
            {
                var e = frame.Events(i).Value;

                newFrame.Events.Add(EventSerializer.Instance.Deserialize(e));
            }

            for (int i = 0; i < frame.LastCommandSeqsLength; i++)
            {
                var commandSeq = frame.LastCommandSeqs(i).Value;
                var pair = CommandSeqSerializer.Instance.Deserialize(commandSeq);

                newFrame.LastCommandSeqs.Add(pair.Key, pair.Value);
            }

            return newFrame;
        }

        public override Schema.DeltaSyncFrame ToFlatBufferObject(ByteBuffer buffer)
        {
            return Schema.DeltaSyncFrame.GetRootAsDeltaSyncFrame(buffer);
        }
    }
}