using BulletSharp;
using BulletSharp.Math;
using Nostradamus.Physics;

namespace Nostradamus.Examples
{
    public class BallDesc : RigidBodyDesc
    {
        public BallDesc(ActorId id, Vector3 initialPosition)
            : base(id, 10, new SphereShape(2.5f), Matrix.Identity, Matrix.Translation(initialPosition), false)
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
        bool hasMoved;

        protected internal override void OnCommandReceived(ICommandArgs command)
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
                base.OnCommandReceived(command);
        }

        protected internal override void OnUpdate()
        {
            base.OnUpdate();

            hasMoved = false;
        }
    }
}
