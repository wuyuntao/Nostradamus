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

        public readonly float LinearDamping;

        public readonly float AngularDamping;

        public readonly float Friction;

        public readonly float RollingFriction;

        public readonly float Restitution;

        protected RigidBodyDesc(ActorId id, float mass, CollisionShape shape, Matrix centerOfMassOffset, Matrix startTransform, bool isKinematic = false
                              , float friction = 0, float rollingFriction = 0, float restitution = 0
                              , float linearDamping = 0, float angularDamping = 0)
            : base(id)
        {
            if (mass <= 0)
                throw new ArgumentOutOfRangeException("mass");

            if (shape == null)
                throw new ArgumentNullException("shape");

            Mass = mass;
            Shape = shape;
            CenterOfMassOffset = centerOfMassOffset;
            StartTransform = startTransform;
            IsKinematic = isKinematic;
            LinearDamping = linearDamping;
            AngularDamping = angularDamping;
            Friction = friction;
            RollingFriction = rollingFriction;
            Restitution = restitution;
        }
    }
}
