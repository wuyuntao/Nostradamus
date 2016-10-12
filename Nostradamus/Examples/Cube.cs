using BulletSharp;
using BulletSharp.Math;
using Nostradamus.Physics;

namespace Nostradamus.Examples
{
    public class CubeDesc : RigidBodyActorDesc
    {
        public CubeDesc(Vector3 initialPosition)
        {
            Mass = 1;
            Shape = new BoxShape(1);
            CenterOfMassOffset = Matrix.Identity;
            IsKinematic = false;
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

    public class Cube : RigidBodyActor
    {
    }
}
