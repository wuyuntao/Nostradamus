using NLog;
using System;

namespace Nostradamus
{
	public enum AuthorityType : byte
	{
		Authoritative,
		Predictive,
	}

	public abstract class Actor
	{
		protected static Logger logger = LogManager.GetCurrentClassLogger();

		private readonly Scene scene;
		private readonly ActorId id;
		private readonly ActorContext context;
		private readonly CommandQueue commandQueue;

		protected Actor(Scene scene, ActorId id, int time, ISnapshotArgs snapshot)
		{
			this.scene = scene;
			this.id = id;
			this.commandQueue = new CommandQueue(this);

			context = scene.Context.CreateActorContext(this, time, snapshot);
		}

		public ISnapshotArgs CreateSnapshot(int time)
		{
			return context.CreateSnapshot(time);
		}

		public void ApplyEvent(IEventArgs @event)
		{
			context.ApplyEvent(@event);
		}

		internal protected abstract void OnCommand(ISnapshotArgs snapshot, ICommandArgs command);

		internal protected abstract void OnEvent(ISnapshotArgs snapshot, IEventArgs @event);

		internal virtual void OnUpdate()
		{
		}

		internal protected Scene Scene
		{
			get { return scene; }
		}

		internal protected ActorId Id
		{
			get { return id; }
		}

		internal ActorContext Context
		{
			get { return context; }
		}

		internal CommandQueue CommandQueue
		{
			get { return commandQueue; }
		}
	}
}
