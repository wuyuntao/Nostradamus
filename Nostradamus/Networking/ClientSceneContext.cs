using System;

namespace Nostradamus.Networking
{
	class ClientSceneContext : SceneContext
	{
		public ClientSceneContext(Scene scene)
			: base(scene)
		{
		}

		internal override ActorContext CreateActorContext(Actor actor, int time, ISnapshotArgs snapshot)
		{
			return new ClientActorContext(this, actor, time, snapshot);
		}

		public void EnqueueAuthoritativeEvent(ActorId actorId, int time, int lastCommandSequence, IEventArgs @event)
		{
			var actor = Scene.GetActor(actorId);
			if (actor == null)
				throw new ArgumentException(string.Format("{0} not exist", actorId));

			var actorContext = (ClientActorContext)actor.Context;
			actorContext.EnqueueAuthoritativeEvent(time, lastCommandSequence, @event);
		}
	}
}
