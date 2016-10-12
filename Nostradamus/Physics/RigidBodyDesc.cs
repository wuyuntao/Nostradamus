using BulletSharp;
using BulletSharp.Math;
using System;

namespace Nostradamus.Physics
{
    public abstract class RigidBodyDesc : SceneActorDesc
    {
        public readonly float Mass;

        public readonly CollisionShape Shape;

        public readonly Matrix CenterOfMassOffset;

        public readonly Matrix StartTransform;

        public readonly bool IsKinematic;

        protected RigidBodyDesc(ActorId id, float mass, CollisionShape shape, Matrix centerOfMassOffset, Matrix startTransform, bool isKinematic)
            : base(id)
        {
            if (mass <= 0)
                throw new ArgumentOutOfRangeException(nameof(mass));

            if (shape == null)
                throw new ArgumentNullException(nameof(shape));

            Mass = mass;
            Shape = shape;
            CenterOfMassOffset = centerOfMassOffset;
            StartTransform = startTransform;
            IsKinematic = isKinematic;
        }
    }
}
