using NLog;
using Nostradamus.Client;
using Nostradamus.Server;
using Nostradamus.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nostradamus
{
    public abstract class Scene : Disposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly SceneDesc desc;
        private readonly SceneContext context;
        private readonly Dictionary<ActorId, ActorContext> actors = new Dictionary<ActorId, ActorContext>();

        public Scene(SceneDesc desc)
        {
            this.desc = desc;

            switch (desc.Mode)
            {
                case SceneMode.Client:
                    context = new ClientSceneContext(this, desc);
                    break;

                case SceneMode.Server:
                    context = new ServerSceneContext(this, desc);
                    break;

                default:
                    throw new NotSupportedException(desc.Mode.ToString());
            }
        }

        protected override void DisposeManaged()
        {
            foreach (var context in actors.Values)
                context.Actor.Dispose();

            actors.Clear();

            base.DisposeManaged();
        }

        internal ActorContext CreateActorContext(Actor actor, ISnapshotArgs snapshot)
        {
            var context = this.context.CreateActorContext(actor, snapshot);

            actors.Add(actor.Id, context);

            return context;
        }

        internal ActorContext CreateActorContext(ActorId actorId, ISnapshotArgs snapshot)
        {
            var actor = CreateActor(actorId, snapshot);

            return actor.Context;
        }

        internal ActorContext GetActorContext(ActorId actorId)
        {
            ActorContext actor;
            actors.TryGetValue(actorId, out actor);
            return actor;
        }

        protected abstract Actor CreateActor(ActorId actorId, ISnapshotArgs snapshot);

        internal void Update(int time, int deltaTime)
        {
            Time = time;
            DeltaTime = deltaTime;

            foreach (var context in actors.Values)
                context.Update();

            OnUpdate();
        }

        protected virtual void OnUpdate()
        { }

        public SceneDesc Desc
        {
            get { return desc; }
        }

        public SceneContext Context
        {
            get { return context; }
        }

        public IEnumerable<Actor> Actors
        {
            get
            {
                return from context in actors.Values
                       select context.Actor;
            }
        }

        internal IEnumerable<ActorContext> ActorContexts
        {
            get { return actors.Values; }
        }

        public int Time { get; internal set; }

        public int DeltaTime { get; internal set; }
    }
}
