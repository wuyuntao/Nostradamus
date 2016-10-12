using BulletSharp;
using BulletSharp.Math;
using Nostradamus.Physics;
using Nostradamus.Server;

namespace Nostradamus.Examples
{
    public class ExampleSceneDesc : PhysicsSceneDesc
    {
        public ExampleSceneDesc()
        {
            var ground = new SceneColliderDesc()
            {
                Shape = new BoxShape(500, 1, 500),
                Transform = Matrix.Translation(0, -1, 0),
            };

            Colliders = new[] { ground };
        }
    }

    public class ExampleScene : PhysicsScene
    {
        private Ball ball;
        private Cube cube;

        protected internal override void Update()
        {
            if (cube == null && ball == null && Simulator is ServerSimulator)
            {
                var cubeDesc = new CubeDesc(new Vector3(3.1f, 1.1f, 3.1f));
                cube = Context.CreateActor<Cube>(cubeDesc);
                AddActor(cube);

                var ballDesc = new BallDesc(new Vector3(-3.1f, 2.6f, -3.1f));
                ball = Context.CreateActor<Ball>(ballDesc);
                AddActor(ball);
            }

            base.Update();
        }

        public Ball Ball
        {
            get { return ball; }
        }

        public Cube Cube
        {
            get { return cube; }
        }
    }
}
