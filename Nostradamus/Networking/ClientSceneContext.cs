using System;

namespace Nostradamus.Networking
{
	class ClientSceneContext : SceneContext
	{
		public void EnqueueAuthoritativeEvent(ActorId actorId, int time, int lastCommandSequence, IEventArgs @event)
		{
			var actor = Scene.GetActor( actorId );
			if( actor == null )
				throw new ArgumentException( string.Format( "{0} not exist", actorId ) );

			var actorContext = (ClientActorContext)actor.Context;
			actorContext.EnqueueAuthoritativeEvent( time, lastCommandSequence, @event );
		}
	}
}
