using BulletSharp.Math;

namespace Nostradamus.Physics
{
	public class RigidBodyMovedEvent : IEventArgs
	{
		public Vector3 Position;

		public Quaternion Rotation;

		public Vector3 LinearVelocity;

		public Vector3 AngularVelocity;
	}
}
