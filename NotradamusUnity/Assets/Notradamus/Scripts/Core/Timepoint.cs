namespace Nostradamus
{
	class Timepoint
	{
		private readonly Timeline timeline;
		private readonly int time;
		private ISnapshotArgs snapshot;

		public Timepoint(Timeline timeline, int time, ISnapshotArgs snapshot)
		{
			this.timeline = timeline;
			this.time = time;
			this.snapshot = snapshot;
		}

		public Timeline Timeline
		{
			get { return timeline; }
		}

		public int Time
		{
			get { return time; }
		}

		public ISnapshotArgs Snapshot
		{
			get { return snapshot; }
		}
	}
}
