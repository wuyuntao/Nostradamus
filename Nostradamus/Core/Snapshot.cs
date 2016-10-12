
namespace Nostradamus
{
    public interface ISnapshotArgs
    {
        ISnapshotArgs Clone();

        ISnapshotArgs Interpolate(ISnapshotArgs snapshot, float factor);

        ISnapshotArgs Extrapolate(int deltaTime);

        bool IsApproximate(ISnapshotArgs snapshot);
    }

    public sealed class Snapshot
    {
        public readonly ActorId ActorId;

        public readonly ISnapshotArgs Args;

        public Snapshot(ActorId actorId, ISnapshotArgs args)
        {
            ActorId = actorId;
            Args = args;
        }
    }
}
