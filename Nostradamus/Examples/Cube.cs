using BulletSharp;
using BulletSharp.Math;
using Nostradamus.Physics;

namespace Nostradamus.Examples
{
    public class CubeDesc : RigidBodyDesc
    {
        public CubeDesc(ActorId id, Vector3 initialPosition)
            : base(id, 1, new BoxShape(1), Matrix.Identity, Matrix.Translation(initialPosition), false)
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

    public class Cube : RigidBodyActor
    {
    }
}
