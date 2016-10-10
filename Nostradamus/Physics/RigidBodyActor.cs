﻿using BulletSharp;
using BulletSharp.Math;

namespace Nostradamus.Physics
{
    public abstract class RigidBodyActor : Actor
    {
        private readonly PhysicsScene scene;
        private readonly RigidBody rigidBody;

        protected RigidBodyActor(PhysicsScene scene, ActorId id, ClientId ownerId, RigidBodyActorDesc parameters, RigidBodySnapshot snapshot)
            : base(scene, id, ownerId, snapshot)
        {
            this.scene = scene;

            rigidBody = CreateRigidBody(parameters);
            scene.World.AddRigidBody(rigidBody);
        }

        private RigidBody CreateRigidBody(RigidBodyActorDesc desc)
        {
            var localInertia = Vector3.Zero;
            if (!desc.IsKinematic)
                desc.Shape.CalculateLocalInertia(desc.Mass, out localInertia);

            var motionState = new DefaultMotionState(desc.StartTransform, desc.CenterOfMassOffset);

            using (var rbInfo = new RigidBodyConstructionInfo(desc.Mass, motionState, desc.Shape, localInertia))
            {
                var rb = new RigidBody(rbInfo);
                rb.UserObject = this;

                if (desc.IsKinematic)
                    rb.CollisionFlags |= CollisionFlags.KinematicObject;

                return rb;
            }
        }

        protected override void DisposeManaged()
        {
            scene.World.RemoveRigidBody(rigidBody);
            rigidBody.Dispose();

            base.DisposeManaged();
        }

        protected internal override ISnapshotArgs OnEventApplied(IEventArgs @event)
        {
            if (@event is RigidBodyMovedEvent)
            {
                var e = (RigidBodyMovedEvent)@event;

                var s = (RigidBodySnapshot)Snapshot.Clone();

                s.Position = e.Position;
                s.Rotation = e.Rotation;
                s.LinearVelocity = e.LinearVelocity;
                s.AngularVelocity = e.AngularVelocity;

                logger.Debug("{0} applied {1}: Position: {2}, Rotation: {3}, LinearVelocity: {4}, AngularVelocity: {5}"
                        , this, @event.GetType().Name, e.Position, e.Rotation, e.LinearVelocity, e.AngularVelocity);

                return s;
            }
            else
                return base.OnEventApplied(@event);
        }

        protected void ApplyCentralForce(Vector3 force)
        {
            rigidBody.ApplyCentralForce(force);
            rigidBody.Activate();

            logger.Debug("{0} applied central force {1}", this, force);
        }

        internal void OnPrePhysicsUpdate()
        {
            var snapshot = (RigidBodySnapshot)Snapshot;

            rigidBody.CenterOfMassTransform = Matrix.RotationQuaternion(snapshot.Rotation) * Matrix.Translation(snapshot.Position);
            rigidBody.LinearVelocity = snapshot.LinearVelocity;
            rigidBody.AngularVelocity = snapshot.AngularVelocity;
        }

        internal void OnPostPhysicsUpdate()
        {
            // TODO: Check threshold with dead reckoning algorithm to reduce the frequency of triggering event

            var eventArgs = new RigidBodyMovedEvent()
            {
                Position = rigidBody.CenterOfMassPosition,
                Rotation = Quaternion.RotationMatrix(rigidBody.CenterOfMassTransform),
                LinearVelocity = rigidBody.LinearVelocity,
                AngularVelocity = rigidBody.AngularVelocity,
            };

            ApplyEvent(eventArgs);
        }

        public new PhysicsScene Scene
        {
            get { return scene; }
        }

        internal RigidBody RigidBody
        {
            get { return rigidBody; }
        }
    }
}
