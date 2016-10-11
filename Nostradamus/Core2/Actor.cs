using Nostradamus.Utils;
using System;

namespace Nostradamus.Core2
{
    public abstract class Actor : Disposable
    {
        public ActorContext Context { get; private set; }

        public ActorDesc Desc { get; private set; }

        public ISnapshotArgs Snapshot { get; protected set; }

        internal Actor()
        { }

        internal virtual void Initialize(ActorContext context, ActorDesc desc)
        {
            Context = context;
            Desc = desc;
            Snapshot = desc.InitSnapshot();
        }

        protected internal virtual void RecoverSnapshot(ISnapshotArgs snapshot)
        {
            Snapshot = snapshot;
        }

        protected internal virtual void Update()
        {
        }

        protected internal virtual void ReceiveCommand(ICommandArgs command)
        {
            throw new NotSupportedException(command.ToString());
        }

        protected internal virtual void ApplyEvent(IEventArgs @event)
        {
            throw new NotSupportedException(@event.ToString());
        }
    }
}
