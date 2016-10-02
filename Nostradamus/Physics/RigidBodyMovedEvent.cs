using BulletSharp.Math;
using ProtoBuf;

namespace Nostradamus.Physics
{
	[ProtoContract]
	public class RigidBodyMovedEvent : IEventArgs
	{
		[ProtoMember(1)]
		public float[] PositionValues { get; set; }

		[ProtoMember(2)]
		public float[] RotationValues { get; set; }

		[ProtoMember(3)]
		public float[] LinearVelocityValues { get; set; }

		[ProtoMember(4)]
		public float[] AngularVelocityValues { get; set; }

		public RigidBodyMovedEvent()
		{
			PositionValues = new float[3];
			RotationValues = new float[4];
			LinearVelocityValues = new float[3];
			AngularVelocityValues = new float[3];
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
