using BulletSharp.Math;

namespace Nostradamus.Physics
{
	public class PhysicsSceneDesc
	{
		public Vector3 Gravity = new Vector3(0, -9.81f, 0);

		public SceneColliderDesc[] Colliders;
	}
}
