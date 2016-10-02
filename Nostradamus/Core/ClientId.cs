using ProtoBuf;
using System;

namespace Nostradamus
{
	[ProtoContract]
	public sealed class ClientId : IEquatable<ClientId>, IComparable<ClientId>
	{
		[ProtoMember(1)]
		public int Value { get; set; }

		[ProtoMember(2, IsRequired = false)]
		public string Description { get; set; }

		public ClientId(int value, string description = null)
		{
			Value = value;
			Description = description;
		}

		public ClientId() { }

		public override bool Equals(object obj)
		{
			if (!(obj is ClientId))
				return false;

			return Equals((ClientId)obj);
		}

		public bool Equals(ClientId other)
		{
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

		#region IComparable

		int IComparable<ClientId>.CompareTo(ClientId other)
		{
			return Value.CompareTo(other.Value);
		}

		#endregion

		public static bool operator ==(ClientId a, ClientId b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(ClientId a, ClientId b)
		{
			return !a.Equals(b);
		}
	}
}
