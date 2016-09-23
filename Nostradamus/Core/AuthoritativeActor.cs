namespace Nostradamus
{
	public abstract class AuthoritativeActor : Actor
	{
		Branch authoritativeBranch;

		protected AuthoritativeActor(Scene scene)
			: base( scene )
		{
			authoritativeBranch = WorldLine.CreateBranch( "Authoritative" );
		}

		public virtual void OnAuthoritativeEvent(int time, int lastCommandSeq, params IEventArgs[] events)
		{
			var node = authoritativeBranch.CreateNode( time );

			if( events != null )
			{
				foreach( var e in events )
					node.AddEvent( e );
			}
		}

		internal Branch AuthoritativeBranch
		{
			get { return authoritativeBranch; }
		}
	}
}