using System;
using System.Collections.Generic;
using System.Linq;

namespace Nostradamus
{
    public abstract class Simulator : ActorManager
    {
        private Scene scene;

        public TScene CreateScene<TScene>(SceneDesc desc)
            where TScene : Scene, new()
        {
            if (this.scene != null)
                throw new InvalidOperationException();  // TODO: Message

            var scene = CreateActor<TScene>(desc);
            this.scene = scene;

            return scene;
        }

        protected void Simulate(IEnumerable<Command> commands)
        {
            foreach (var command in commands)
            {
                var actor = Scene.GetActor(command.ActorId);

                actor.OnCommandReceived(command.Args);
            }

            foreach (var actor in Scene.Actors)
            {
                actor.OnUpdate();
            }

            Scene.OnUpdate();
        }

        protected void ApplyEvents(IEnumerable<Event> events)
        {
            foreach (var e in events)
            {
                var actor = Scene.GetActor(e.ActorId);

                actor.ApplyEvent(e.Args);
            }
        }

        protected SimulatorSnapshot CreateSnapshot()
        {
            return new SimulatorSnapshot()
            {
                Actors = new List<Snapshot>(from a in Scene.Actors
                                            select new Snapshot(a.Desc.Id, a.Snapshot))
            };
        }

        protected void RecoverSnapshot(SimulatorSnapshot snapshot)
        {
            Scene.OnSnapshotRecovered(new SceneSnapshot()
            {
                Actors = new List<ActorId>(from actorSnapshot in snapshot.Actors
                                           select actorSnapshot.ActorId)
            });

            foreach (var actorSnapshot in snapshot.Actors)
            {
                var actor = GetActor(actorSnapshot.ActorId);

                actor.OnSnapshotRecovered(actorSnapshot.Args);
            }
        }

        public Scene Scene
        {
            get
            {
                if (scene == null)
                    throw new InvalidOperationException();          // TODO: Message

                return scene;
            }
        }
    }
}
