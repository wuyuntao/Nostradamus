using NLog;
using Nostradamus.Client;
using Nostradamus.Server;
using Nostradamus.Utils;
using System;

namespace Nostradamus
{
    public abstract class SceneContext : Disposable
    {
        protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Scene scene;
        private readonly SceneDesc sceneDesc;
        private int time;

        protected SceneContext(Scene scene, SceneDesc desc)
        {
            this.scene = scene;
            this.sceneDesc = desc;
        }

        protected override void DisposeManaged()
        {
            scene.Dispose();

            base.DisposeManaged();
        }

        internal ActorContext CreateActorContext(Actor actor, ISnapshotArgs snapshot)
        {
            switch (sceneDesc.Mode)
            {
                case SceneMode.Client:
                    return new ClientActorContext(actor, snapshot);

                case SceneMode.Server:
                    return new ServerActorContext(actor, snapshot);

                default:
                    throw new NotSupportedException(sceneDesc.Mode.ToString());
            }
        }

        public virtual void Simulate()
        {
            time += sceneDesc.SimulationDeltaTime;
        }

        protected Scene Scene
        {
            get { return scene; }
        }

        protected SceneDesc SceneDesc
        {
            get { return sceneDesc; }
        }

        public int Time
        {
            get { return time; }
            protected set
            {
                if (value <= time)
                    throw new ArgumentOutOfRangeException("time");

                time = value;
            }
        }
    }
}