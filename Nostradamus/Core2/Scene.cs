using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostradamus.Core2
{
    public abstract class Scene : Actor
    {
        protected Scene(SceneDesc desc)
            : base(desc)
        { }

        protected internal override void ApplyEvent(IEventArgs @event)
        {
            if (@event is ActorAddedEvent)
            {
                var desc = ((ActorAddedEvent)@event).ActorDesc;
                var actor = CreateActor(desc);

                var s = (SceneSnapshot)Snapshot.Clone();
                s.ActiveActors.Add(actor.Desc.Id);

                Snapshot = s;
            }
            else if (@event is ActorRemovedEvent)
            {
                var id = ((ActorRemovedEvent)@event).ActorId;

                var s = (SceneSnapshot)Snapshot.Clone();
                s.ActiveActors.Remove(id);

                Snapshot = s;
            }
            else
                base.ApplyEvent(@event);
        }

        public Actor GetActor(ActorId actorId)
        {
            throw new NotImplementedException();
        }

        protected abstract Actor CreateActor(ActorDesc actorDesc);

        public IEnumerable<Actor> Actors { get; }
    }
}
