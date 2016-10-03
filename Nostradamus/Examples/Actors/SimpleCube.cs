using BulletSharp;
using BulletSharp.Math;
using Nostradamus.Physics;
using System;

namespace Nostradamus.Examples
{
	public class SimpleCube : RigidBodyActor
	{
		public SimpleCube(PhysicsScene scene, ActorId id, ClientId ownerId, Vector3 initialPosition)
			: base(scene, id, ownerId,
					CreateRigidBodyDesc(initialPosition),
					CreateRigidBodySnapshot(initialPosition))
		{ }

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
	}
}
