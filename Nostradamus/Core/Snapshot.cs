namespace Nostradamus
{
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

	public interface ISnapshotArgs
	{
	}
}
