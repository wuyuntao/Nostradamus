using NLog;
using System;

namespace Nostradamus
{
	public abstract class ActorContext
	{
		protected static Logger logger = LogManager.GetCurrentClassLogger();

		private SceneContext sceneContext;
		private Actor actor;

		protected ActorContext(SceneContext sceneContext, Actor actor, int time, ISnapshotArgs snapshot)
		{
			if (this.actor != null)
				throw new InvalidOperationException("Already initialized");

			this.sceneContext = sceneContext;
			this.actor = actor;
		}

		internal virtual void Update()
		{
			if (actor == null)
				throw new InvalidOperationException("Not initialized yet");

			actor.OnUpdate();
		}

		internal abstract ISnapshotArgs CreateSnapshot(int time);

		public virtual void EnqueueCommand(int time, ICommandArgs command)
		{
			actor.CommandQueue.Enqueue(time, command);
		}

		internal abstract void ApplyEvent(IEventArgs @event);

		protected Actor Actor
		{
			get { return actor; }
		}

		protected SceneContext SceneContext
		{
			get { return sceneContext; }
		}
	}
}