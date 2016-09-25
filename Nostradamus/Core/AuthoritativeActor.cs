namespace Nostradamus
{
	public abstract class AuthoritativeActor : Actor
	{
		Timeline authoritativeTimeline;

		protected AuthoritativeActor(Scene scene, ActorId id)
			: base( scene, id )
		{
			authoritativeTimeline = new Timeline( "Authoritative" );
		}

		internal void AddAuthoritativePoint(int time, int lastCommandSeq, ISnapshotArgs snapshot)
		{
			authoritativeTimeline.AddPoint( time, snapshot );
		}

		internal Timeline AuthoritativeTimeline
		{
			get { return authoritativeTimeline; }
		}
	}
}