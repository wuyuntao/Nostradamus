using NLog;
using Nostradamus.Utils;
using System;

namespace Nostradamus
{
	public abstract class Simulator : Disposable
	{
		protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

		private Scene scene;
		private int time;

		protected Simulator()
		{ }

		protected override void DisposeManaged()
		{
			SafeDispose(ref scene);

			base.DisposeManaged();
		}

		internal void InitializeScene(Scene scene)
		{
			if (scene == null)
				throw new ArgumentNullException("scene");

			if (this.scene != null)
				throw new InvalidOperationException("Cannot create multiple scene");

			this.scene = scene;
		}

		internal abstract ActorContext CreateActorContext(Actor actor, ISnapshotArgs snapshot);

		protected Scene Scene
		{
			get { return scene; }
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