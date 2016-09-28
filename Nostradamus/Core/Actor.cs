using NLog;
using System;

namespace Nostradamus
{
	public abstract class Actor
	{
		protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

		private readonly Scene scene;
		private readonly ActorId id;
		private readonly ClientId ownerId;
		private ISnapshotArgs snapshot;
		private int lastCommandSeq;

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

			scene.CreateEvent(this, lastCommandSeq, @event);

			this.snapshot = snapshot;
		}

		internal protected virtual void SetSnapshot(ISnapshotArgs snapshot)
		{
			if (snapshot == null)
				throw new InvalidOperationException("Snapshot cannot be null");

			this.snapshot = snapshot;
		}

		internal void ReceiveCommand(Command command)
		{
			lastCommandSeq = command.Sequence;

			OnCommandReceived(command.Args);

			lastCommandSeq = 0;
		}

		internal protected abstract void OnCommandReceived(ICommandArgs command);

		internal protected abstract ISnapshotArgs OnEventApplied(IEventArgs @event);

		internal protected abstract void OnUpdate();

		internal protected Scene Scene
		{
			get { return scene; }
		}

		internal protected ActorId Id
		{
			get { return id; }
		}

		internal protected ClientId OwnerId
		{
			get { return ownerId; }
		}

		internal protected ISnapshotArgs Snapshot
		{
			get { return snapshot; }
		}
	}
}
