using NLog;
using System;

namespace Nostradamus
{
	public abstract class ActorContext
	{
		protected static Logger logger = LogManager.GetCurrentClassLogger();

		private Actor actor;

		internal virtual void Initialize(Actor actor, int time, ISnapshotArgs snapshot)
		{
			if( this.actor != null )
				throw new InvalidOperationException( "Already initialized" );

			this.actor = actor;
		}

		internal virtual void Update()
		{
			if( actor == null )
				throw new InvalidOperationException( "Not initialized yet" );

			actor.OnUpdate();
		}

		internal abstract ISnapshotArgs CreateSnapshot(int time);

		public virtual void EnqueueCommand(int time, ICommandArgs command)
		{
			actor.CommandQueue.Enqueue( time, command );
		}

		internal abstract void ApplyEvent(IEventArgs @event);

		protected Actor Actor
		{
			get { return actor; }
		}
	}
}