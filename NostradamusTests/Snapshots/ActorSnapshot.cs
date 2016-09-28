using System;

namespace Nostradamus.Tests.Snapshots
{
	class ActorSnapshot : ISnapshotArgs
	{
		public float PositionX;
		public float PositionY;

		ISnapshotArgs ISnapshotArgs.Clone()
		{
			return new ActorSnapshot() { PositionX = PositionX, PositionY = PositionY };
		}

		ISnapshotArgs ISnapshotArgs.Interpolate(ISnapshotArgs snapshot, float factor)
		{
			var s = (ActorSnapshot)snapshot;

			return new ActorSnapshot()
			{
				PositionX = PositionX + (s.PositionX - PositionX) * factor,
				PositionY = PositionY + (s.PositionY - PositionY) * factor,
			};
		}

		ISnapshotArgs ISnapshotArgs.Extrapolate(int deltaTime)
		{
			return new ActorSnapshot() { PositionX = PositionX, PositionY = PositionY };
		}

		bool ISnapshotArgs.IsApproximate(ISnapshotArgs snapshot)
		{
			var s = (ActorSnapshot)snapshot;

			var distanceSqrt = (PositionX - s.PositionX) * (PositionX - s.PositionX) + (PositionY - s.PositionY) * (PositionY - s.PositionY);

			return distanceSqrt < 0.01f * 0.01f;
		}
	}
}
