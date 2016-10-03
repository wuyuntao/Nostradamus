using BulletSharp;
using BulletSharp.Math;

namespace Nostradamus.Physics
{
	public sealed class RigidBodyDesc
	{
		public float Mass;

		public CollisionShape Shape;

		public Matrix CenterOfMassOffset;

		public Matrix StartTransform;

		public bool IsKinematic;
	}
}
