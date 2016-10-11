using System;
using System.Collections.Generic;

namespace Nostradamus.Core2
{
    public abstract class Scene : Actor
    {
        public Actor GetActor(ActorId id)
        {
            var snapshot = (SceneSnapshot)Snapshot;
            if (!snapshot.Actors.Contains(id))
                return null;

            return Context.GetActor(id);
        }

        public void AddActor(Actor actor)
        {
            var snapshot = (SceneSnapshot)Snapshot;

            if (!snapshot.Actors.Contains(actor.Desc.Id))
                snapshot.Actors.Add(actor.Desc.Id);
        }

        public void RemoveActor(Actor actor)
        {
            var snapshot = (SceneSnapshot)Snapshot;

            if (snapshot.Actors.Contains(actor.Desc.Id))
                snapshot.Actors.Remove(actor.Desc.Id);
        }

        public IEnumerable<Actor> Actors
        {
            get
            {
                var snapshot = (SceneSnapshot)Snapshot;

                foreach (var id in snapshot.Actors)
                {
                    var actor = Context.GetActor(id);
                    if (actor == null)
                        throw new InvalidOperationException();      // TODO: Message

                    yield return actor;
                }
            }
        }

        public Simulator Simulator
        {
            get { return (Simulator)Context.ActorManager; }
        }
        public new SceneDesc Desc
        {
            get { return (SceneDesc)base.Desc; }
        }
    }
}
