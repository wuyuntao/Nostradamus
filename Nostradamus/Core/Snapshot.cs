
namespace Nostradamus
{
    public interface ISnapshotArgs
    {
        ISnapshotArgs Clone();

        ISnapshotArgs Interpolate(ISnapshotArgs snapshot, float factor);

        ISnapshotArgs Extrapolate(int deltaTime);

        bool IsApproximate(ISnapshotArgs snapshot);
    }

    public sealed class ActorSnapshot
    {
        public readonly ActorDesc Desc;

        public readonly ISnapshotArgs Args;

        public ActorSnapshot(ActorDesc actorDesc, ISnapshotArgs args)
        {
            Desc = actorDesc;
            Args = args;
        }
    }
}
