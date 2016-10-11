using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostradamus.Core2
{
    public abstract class SceneDesc : ActorDesc
    {
        protected internal override ISnapshotArgs CreateInitialSnapshot()
        {
            return new SceneSnapshot() { ActiveActors = new List<ActorId>() { Id } };
        }
    }
}
