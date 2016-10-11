using System;

namespace Nostradamus.Core2
{
    public sealed class ActorContext
    {
        internal ActorManager ActorManager { get; private set; }

        internal Actor Actor { get; private set; }

        internal ActorContext(ActorManager actorManager, Actor actor)
        {
            ActorManager = actorManager;
            Actor = Actor;
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
    }
}
