using FlatBuffers;
using Nostradamus.Networking;
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
        public static readonly ClientIdSerializer Instance = new ClientIdSerializer();

        public override Offset<Schema.ClientId> Serialize(FlatBufferBuilder fbb, ClientId id)
        {
            return Schema.ClientId.CreateClientId(fbb, id.Value, fbb.CreateString(id.Description));
        }

        public override ClientId Deserialize(Schema.ClientId id)
        {
            return new ClientId(id.Value, id.Description);
        }

        public override Schema.ClientId ToFlatBufferObject(ByteBuffer buffer)
        {
            return Schema.ClientId.GetRootAsClientId(buffer);
        }
    }
}
