using NLog;
using Nostradamus.Utils;
using System;

namespace Nostradamus
{
    public abstract class Actor : Disposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Scene scene;
        private readonly ActorId id;
        private readonly ClientId ownerId;
        private ActorContext context;

        protected Actor(Scene scene, ActorId id, ClientId ownerId, ISnapshotArgs snapshot)
        {
            this.scene = scene;
            this.id = id;
            this.ownerId = ownerId;
            this.context = scene.CreateActorContext(this, snapshot);
        }

        public ISnapshotArgs InterpolateSnapshot(int time)
        {
            return context.InterpolateSnapshot(time);
        }

        protected internal void ApplyEvent(IEventArgs @event)
        {
            context.ApplyEvent(@event);
        }

        protected internal virtual void OnCommandReceived(ICommandArgs command)
        {
            throw new NotSupportedException(command.GetType().FullName);
        }

        protected internal virtual ISnapshotArgs OnEventApplied(IEventArgs @event)
        {
            throw new NotSupportedException(@event.GetType().FullName);
        }

        protected internal abstract void OnUpdate();

        public override string ToString()
        {
            if (string.IsNullOrEmpty(id.Description))
                return string.Format("{0} #{1}", GetType().Name, id.Value);
            else
                return string.Format("{0} #{1} ({2})", GetType().Name, id.Value, id.Description);
        }

        public Scene Scene
        {
            get { return scene; }
        }

        public ActorId Id
        {
            get { return id; }
        }

        public ClientId OwnerId
        {
            get { return ownerId; }
        }

        public ISnapshotArgs Snapshot
        {
            get { return context.ActorSnapshot; }
        }

        public ActorContext Context
        {
            get { return context; }
        }
    }
}
