using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostradamus.Core2
{
    public class SceneSnapshot : ISnapshotArgs
    {
        public List<ActorId> ActiveActors;

        #region ISnapshotArgs

        ISnapshotArgs ISnapshotArgs.Clone()
        {
            throw new NotImplementedException();
        }

        ISnapshotArgs ISnapshotArgs.Extrapolate(int deltaTime)
        {
            throw new NotImplementedException();
        }

        ISnapshotArgs ISnapshotArgs.Interpolate(ISnapshotArgs snapshot, float factor)
        {
            throw new NotImplementedException();
        }

        bool ISnapshotArgs.IsApproximate(ISnapshotArgs snapshot)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
