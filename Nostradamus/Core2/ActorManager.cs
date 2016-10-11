using System;
using System.Collections.Generic;

namespace Nostradamus.Core2
{
    public abstract class ActorManager
    {
        protected Dictionary<ActorId, ActorContext> Actors { get; private set; }

        protected ActorManager()
        {
            Actors = new Dictionary<ActorId, ActorContext>();
        }

        public TActor CreateActor<TActor>(ActorDesc desc)
            where TActor : Actor, new()
        {
            var actor = new TActor();
            var context = new ActorContext(this, actor);
            actor.Initialize(context, desc);

            Actors.Add(desc.Id, context);

            return actor;
        }

        public ActorContext GetActorContext(ActorId id)
        {
            ActorContext context;
            Actors.TryGetValue(id, out context);
            return context;
        }

        public Actor GetActor(ActorId id)
        {
            ActorContext context;
            if (Actors.TryGetValue(id, out context))
                return context.Actor;
            else
                return null;
        }
    }
}
