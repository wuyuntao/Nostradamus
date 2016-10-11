using BulletSharp.Math;
using Nostradamus.Physics;

namespace Nostradamus.Core2
{
    public sealed class PhysicsSceneDesc : SceneDesc
    {
        public Vector3 Gravity = new Vector3(0, -9.81f, 0);

        public SceneColliderDesc[] Colliders;
    }
}
