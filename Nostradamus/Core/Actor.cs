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
		private ISnapshotArgs snapshot;
		private ActorContext context;

		protected Actor(Scene scene, ActorId id, ISnapshotArgs snapshot)
		{
			this.scene = scene;
			this.id = id;
			this.snapshot = snapshot;
			this.context = scene.CreateActorContext(this);
		}

		internal ISnapshotArgs CreateSnapshot()
		{
			return snapshot.Clone();
		}

		internal void RecoverSnapshot(ISnapshotArgs snapshot)
		{
			if (snapshot == null)
				throw new InvalidOperationException("Snapshot cannot be null");

			this.snapshot = snapshot;

			OnSnapshotRecovered(snapshot);
		}

		protected internal void ApplyEvent(IEventArgs @event)
		{
			var snapshot = OnEventApplied(@event);
			if (snapshot == null)
				throw new InvalidOperationException("Snapshot cannot be null");

			context.EnqueueEvent(@event);

			this.snapshot = snapshot;
		}

		protected abstract void OnSnapshotRecovered(ISnapshotArgs snapshot);

		protected internal abstract void OnCommandReceived(ICommandArgs command);

		protected abstract ISnapshotArgs OnEventApplied(IEventArgs @event);

		protected internal abstract void OnUpdate();

		protected internal Scene Scene
		{
			get { return scene; }
		}

		public ActorId Id
		{
			get { return id; }
		}

		public ISnapshotArgs Snapshot
		{
			get { return snapshot; }
		}
	}
}
