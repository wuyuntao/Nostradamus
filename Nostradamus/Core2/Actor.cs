using Nostradamus.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostradamus.Core2
{
    public abstract class Actor : Disposable
    {
        public ActorDesc Desc { get; private set; }

        public ISnapshotArgs Snapshot { get; protected set; }

        protected Actor(ActorDesc desc)
        {
            this.Desc = desc;
            this.Snapshot = desc.CreateInitialSnapshot();
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
