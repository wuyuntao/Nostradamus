using NLog;
using System;
using System.Collections.Generic;

namespace Nostradamus
{
    public abstract class Scene : Actor
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public SceneActor GetActor(ActorId id)
        {
            var snapshot = (SceneSnapshot)Snapshot;
            if (!snapshot.Actors.Contains(id))
                return null;

            return Context.GetActor(id) as SceneActor;
        }

        public void AddActor(SceneSnapshot snapshot, SceneActor actor)
        {
            if (!snapshot.Actors.Contains(actor.Desc.Id))
                snapshot.Actors.Add(actor.Desc.Id);
        }

        public void RemoveActor(SceneSnapshot snapshot, SceneActor actor)
        {
            if (snapshot.Actors.Contains(actor.Desc.Id))
                snapshot.Actors.Remove(actor.Desc.Id);
        }

        public IEnumerable<SceneActor> Actors
        {
            get
            {
                var snapshot = (SceneSnapshot)Snapshot;

                foreach (var id in snapshot.Actors)
                {
                    var actor = Context.GetActor(id) as SceneActor;
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
