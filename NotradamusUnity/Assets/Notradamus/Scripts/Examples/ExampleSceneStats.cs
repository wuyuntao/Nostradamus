using BulletSharp.Math;

namespace Nostradamus.Examples
{
    class ExampleSceneStats : Stats
    {
        public Vector3? BallPosition;

        public Vector3? BallRigidBodyPosition;

        public override string ToCsv()
        {
            return string.Format("{0},{1},{2},{3},{4},{5}"
                    , BallPosition.HasValue ? BallPosition.Value.X.ToString("f4") : string.Empty
                    , BallPosition.HasValue ? BallPosition.Value.Y.ToString("f4") : string.Empty
                    , BallPosition.HasValue ? BallPosition.Value.Z.ToString("f4") : string.Empty
                    , BallRigidBodyPosition.HasValue ? BallRigidBodyPosition.Value.X.ToString("f4") : string.Empty
                    , BallRigidBodyPosition.HasValue ? BallRigidBodyPosition.Value.Y.ToString("f4") : string.Empty
                    , BallRigidBodyPosition.HasValue ? BallRigidBodyPosition.Value.Z.ToString("f4") : string.Empty
                );
        }
    }
}
