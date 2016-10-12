using System;

namespace Nostradamus
{
    public abstract class SceneDesc : ActorDesc
    {
        public readonly int SimulationDeltaTime;

        public readonly int ReconciliationDeltaTime;

        public readonly int ConvergenceTime;

        public readonly float ConvergenceRate;

        protected SceneDesc(ActorId id, int simulationDeltaTime, int reconciliationDeltaTime, int convergenceTime, float convergenceRate)
            : base(id)
        {
            if (simulationDeltaTime <= 0)
                throw new ArgumentOutOfRangeException(nameof(simulationDeltaTime));

            if (reconciliationDeltaTime <= 0)
                throw new ArgumentOutOfRangeException(nameof(reconciliationDeltaTime));

            if (convergenceTime <= 0)
                throw new ArgumentOutOfRangeException(nameof(convergenceTime));

            if (convergenceRate <= 0 || convergenceRate >= 1)
                throw new ArgumentOutOfRangeException(nameof(convergenceRate));

            SimulationDeltaTime = simulationDeltaTime;
            ReconciliationDeltaTime = reconciliationDeltaTime;
            ConvergenceTime = convergenceTime;
            ConvergenceRate = convergenceRate;
        }

        protected internal override ISnapshotArgs InitSnapshot()
        {
            return new SceneSnapshot();
        }
    }
}
