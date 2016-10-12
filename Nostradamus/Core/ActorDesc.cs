using System;

namespace Nostradamus
{
    public abstract class ActorDesc
    {
        public readonly ActorId Id;

        protected ActorDesc(ActorId id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            Id = id;
        }

        protected internal abstract ISnapshotArgs InitSnapshot();
    }
}
