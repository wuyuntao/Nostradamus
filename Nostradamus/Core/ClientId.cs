using System;

namespace Nostradamus
{
	public class ClientId : IEquatable<ClientId>
	{
		private readonly int value;
		private readonly string description;

		internal ClientId(int value, string description = null)
		{
			this.value = value;
			this.description = description;
		}

		public override bool Equals(object obj)
		{
			if( !( obj is ClientId ) )
				return false;

			return Equals( (ClientId)obj );
		}

		public bool Equals(ClientId other)
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
