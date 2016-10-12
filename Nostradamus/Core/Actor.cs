using NLog;
using Nostradamus.Utils;
using System;

namespace Nostradamus
{
    public abstract class Actor : Disposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private ActorContext context;
        private ActorDesc desc;
        private ISnapshotArgs snapshot;

        internal Actor()
        { }

        internal virtual void Initialize(ActorContext context, ActorDesc desc)
        {
            this.context = context;
            this.desc = desc;
            this.snapshot = desc.InitSnapshot();
        }

        protected internal virtual void OnSnapshotRecovered(ISnapshotArgs snapshot)
        {
            Snapshot = snapshot;
        }

        protected internal virtual void OnUpdate()
        {
        }

        protected internal virtual void OnCommandReceived(ICommandArgs command)
        {
            throw new NotSupportedException(command.ToString());
        }

        internal void ApplyEvent(IEventArgs @event)
        {
            context.ActorManager.OnEventApplied(this, @event);

            OnEventApplied(@event);
        }

        protected virtual void OnEventApplied(IEventArgs @event)
        {
            throw new NotSupportedException(@event.ToString());
        }

        public ActorContext Context
        {
            get { return context; }
        }

        public ActorDesc Desc
        {
            get { return desc; }
        }

        public ISnapshotArgs Snapshot
        {
            get { return snapshot; }
            protected set { snapshot = value; }
        }
    }
}
