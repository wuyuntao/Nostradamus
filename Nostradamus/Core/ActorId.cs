using System;

namespace Nostradamus
{
    public struct ActorId : IEquatable<ActorId>, IComparable<ActorId>
    {
        public readonly int Value;

        public readonly string Description;

        public ActorId(int value, string description = null)
        {
            Value = value;
            Description = description;
        }

        #region IEquatable

        public override bool Equals(object obj)
        {
            if (!(obj is ActorId))
                return false;

            return Equals((ActorId)obj);
        }

        public bool Equals(ActorId other)
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

        int IComparable<ActorId>.CompareTo(ActorId other)
        {
            return Value.CompareTo(other.Value);
        }

        #endregion

        #region Operator Overrie

        public static bool operator ==(ActorId a, ActorId b)
        {
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);
            else
                return a.Equals(b);
        }

        public static bool operator !=(ActorId a, ActorId b)
        {
            return !a.Equals(b);
        }

        #endregion
    }
}
