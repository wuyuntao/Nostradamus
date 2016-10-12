namespace Nostradamus
{
    public sealed class ActorContext
    {
        private ActorManager actorManager;
        private Actor actor;

        internal ActorContext(ActorManager actorManager, Actor actor)
        {
            this.actorManager = actorManager;
            this.actor = actor;
        }

        public TActor CreateActor<TActor>(ActorDesc desc)
            where TActor : Actor, new()
        {
            return ActorManager.CreateActor<TActor>(desc);
        }

        public Actor GetActor(ActorId id)
        {
            return ActorManager.GetActor(id);
        }

        internal ActorManager ActorManager
        {
            get { return actorManager; }
        }

        internal Actor Actor
        {
            get { return actor; }
        }
    }
}
