﻿using BulletSharp;
using BulletSharp.Math;
using NLog;

namespace Nostradamus.Physics
{
    public abstract class RigidBodyActor : SceneActor
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private RigidBody rigidBody;

        internal override void Initialize(ActorContext context, ActorDesc actorDesc)
        {
            base.Initialize(context, actorDesc);

            var desc = (RigidBodyDesc)actorDesc;
            var snapshot = (RigidBodySnapshot)Snapshot;

            var localInertia = Vector3.Zero;
            if (!desc.IsKinematic)
                desc.Shape.CalculateLocalInertia(desc.Mass, out localInertia);

            var motionState = new DefaultMotionState(desc.StartTransform, desc.CenterOfMassOffset);

            using (var rbInfo = new RigidBodyConstructionInfo(desc.Mass, motionState, desc.Shape, localInertia))
            {
                rigidBody = new RigidBody(rbInfo);
                rigidBody.UserObject = this;

                if (desc.IsKinematic)
                    rigidBody.CollisionFlags |= CollisionFlags.KinematicObject;

                Scene.World.AddRigidBody(rigidBody);
            }
        }

        protected override void DisposeManaged()
        {
            Scene.World.RemoveRigidBody(rigidBody);
            SafeDispose(ref rigidBody);

            base.DisposeManaged();
        }

        protected internal override void OnSnapshotRecovered(ISnapshotArgs snapshot)
        {
            base.OnSnapshotRecovered(snapshot);

            var s = (RigidBodySnapshot)snapshot;
            rigidBody.CenterOfMassTransform = Matrix.RotationQuaternion(s.Rotation) * Matrix.Translation(s.Position);
            rigidBody.LinearVelocity = s.LinearVelocity;
            rigidBody.AngularVelocity = s.AngularVelocity;
        }

        protected internal virtual void OnPhysicsUpdate()
        {
            // TODO: Check threshold with dead reckoning algorithm to reduce the frequency of triggering event

            var @event = new RigidBodyMovedEvent()
            {
                Position = rigidBody.CenterOfMassPosition,
                Rotation = Quaternion.RotationMatrix(rigidBody.CenterOfMassTransform),
                LinearVelocity = rigidBody.LinearVelocity,
                AngularVelocity = rigidBody.AngularVelocity,
            };

            ApplyEvent(@event);
        }

        protected override void OnEventApplied(IEventArgs @event)
        {
            if (@event is RigidBodyMovedEvent)
            {
                var e = (RigidBodyMovedEvent)@event;

                var s = (RigidBodySnapshot)Snapshot.Clone();

                s.Position = e.Position;
                s.Rotation = e.Rotation;
                s.LinearVelocity = e.LinearVelocity;
                s.AngularVelocity = e.AngularVelocity;

                Snapshot = s;
            }
            else
                base.OnEventApplied(@event);
        }

        protected void ApplyCentralForce(Vector3 force)
        {
            rigidBody.ApplyCentralForce(force);
            rigidBody.Activate();

            logger.Debug("{0} applied central force {1}", this, force);
        }

        public new PhysicsScene Scene
        {
            get { return (PhysicsScene)base.Scene; }
        }

        internal RigidBody RigidBody
        {
            get { return rigidBody; }
        }
    }
}
