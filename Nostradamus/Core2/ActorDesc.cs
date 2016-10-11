using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostradamus.Core2
{
    public abstract class ActorDesc
    {
        public ActorId Id;

        protected internal abstract ISnapshotArgs CreateInitialSnapshot();
    }
}
