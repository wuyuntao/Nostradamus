using FlatBuffers;
using FlatBuffers.Schema;
using System;

namespace Nostradamus
{
    public struct ClientId : IEquatable<ClientId>, IComparable<ClientId>
    {
        public readonly int Value;

        public readonly string Description;

        public ClientId(int value, string description = null)
        {
            Value = value;
            Description = description;
        }

        #region IEquatable

        public override bool Equals(object obj)
        {
            if (!(obj is ClientId))
                return false;

            return Equals((ClientId)obj);
        }

        public bool Equals(ClientId other)
        {
            if (ReferenceEquals(other, null))
                return false;
            else
                return Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Description))
                return string.Format("{0} #{1}", GetType().Name, Value);
            else
                return string.Format("{0} #{1} ({2})", GetType().Name, Value, Description);
        }

        #endregion

        #region IComparable

        int IComparable<ClientId>.CompareTo(ClientId other)
        {
            return Value.CompareTo(other.Value);
        }

        #endregion

        #region Operator Override

        public static bool operator ==(ClientId a, ClientId b)
        {
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);
            else
                return a.Equals(b);
        }

        public static bool operator !=(ClientId a, ClientId b)
        {
            return !(a == b);
        }

        #endregion
    }

    class ClientIdSerializer : Serializer<ClientId, Schema.ClientId>
    {
        public static readonly ClientIdSerializer Instance = SerializerSet.Instance.CreateSerializer<ClientIdSerializer, ClientId, Schema.ClientId>();

        public override Offset<Schema.ClientId> Serialize(FlatBufferBuilder fbb, ClientId id)
        {
            var desc = string.IsNullOrEmpty(id.Description) ? default(StringOffset) : fbb.CreateString(id.Description);

            return Schema.ClientId.CreateClientId(fbb, id.Value, desc);
        }

        public override ClientId Deserialize(Schema.ClientId id)
        {
            return new ClientId(id.Value, id.Description);
        }

        protected override Schema.ClientId GetRootAs(ByteBuffer buffer)
        {
            return Schema.ClientId.GetRootAsClientId(buffer);
        }
    }
}
