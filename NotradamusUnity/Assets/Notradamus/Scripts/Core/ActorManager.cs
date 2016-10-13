using Nostradamus.Utils;
using System;
using System.Collections.Generic;

namespace Nostradamus
{
    public delegate void EventAppliedEventHandler(Actor actor, IEventArgs @event);

    public abstract class ActorManager : Disposable
    {
        private delegate Actor ActorFactoryMethod(ActorDesc desc);

        public event EventAppliedEventHandler EventApplied;

        private Dictionary<Type, ActorFactoryMethod> actorFactories = new Dictionary<Type, ActorFactoryMethod>();
        private Dictionary<ActorId, ActorContext> actors = new Dictionary<ActorId, ActorContext>();

        protected ActorManager()
        { }

        protected override void DisposeManaged()
        {
            foreach (var context in actors.Values)
                context.Actor.Dispose();

            actors.Clear();

            base.DisposeManaged();
        }

        public void RegisterActorFactory<TActorDesc, TActor>(Func<TActorDesc, TActor> factory)
            where TActorDesc : ActorDesc
            where TActor : Actor
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            var type = typeof(TActorDesc);
            var factoryMethod = new ActorFactoryMethod(desc => factory((TActorDesc)desc));

            actorFactories.Add(type, factoryMethod);
        }

        public Actor CreateActor(ActorDesc desc)
        {
            ActorFactoryMethod factory;
            if (!actorFactories.TryGetValue(desc.GetType(), out factory))
                throw new InvalidOperationException();      // TODO: Message

            var actor = factory(desc);
            var context = new ActorContext(this, actor);
            actor.Initialize(context, desc);

            actors.Add(desc.Id, context);

            return actor;
        }

        public TActor CreateActor<TActor>(ActorDesc desc)
            where TActor : Actor, new()
        {
            return (TActor)CreateActor(desc);
        }

        public Actor GetActor(ActorId id)
        {
            ActorContext context;
            if (actors.TryGetValue(id, out context))
                return context.Actor;
            else
                return null;
        }

        internal void OnEventApplied(Actor actor, IEventArgs @event)
        {
            if (EventApplied != null)
                EventApplied(actor, @event);
        }
    }
}
