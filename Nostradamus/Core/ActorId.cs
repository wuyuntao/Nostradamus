using ProtoBuf;
using System;

namespace Nostradamus
{
	[ProtoContract]
	public sealed class ActorId : IEquatable<ActorId>, IComparable<ActorId>
	{
		[ProtoMember(1)]
		public int Value { get; set; }

		[ProtoMember(2, IsRequired = false)]
		public string Description { get; set; }

		public ActorId(int value, string description = null)
		{
			Value = value;
			Description = description;
		}

		public ActorId() { }

		public override bool Equals(object obj)
		{
			if (!(obj is ActorId))
				return false;

			return Equals((ActorId)obj);
		}

		public bool Equals(ActorId other)
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

		int IComparable<ActorId>.CompareTo(ActorId other)
		{
			return Value.CompareTo(other.Value);
		}

		#endregion

		public static bool operator ==(ActorId a, ActorId b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(ActorId a, ActorId b)
		{
			return !a.Equals(b);
		}
	}
}
