using BulletSharp;
using BulletSharp.Math;
using Nostradamus.Physics;
using System;

namespace Nostradamus.Examples
{
	public class SimpleBall : RigidBodyActor
	{
		bool hasMoved;

		public SimpleBall(PhysicsScene scene, ActorId id, ClientId ownerId, Vector3 initialPosition)
			: base(scene, id, ownerId,
					CreateRigidBodyDesc(initialPosition),
					CreateRigidBodySnapshot(initialPosition))
		{ }

		private static RigidBodyDesc CreateRigidBodyDesc(Vector3 initialPosition)
		{
			return new RigidBodyDesc()
			{
				Mass = 100,
				Shape = new SphereShape(2.5f),
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
			if (command is MoveBallCommand)
			{
				if (hasMoved)
					return;

				var c = (MoveBallCommand)command;
				var horizontal = new Vector3(c.InputX, 0, c.InputZ) * 1000;
				if (horizontal.LengthSquared > 0)
				{
					ApplyCentralForce(horizontal);
				}

				var vertical = new Vector3(0, c.InputY, 0) * 2000;
				if (vertical.LengthSquared > 0)
				{
					ApplyCentralForce(vertical);
				}

				hasMoved = true;
			}
			else
				throw new NotSupportedException(command.GetType().FullName);
		}

		protected override ISnapshotArgs OnEventApplied(IEventArgs @event)
		{
			var snapshot = base.OnEventApplied(@event);
			if (snapshot == null)
				throw new NotSupportedException(@event.GetType().FullName);

			return snapshot;
		}
	}
}
