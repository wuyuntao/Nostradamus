using FlatBuffers;
using FlatBuffers.Schema;
using Nostradamus.Networking;
using System.Collections.Generic;
using System.Linq;

namespace Nostradamus
{
    public sealed class SimulatorSnapshot : ISnapshotArgs
    {
        public List<ActorSnapshot> Actors = new List<ActorSnapshot>();

        #region ISnapshotArgs

        ISnapshotArgs ISnapshotArgs.Clone()
        {
            return new SimulatorSnapshot()
            {
                Actors = new List<ActorSnapshot>(from a in Actors
                                                 select new ActorSnapshot(a.Desc, a.Args.Clone()))
            };
        }

        ISnapshotArgs ISnapshotArgs.Extrapolate(int deltaTime)
        {
            return new SimulatorSnapshot()
            {
                Actors = new List<ActorSnapshot>(from a in Actors
                                                 select new ActorSnapshot(a.Desc, a.Args.Extrapolate(deltaTime)))
            };
        }

        ISnapshotArgs ISnapshotArgs.Interpolate(ISnapshotArgs snapshot, float factor)
        {
            var otherSnapshot = (SimulatorSnapshot)snapshot;
            var newSnapshot = new SimulatorSnapshot();

            foreach (var selfActor in Actors)
            {
                var otherActor = otherSnapshot.Actors.Find(a => a.Desc.Id == selfActor.Desc.Id);
                if (otherActor != null)
                {
                    var newActor = selfActor.Args.Interpolate(otherActor.Args, factor);
                    newSnapshot.Actors.Add(new ActorSnapshot(selfActor.Desc, newActor));
                }
                else
                {
                    newSnapshot.Actors.Add(new ActorSnapshot(selfActor.Desc, selfActor.Args.Clone()));
                }
            }

            return newSnapshot;
        }

        bool ISnapshotArgs.IsApproximate(ISnapshotArgs snapshot)
        {
            var otherSnapshot = (SimulatorSnapshot)snapshot;

            if (Actors.Count != otherSnapshot.Actors.Count)
                return false;

            Actors.Sort(SortByActorId);
            otherSnapshot.Actors.Sort(SortByActorId);

            for (int i = 0; i < Actors.Count; i++)
            {
                var selfActor = Actors[i];
                var otherActor = otherSnapshot.Actors[i];

                if (selfActor.Desc.Id != otherActor.Desc.Id)
                    return false;

                if (!selfActor.Args.IsApproximate(otherActor.Args))
                    return false;
            }

            return true;
        }

        private int SortByActorId(ActorSnapshot x, ActorSnapshot y)
        {
            return x.Desc.Id.CompareTo(y.Desc.Id);
        }

        #endregion
    }

    class SimulatorSnapshotSerializer : Serializer<SimulatorSnapshot, Schema.SimulatorSnapshot>
    {
        public static readonly SimulatorSnapshotSerializer Instance = new SimulatorSnapshotSerializer();

        public override Offset<Schema.SimulatorSnapshot> Serialize(FlatBufferBuilder fbb, SimulatorSnapshot snapshot)
        {
            var oActors = new Offset<Schema.ActorSnapshot>[snapshot.Actors.Count];
            for (int i = 0; i < snapshot.Actors.Count; i++)
            {
                var oActor = ActorSnapshotSerializer.Instance.Serialize(fbb, snapshot.Actors[i]);

                oActors[i] = oActor;
            }
            var voActors = Schema.SimulatorSnapshot.CreateActorsVector(fbb, oActors);

            return Schema.SimulatorSnapshot.CreateSimulatorSnapshot(fbb, voActors);
        }

        public override SimulatorSnapshot Deserialize(Schema.SimulatorSnapshot snapshot)
        {
            var actors = new List<ActorSnapshot>();

            for (int i = 0; i < snapshot.ActorsLength; i++)
            {
                var actor = snapshot.Actors(i).Value;

                actors.Add(ActorSnapshotSerializer.Instance.Deserialize(actor));
            }

            return new SimulatorSnapshot() { Actors = actors };
        }

        protected override Schema.SimulatorSnapshot GetRootAs(ByteBuffer buffer)
        {
            return Schema.SimulatorSnapshot.GetRootAsSimulatorSnapshot(buffer);
        }
    }
}
