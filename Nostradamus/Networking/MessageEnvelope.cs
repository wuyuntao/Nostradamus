using FlatBuffers;
using System;

namespace Nostradamus.Networking
{
    public class MessageEnvelope
    {
        public readonly object Message;

        public MessageEnvelope(object message)
        {
            Message = message;
        }
    }

    class MessageEnvelopeSerializer : Serializer<MessageEnvelope, Schema.MessageEnvelope>
    {
        public static readonly MessageEnvelopeSerializer Instance = new MessageEnvelopeSerializer();

        public override Offset<Schema.MessageEnvelope> Serialize(FlatBufferBuilder fbb, MessageEnvelope envelope)
        {
            var key = GetTypeKey(envelope.Message.GetType());
            var serializer = GetSerializer(key);
            var data = serializer.Serialize(envelope.Message);

            var oId = fbb.CreateString(key);
            var voData = Nostradamus.Schema.MessageEnvelope.CreateDataVector(fbb, data);
            var oEnvelope = Nostradamus.Schema.MessageEnvelope.CreateMessageEnvelope(fbb, oId, voData);

            return oEnvelope;
        }

        public override MessageEnvelope Deserialize(Schema.MessageEnvelope envelope)
        {
            var serializer = GetSerializer(envelope.Id);
            var segment = envelope.GetDataBytes().Value;
            var data = new byte[segment.Count];
            Array.Copy(segment.Array, segment.Offset, data, 0, segment.Count);
            var message = serializer.Deserialize(data);

            return new MessageEnvelope(message);
        }

        public override Schema.MessageEnvelope ToFlatBufferObject(ByteBuffer buffer)
        {
            return Schema.MessageEnvelope.GetRootAsMessageEnvelope(buffer);
        }
    }
}
