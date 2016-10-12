using System;
using System.Collections.Generic;

namespace Nostradamus
{
    public class SceneSnapshot : ISnapshotArgs
    {
        public List<ActorId> Actors = new List<ActorId>();

        #region ISnapshotArgs

        ISnapshotArgs ISnapshotArgs.Clone()
        {
            return new SceneSnapshot() { Actors = new List<ActorId>(Actors) };
        }

        ISnapshotArgs ISnapshotArgs.Extrapolate(int deltaTime)
        {
            return ((ISnapshotArgs)this).Clone();
        }

        ISnapshotArgs ISnapshotArgs.Interpolate(ISnapshotArgs snapshot, float factor)
        {
            if (factor >= 1)
                return snapshot.Clone();
            else
                return ((ISnapshotArgs)this).Clone();
        }

        bool ISnapshotArgs.IsApproximate(ISnapshotArgs snapshot)
        {
            var otherActors = ((SceneSnapshot)snapshot).Actors;

            if (Actors.Count != otherActors.Count)
                return false;

            Actors.Sort();
            otherActors.Sort();

            for (int i = 0; i < Actors.Count; i++)
            {
                if (Actors[i] != otherActors[i])
                    return false;
            }

            return true;
        }

        #endregion
    }
}
