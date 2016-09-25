namespace Nostradamus
{
	public interface ISnapshotArgs
	{
		ISnapshotArgs Clone();

		ISnapshotArgs Interpolate(ISnapshotArgs snapshot, float factor);

		ISnapshotArgs Extrapolate(int deltaTime);
	}

	class Snapshot
	{
		public readonly int Time;
		public readonly ISnapshotArgs Args;

		public Snapshot(int time, ISnapshotArgs args)
		{
			Time = time;
			Args = args;
		}
	}
}
