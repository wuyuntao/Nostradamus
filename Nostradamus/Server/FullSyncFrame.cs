using System.Collections.Generic;

namespace Nostradamus.Server
{
	public sealed class FullSyncFrame
	{
		public int Time { get; set; }

		public int DeltaTime { get; set; }

		public SimulatorSnapshot Snapshot { get; set; }

		public FullSyncFrame(int time, int deltaTime)
		{
			Time = time;
			DeltaTime = deltaTime;
		}
	}
}