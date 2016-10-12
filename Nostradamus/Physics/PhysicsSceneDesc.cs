using BulletSharp.Math;
using Nostradamus.Physics;

namespace Nostradamus.Physics
{
    public abstract class PhysicsSceneDesc : SceneDesc
    {
        public static readonly Vector3 DefaultGravity = new Vector3(0, -9.81f, 0);

        public readonly Vector3 Gravity;

        public readonly SceneColliderDesc[] Colliders;

        public PhysicsSceneDesc(ActorId id, int simulationDeltaTime, int reconciliationDeltaTime, Vector3 gravity, SceneColliderDesc[] colliders)
            : base(id, simulationDeltaTime, reconciliationDeltaTime)
        {
            Gravity = gravity;
            Colliders = colliders;
        }
    }
}
