using System;
using System.Collections.Generic;

namespace Nostradamus.Core2
{
    public sealed class SimulatorSnapshot : ISnapshotArgs
    {
        public List<Snapshot> Actors = new List<Snapshot>();

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
