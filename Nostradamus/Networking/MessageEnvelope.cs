using FlatBuffers;
using FlatBuffers.Schema;
using System;

namespace Nostradamus.Networking
{
    public class MessageEnvelope
    {
        public readonly object Message;

        public MessageEnvelope(object message)
        {
            MessageEnvelopeSerializer s = null;
            Message = message;
        }
    }

    class MessageEnvelopeSerializer : Serializer<MessageEnvelope, Schema.MessageEnvelope>
    {
        public static readonly MessageEnvelopeSerializer Instance = new MessageEnvelopeSerializer();

        public override Offset<Schema.MessageEnvelope> Serialize(FlatBufferBuilder fbb, MessageEnvelope envelope)
        {
            var type = envelope.Message.GetType();
            var data = SerializerSet.Instance.Serialize(type, envelope.Message);

            var oId = fbb.CreateString(type.FullName);
            var voData = Nostradamus.Schema.MessageEnvelope.CreateDataVector(fbb, data);
            var oEnvelope = Nostradamus.Schema.MessageEnvelope.CreateMessageEnvelope(fbb, oId, voData);

            return oEnvelope;
        }

        public override MessageEnvelope Deserialize(Schema.MessageEnvelope envelope)
        {
            var segment = envelope.GetDataBytes().Value;
            var data = new byte[segment.Count];
            Array.Copy(segment.Array, segment.Offset, data, 0, segment.Count);
            var message = SerializerSet.Instance.Deserialize(envelope.Id, data);

            return new MessageEnvelope(message);
        }

        protected override Schema.MessageEnvelope GetRootAs(ByteBuffer buffer)
        {
            return Schema.MessageEnvelope.GetRootAsMessageEnvelope(buffer);
        }
    }
}
