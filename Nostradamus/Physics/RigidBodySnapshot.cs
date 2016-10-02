using BulletSharp.Math;
using ProtoBuf;
using System;

namespace Nostradamus.Physics
{
	[ProtoContract]
	public class RigidBodySnapshot : ISnapshotArgs
	{
		[ProtoMember(1)]
		public float[] PositionValues { get; set; }

		[ProtoMember(2)]
		public float[] RotationValues { get; set; }

		[ProtoMember(3)]
		public float[] LinearVelocityValues { get; set; }

		[ProtoMember(4)]
		public float[] AngularVelocityValues { get; set; }

		public RigidBodySnapshot()
		{
			PositionValues = new float[3];
			RotationValues = new float[4];
			LinearVelocityValues = new float[3];
			AngularVelocityValues = new float[3];
		}

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

		public Vector3 Position
		{
			get
			{
				return new Vector3(PositionValues[0], PositionValues[1], PositionValues[2]);
			}
			set
			{
				PositionValues[0] = value.X;
				PositionValues[1] = value.Y;
				PositionValues[2] = value.Z;
			}
		}

		public Quaternion Rotation
		{
			get
			{
				return new Quaternion(RotationValues[0], RotationValues[1], RotationValues[2], RotationValues[3]);
			}
			set
			{
				RotationValues[0] = value.X;
				RotationValues[1] = value.Y;
				RotationValues[2] = value.Z;
				RotationValues[3] = value.W;
			}
		}

		public Vector3 LinearVelocity
		{
			get
			{
				return new Vector3(LinearVelocityValues[0], LinearVelocityValues[1], LinearVelocityValues[2]);
			}
			set
			{
				LinearVelocityValues[0] = value.X;
				LinearVelocityValues[1] = value.Y;
				LinearVelocityValues[2] = value.Z;
			}
		}

		public Vector3 AngularVelocity
		{
			get
			{
				return new Vector3(AngularVelocityValues[0], AngularVelocityValues[1], AngularVelocityValues[2]);
			}
			set
			{
				AngularVelocityValues[0] = value.X;
				AngularVelocityValues[1] = value.Y;
				AngularVelocityValues[2] = value.Z;
			}
		}
	}
}
