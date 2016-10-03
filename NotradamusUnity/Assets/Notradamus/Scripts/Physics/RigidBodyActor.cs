﻿using BulletSharp;
using BulletSharp.Math;

namespace Nostradamus.Physics
{
	public abstract class RigidBodyActor : Actor
	{
		private readonly PhysicsScene scene;
		private RigidBody rigidBody;

		protected RigidBodyActor(PhysicsScene scene, ActorId id, ClientId ownerId, RigidBodyDesc parameters, RigidBodySnapshot snapshot)
			: base(scene, id, ownerId, snapshot)
		{
			this.scene = scene;

			InitializeRigidBody(parameters);
		}

		private void InitializeRigidBody(RigidBodyDesc desc)
		{
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

				scene.World.AddRigidBody(rigidBody);
			}
		}

		protected override void DisposeManaged()
		{
			scene.World.RemoveRigidBody(rigidBody);

			SafeDispose(ref rigidBody);

			base.DisposeManaged();
		}

		protected override ISnapshotArgs OnEventApplied(IEventArgs @event)
		{
			if (@event is RigidBodyMovedEvent)
			{
				var e = (RigidBodyMovedEvent)@event;

				var s = (RigidBodySnapshot)CreateSnapshot();

				s.Position = e.Position;
				s.Rotation = e.Rotation;
				s.LinearVelocity = e.LinearVelocity;
				s.AngularVelocity = e.AngularVelocity;

				return s;
			}
			else
				return null;
		}

		protected internal override void OnUpdate()
		{
			SyncSnapshotFromRigidBody((RigidBodySnapshot)Snapshot);
		}

		protected override void OnSnapshotRecovered(ISnapshotArgs snapshot)
		{
			var s = (RigidBodySnapshot)snapshot;

			SyncRigidBodyFromSnapshot(s);
		}

		private void SyncSnapshotFromRigidBody(RigidBodySnapshot snapshot)
		{
			snapshot.Position = rigidBody.CenterOfMassPosition;
			snapshot.Rotation = Quaternion.RotationMatrix(rigidBody.CenterOfMassTransform);
			snapshot.LinearVelocity = rigidBody.LinearVelocity;
			snapshot.AngularVelocity = rigidBody.AngularVelocity;
		}

		private void SyncRigidBodyFromSnapshot(RigidBodySnapshot snapshot)
		{
			rigidBody.CenterOfMassTransform = Matrix.RotationQuaternion(snapshot.Rotation) * Matrix.Translation(snapshot.Position);
			rigidBody.LinearVelocity = snapshot.LinearVelocity;
			rigidBody.AngularVelocity = snapshot.AngularVelocity;
		}

		protected void ApplyCentralForce(Vector3 force)
		{
			rigidBody.ApplyCentralForce(force);
			rigidBody.Activate();
		}

		internal void ApplyMovedEvent()
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

		protected new PhysicsScene Scene
		{
			get { return scene; }
		}

		internal RigidBody RigidBody
		{
			get { return rigidBody; }
		}
	}
}
