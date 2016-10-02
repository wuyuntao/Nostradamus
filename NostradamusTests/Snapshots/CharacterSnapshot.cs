using ProtoBuf;

namespace Nostradamus.Tests.Snapshots
{
	[ProtoContract]
	class CharacterSnapshot : ISnapshotArgs
	{
		[ProtoMember(1)]
		public float PositionX { get; set; }

		[ProtoMember(2)]
		public float PositionY { get; set; }

		ISnapshotArgs ISnapshotArgs.Clone()
		{
			return new CharacterSnapshot() { PositionX = PositionX, PositionY = PositionY };
		}

		ISnapshotArgs ISnapshotArgs.Interpolate(ISnapshotArgs snapshot, float factor)
		{
			var s = (CharacterSnapshot)snapshot;

			return new CharacterSnapshot()
			{
				PositionX = PositionX + (s.PositionX - PositionX) * factor,
				PositionY = PositionY + (s.PositionY - PositionY) * factor,
			};
		}

		ISnapshotArgs ISnapshotArgs.Extrapolate(int deltaTime)
		{
			return new CharacterSnapshot() { PositionX = PositionX, PositionY = PositionY };
		}

		bool ISnapshotArgs.IsApproximate(ISnapshotArgs snapshot)
		{
			var s = (CharacterSnapshot)snapshot;

			var distanceSqrt = (PositionX - s.PositionX) * (PositionX - s.PositionX) + (PositionY - s.PositionY) * (PositionY - s.PositionY);

			return distanceSqrt < 0.01f * 0.01f;
		}
	}
}
