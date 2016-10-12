using BulletSharp;
using BulletSharp.Math;

namespace Nostradamus.Physics
{
    public abstract class RigidBodyActorDesc : ActorDesc
    {
        public float Mass;

        public CollisionShape Shape;

        public Matrix CenterOfMassOffset;

        public Matrix StartTransform;

        public bool IsKinematic;
    }
}
