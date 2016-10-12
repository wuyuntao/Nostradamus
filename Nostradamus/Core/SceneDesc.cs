using System;

namespace Nostradamus
{
    public abstract class SceneDesc : ActorDesc
    {
        public readonly int SimulationDeltaTime;

        public readonly int ReconciliationDeltaTime;

        protected SceneDesc(ActorId id, int simulationDeltaTime, int reconciliationDeltaTime)
            : base(id)
        {
            if (simulationDeltaTime <= 0)
                throw new ArgumentOutOfRangeException(nameof(simulationDeltaTime));

            if (reconciliationDeltaTime <= 0)
                throw new ArgumentOutOfRangeException(nameof(reconciliationDeltaTime));

            SimulationDeltaTime = simulationDeltaTime;
            ReconciliationDeltaTime = reconciliationDeltaTime;
        }

        protected internal override ISnapshotArgs InitSnapshot()
        {
            return new SceneSnapshot();
        }
    }
}
