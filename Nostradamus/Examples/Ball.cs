using BulletSharp.Math;
using Nostradamus.Physics;

namespace Nostradamus.Examples
{
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
