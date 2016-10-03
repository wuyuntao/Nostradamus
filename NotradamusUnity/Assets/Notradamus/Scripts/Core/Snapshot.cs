
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
		public ActorId ActorId { get; set; }

		public ISnapshotArgs Args { get; set; }

		public Snapshot(ActorId actorId, ISnapshotArgs args)
		{
			ActorId = actorId;
			Args = args;
		}
	}
}
