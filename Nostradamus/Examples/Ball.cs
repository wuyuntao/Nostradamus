using BulletSharp;
using BulletSharp.Math;
using Nostradamus.Physics;

namespace Nostradamus.Examples
{
    public class BallDesc : RigidBodyDesc
    {
        public readonly float HorizontalForceFactor = 1000;

        public readonly float VerticalForceFactor = 1000;

        public BallDesc(ActorId id, Vector3 initialPosition)
            : base(id, 10, new SphereShape(2.5f), Matrix.Identity, Matrix.Translation(initialPosition), false, 1, 1, 0.8f, 0.2f, 0.2f)
        { }

        protected internal override ISnapshotArgs InitSnapshot()
        {
            return new RigidBodySnapshot()
            {
                Position = StartTransform.Origin,
                Rotation = Quaternion.Identity,
            };
        }
    }

    public class Ball : RigidBodyActor
    {
        private Vector3 inputVector;
        private int inputCount;

        protected internal override void OnCommandReceived(ICommandArgs command)
        {
            if (command is KickBallCommand)
            {
                var c = (KickBallCommand)command;

                inputVector += new Vector3(c.InputX, c.InputY, c.InputZ);
                inputCount++;
            }
            else
                base.OnCommandReceived(command);
        }

        protected internal override void OnUpdate()
        {
            base.OnUpdate();

            if (inputCount > 0)
            {
                var desc = (BallDesc)Desc;
                var force = new Vector3(inputVector.X * desc.HorizontalForceFactor, inputVector.Y * desc.VerticalForceFactor, inputVector.Z * desc.HorizontalForceFactor) / inputCount;

                if (force.LengthSquared > 0)
                    ApplyCentralForce(force);

                inputVector = Vector3.Zero;
                inputCount = 0;
            }
        }
    }
}
