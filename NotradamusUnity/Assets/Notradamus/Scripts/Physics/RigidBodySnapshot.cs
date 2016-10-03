using BulletSharp.Math;
using System;

namespace Nostradamus.Physics
{
	public class RigidBodySnapshot : ISnapshotArgs
	{
		public Vector3 Position { get; set; }

		public Quaternion Rotation { get; set; }

		public Vector3 LinearVelocity { get; set; }

		public Vector3 AngularVelocity { get; set; }

		ISnapshotArgs ISnapshotArgs.Clone()
		{
			return new RigidBodySnapshot()
			{
				Position = Position,
				Rotation = Rotation,
				LinearVelocity = LinearVelocity,
				AngularVelocity = AngularVelocity,
			};
		}

		ISnapshotArgs ISnapshotArgs.Extrapolate(int deltaTime)
		{
			var position = Position + LinearVelocity * deltaTime / 1000f;

			var angle = AngularVelocity.Length * deltaTime / 1000f;
			var axis = Vector3.Normalize(AngularVelocity);
			var rotation = Rotation * Quaternion.RotationAxis(AngularVelocity, angle);

			return new RigidBodySnapshot()
			{
				Position = position,
				Rotation = rotation,
				LinearVelocity = LinearVelocity,
				AngularVelocity = AngularVelocity,
			};
		}

		ISnapshotArgs ISnapshotArgs.Interpolate(ISnapshotArgs snapshot, float factor)
		{
			var s = (RigidBodySnapshot)snapshot;

			return new RigidBodySnapshot()
			{
				Position = Vector3.Lerp(Position, s.Position, factor),
				Rotation = Quaternion.Slerp(Rotation, s.Rotation, factor),
				// TODO: Is it meaningful to get interpolated velocities?
				LinearVelocity = Vector3.Lerp(LinearVelocity, s.LinearVelocity, factor),
				AngularVelocity = Vector3.Lerp(AngularVelocity, s.AngularVelocity, factor),
			};
		}

		bool ISnapshotArgs.IsApproximate(ISnapshotArgs snapshot)
		{
			var s = (RigidBodySnapshot)snapshot;

			// TODO: Allow custom configuration of error
			var distanceSquared = Vector3.DistanceSquared(Position, s.Position);
			if (distanceSquared > 0.01f * 0.01f)
				return false;

			var angle = (Rotation * Quaternion.Invert(s.Rotation)).Angle;
			if (angle > 1 / 180f * Math.PI)
				return false;

			return true;
		}
	}
}
