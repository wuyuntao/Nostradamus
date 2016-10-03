using BulletSharp.Math;

namespace Nostradamus.Physics
{
	public class RigidBodyMovedEvent : IEventArgs
	{
		public Vector3 Position { get; set; }

		public Quaternion Rotation { get; set; }

		public Vector3 LinearVelocity { get; set; }

		public Vector3 AngularVelocity { get; set; }
	}
}
