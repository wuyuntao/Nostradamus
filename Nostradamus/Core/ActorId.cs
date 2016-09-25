using System;

namespace Nostradamus
{
	public class ActorId : IEquatable<ActorId>
	{
		private readonly int value;
		private readonly string description;

		internal ActorId(int value, string description = null)
		{
			this.value = value;
			this.description = description;
		}

		public override bool Equals(object obj)
		{
			if( !( obj is ActorId ) )
				return false;

			return Equals( (ActorId)obj );
		}

		public bool Equals(ActorId other)
		{
			return value.Equals( other.value );
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public override string ToString()
		{
			if( string.IsNullOrEmpty( description ) )
				return string.Format( "{0} #{1}", GetType().Name, value );
			else
				return string.Format( "{0} #{1} ({2})", GetType().Name, value, description );
		}

		//public int Value
		//{
		//	get { return value; }
		//}

		//public string Description
		//{
		//	get { return description; }
		//}
	}
}
