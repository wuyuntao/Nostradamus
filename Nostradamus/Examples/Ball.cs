using BulletSharp;
using BulletSharp.Math;
using Nostradamus.Physics;

namespace Nostradamus.Examples
{
    public class BallDesc : RigidBodyActorDesc
    {
        public BallDesc(Vector3 initialPosition)
        {
            Mass = 10;
            Shape = new SphereShape(2.5f);
            CenterOfMassOffset = Matrix.Identity;
            StartTransform = Matrix.Translation(initialPosition);
        }

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
        bool hasMoved;

        protected internal override void ReceiveCommand(ICommandArgs command)
        {
            if (command is KickBallCommand)
            {
                if (hasMoved)
                    return;

                var c = (KickBallCommand)command;
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
                base.ReceiveCommand(command);
        }

        protected internal override void Update()
        {
            base.Update();

            hasMoved = false;
        }
    }
}
