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
        private bool isSimulating;

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

        public void Simulate()
        {
            isSimulating = true;

            OnSimulate();

            logger.Debug("Simulate done. {0}ms", Time);

            time += sceneDesc.SimulationDeltaTime;
            isSimulating = false;
        }

        protected abstract void OnSimulate();

        internal void ThrowUnlessSimulating()
        {
            if (!isSimulating)
                throw new InvalidOperationException("Is not simulating");
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