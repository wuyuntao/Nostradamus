using System;
using BulletSharp;
using BulletSharp.Math;
using Nostradamus.Physics;
using Nostradamus.Tests.Events;

namespace Nostradamus.Tests.Actors
{
	class SimpleCube : RigidBodyActor
	{
		public SimpleCube(PhysicsScene scene, ActorId id, ClientId clientId, Vector3 initialPosition)
			: base(scene, id, clientId
					, CreateRigidBodyDesc(initialPosition)
					, CreateRigidBodySnapshot(initialPosition))
		{
		}

		private static RigidBodyDesc CreateRigidBodyDesc(Vector3 initialPosition)
		{
			return new RigidBodyDesc()
			{
				Mass = 1,
				Shape = new BoxShape(1),
				CenterOfMassOffset = Matrix.Identity,
				IsKinematic = false,
				StartTransform = Matrix.Translation(initialPosition),
			};
		}

		private static RigidBodySnapshot CreateRigidBodySnapshot(Vector3 initialPosition)
		{
			return new RigidBodySnapshot()
			{
				Position = initialPosition,
				Rotation = Quaternion.Identity,
				LinearVelocity = Vector3.Zero,
				AngularVelocity = Vector3.Zero,
			};
		}

		protected internal override void OnCommandReceived(ICommandArgs command)
		{
			throw new NotSupportedException(command.GetType().FullName);
		}

		protected internal override ISnapshotArgs OnEventApplied(IEventArgs @event)
		{
			var snapshot = base.OnEventApplied(@event);
			if (snapshot == null)
				throw new NotSupportedException(@event.GetType().FullName);

			return snapshot;
		}
	}
}
