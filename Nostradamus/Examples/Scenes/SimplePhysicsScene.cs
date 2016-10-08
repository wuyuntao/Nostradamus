using BulletSharp;
using BulletSharp.Math;
using Nostradamus.Physics;
using Nostradamus.Server;
using System;

namespace Nostradamus.Examples
{
    public class SimplePhysicsScene : PhysicsScene
    {
        private SimpleBall ball;
        private SimpleCube cube;

        public SimplePhysicsScene(Simulator simulator)
            : base(simulator, CreatePhysicsSceneDesc())
        {
            if (simulator is ServerSimulator)
            {
                var clientId = new ClientId(1);
                var cubeId = new ActorId(1, "Cube");
                var cubePosition = new Vector3(0.1f, 1.5f, 0.1f);
                cube = new SimpleCube(this, cubeId, clientId, cubePosition);

                var ballId = new ActorId(2, "Ball");
                var ballPosition = new Vector3(-0.1f, 6f, -0.1f);
                ball = new SimpleBall(this, ballId, null, ballPosition);
            }
        }

        private static PhysicsSceneDesc CreatePhysicsSceneDesc()
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

        protected override Actor CreateActor(ActorId actorId, ISnapshotArgs snapshot)
        {
            switch (actorId.Value)
            {
                case 1:
                    var clientId = new ClientId(1);
                    var cubePosition = new Vector3(0.1f, 1.5f, 0.1f);
                    cube = new SimpleCube(this, actorId, clientId, cubePosition);
                    return cube;

                case 2:
                    var ballId = new ActorId(2, "Ball");
                    var ballPosition = new Vector3(-0.1f, 6f, -0.1f);
                    ball = new SimpleBall(this, actorId, null, ballPosition);
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
