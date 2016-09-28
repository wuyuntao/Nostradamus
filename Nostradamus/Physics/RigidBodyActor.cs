using BulletSharp;
using BulletSharp.Math;
using System;

namespace Nostradamus.Physics
{
	public abstract class RigidBodyActor : Actor
	{
		private readonly PhysicsScene scene;
		private readonly RigidBody rigidBody;

		protected RigidBodyActor(PhysicsScene scene, ActorId id, ClientId ownerId, RigidBodyDesc parameters, RigidBodySnapshot snapshot)
			: base(scene, id, ownerId, snapshot)
		{
			this.scene = scene;
			this.rigidBody = CreateRigidBody(parameters);
		}

		private RigidBody CreateRigidBody(RigidBodyDesc p)
		{
			var localInertia = Vector3.Zero;
			if (!p.IsKinematic)
				p.Shape.CalculateLocalInertia(p.Mass, out localInertia);

			var motionState = new DefaultMotionState(p.StartTransform, p.CenterOfMassOffset);

			using (var rbInfo = new RigidBodyConstructionInfo(p.Mass, motionState, p.Shape, localInertia))
			{
				var body = new RigidBody(rbInfo);
				body.UserObject = this;

				if (p.IsKinematic)
					body.CollisionFlags |= CollisionFlags.KinematicObject;

				return body;
			}
		}

		protected internal override void OnUpdate()
		{
			SyncSnapshotFromRigidBody((RigidBodySnapshot)Snapshot);
		}

		protected internal override void SetSnapshot(ISnapshotArgs snapshot)
		{
			var s = (RigidBodySnapshot)snapshot;

			base.SetSnapshot(snapshot);

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
