using System;
using NLog;

namespace Nostradamus
{
    public abstract class SceneContext
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        private Scene scene;

        protected SceneContext(Scene scene)
        {
            this.scene = scene;

            scene.Context = this;
        }

		internal abstract ActorContext CreateActorContext(Actor actor, int time, ISnapshotArgs snapshot);

		protected Scene Scene
        {
            get { return scene; }
        }
	}
}