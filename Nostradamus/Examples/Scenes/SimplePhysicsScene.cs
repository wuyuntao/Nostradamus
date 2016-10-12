using BulletSharp;
using BulletSharp.Math;
using Nostradamus.Physics;
using System;

namespace Nostradamus.Examples
{
    public class SimplePhysicsScene : PhysicsScene
    {
        private SimpleBall ball;
        private SimpleCube cube;

        public SimplePhysicsScene(PhysicsSceneDesc desc)
            : base(desc)
        {
            if (Desc.Mode == SceneMode.Server)
            {
                var clientId = new ClientId(1);

                var cubeId = new ActorId(1, "Cube");
                var cubePosition = new Vector3(0.1f, 1.5f, 0.1f);
                cube = new SimpleCube(this, cubeId, null, cubePosition);

                var ballId = new ActorId(2, "Ball");
                var ballPosition = new Vector3(-0.1f, 6f, -0.1f);
                ball = new SimpleBall(this, ballId, clientId, ballPosition);
            }
        }

        protected internal override void Update()
        {
            if (cube == null && ball == null && Simulator is ServerSimulator)
            {

            }

            base.Update();
        }

        public static PhysicsSceneDesc CreateSceneDesc()
        {
            var ground = new SceneColliderDesc()
            {
                Shape = new BoxShape(500, 1, 500),
                Transform = Matrix.Translation(0, -1, 0),
            };

            return new PhysicsSceneDesc()
            {
                Colliders = new[] { ground },
            };
        }

        protected internal override Actor CreateActor(ActorId actorId, ISnapshotArgs snapshot)
        {
            switch (actorId.Value)
            {
                case 1:
                    var cubePosition = new Vector3(0.1f, 1.5f, 0.1f);
                    cube = new SimpleCube(this, actorId, null, cubePosition);
                    return cube;

                case 2:
                    var clientId = new ClientId(1);
                    var ballId = new ActorId(2, "Ball");
                    var ballPosition = new Vector3(-0.1f, 6f, -0.1f);
                    ball = new SimpleBall(this, actorId, clientId, ballPosition);
                    return ball;

                default:
                    throw new NotSupportedException(actorId.ToString());
            }
        }

        public SimpleBall Ball
        {
            get { return ball; }
        }

        public SimpleCube Cube
        {
            get { return cube; }
        }
    }
}
