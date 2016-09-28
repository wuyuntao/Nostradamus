using NLog;
using Nostradamus.Utils;
using System;

namespace Nostradamus
{
	public abstract class Actor : Disposable
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

		protected internal virtual ISnapshotArgs CreateSnapshot()
		{
			return snapshot.Clone();
		}

		protected internal virtual void ApplyEvent(IEventArgs @event)
		{
			var snapshot = OnEventApplied(@event);
			if (snapshot == null)
				throw new InvalidOperationException("Snapshot cannot be null");

			scene.CreateEvent(this, lastCommandSeq, @event);

			this.snapshot = snapshot;
		}

		protected internal virtual void SetSnapshot(ISnapshotArgs snapshot)
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

		protected internal abstract void OnCommandReceived(ICommandArgs command);

		protected internal abstract ISnapshotArgs OnEventApplied(IEventArgs @event);

		protected internal abstract void OnUpdate();

		protected internal Scene Scene
		{
			get { return scene; }
		}

		protected internal ActorId Id
		{
			get { return id; }
		}

		protected internal ClientId OwnerId
		{
			get { return ownerId; }
		}

		protected internal ISnapshotArgs Snapshot
		{
			get { return snapshot; }
		}
	}
}
