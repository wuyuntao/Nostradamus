using NLog;
using System.Collections.Generic;
using System;

namespace Nostradamus
{
    public class Scene
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        private SceneContext context;
        private int maxActorId;
        private readonly Dictionary<ActorId, Actor> actors = new Dictionary<ActorId, Actor>();
        private int time;
        private int deltaTime;

        public ActorId CreateActorId(string description = null)
        {
            return new ActorId(++maxActorId, description);
        }

        internal Actor GetActor(ActorId actorId)
        {
            Actor actor;
            actors.TryGetValue(actorId, out actor);
            return actor;
        }

        public void AddActor(Actor actor)
        {
            actors.Add(actor.Id, actor);
        }

        public void RemoveActor(Actor actor)
        {
            actors.Remove(actor.Id);
        }

        internal protected virtual void OnUpdate(int deltaTime)
        {
            this.time += this.deltaTime;
            this.deltaTime = deltaTime;
        }

        internal protected virtual void OnLateUpdate()
        {
            time += deltaTime;
        }

        public int Time
        {
            get { return time; }
        }

        public int DeltaTime
        {
            get { return deltaTime; }
        }

        public IEnumerable<Actor> Actors
        {
            get { return actors.Values; }
        }

        internal SceneContext Context
        {
            get { return context; }
            set { context = value; }
        }
    }
}
