﻿using Nostradamus.Utils;
using System;
using System.Collections.Generic;

namespace Nostradamus
{
    public abstract class ActorManager : Disposable
    {
        protected Dictionary<ActorId, ActorContext> Actors { get; private set; }

        protected ActorManager()
        {
            Actors = new Dictionary<ActorId, ActorContext>();
        }

        protected override void DisposeManaged()
        {
            foreach (var context in Actors.Values)
                context.Actor.Dispose();

            Actors.Clear();

            base.DisposeManaged();
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