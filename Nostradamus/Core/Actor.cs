using NLog;
using System;

namespace Nostradamus
{
	public abstract class Actor
	{
		protected static Logger logger = LogManager.GetCurrentClassLogger();

		private readonly Scene scene;
		private readonly ActorId id;
		private readonly ClientId ownerId;
		private ISnapshotArgs snapshot;

		protected Actor(Scene scene, ActorId id, ClientId ownerId, ISnapshotArgs snapshot)
		{
			this.scene = scene;
			this.id = id;
			this.ownerId = ownerId;
			this.snapshot = snapshot;
		}

		internal protected virtual ISnapshotArgs CreateSnapshot()
		{
			return snapshot.Clone();
		}

		internal protected virtual void ApplyEvent(IEventArgs @event)
		{
			var snapshot = OnEventApplied(@event);
			if (snapshot == null)
				throw new InvalidOperationException("Snapshot cannot be null");

			this.snapshot = snapshot;
		}

		internal protected virtual void RollbackSnapshot(ISnapshotArgs snapshot)
		{
			if (snapshot == null)
				throw new InvalidOperationException("Snapshot cannot be null");

			this.snapshot = snapshot;
		}

		internal protected abstract void OnCommandReceived(ICommandArgs command);

		internal protected abstract ISnapshotArgs OnEventApplied(IEventArgs @event);

		internal protected abstract void OnUpdate();

		protected Scene Scene
		{
			get { return scene; }
		}

		protected ActorId Id
		{
			get { return id; }
		}

		protected ISnapshotArgs Snapshot
		{
			get { return snapshot; }
		}
	}
}
