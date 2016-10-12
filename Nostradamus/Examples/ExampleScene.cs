using BulletSharp;
using BulletSharp.Math;
using Nostradamus.Physics;
using Nostradamus.Server;

namespace Nostradamus.Examples
{
    public class ExampleSceneDesc : PhysicsSceneDesc
    {
        public ExampleSceneDesc(int simulationDeltaTime, int reconciliationDeltaTime)
            : base(new ActorId(1), simulationDeltaTime, reconciliationDeltaTime, DefaultGravity, CreateColliders())
        { }

        private static SceneColliderDesc[] CreateColliders()
        {
            var ground = new SceneColliderDesc()
            {
                Shape = new BoxShape(500, 1, 500),
                Transform = Matrix.Translation(0, -1, 0),
            };

            return new[] { ground };
        }
    }

    public class ExampleScene : PhysicsScene
    {
        private Ball ball;
        private Cube cube;

        protected internal override void OnUpdate()
        {
            if (cube == null && ball == null && Simulator is ServerSimulator)
            {
                ApplyEvent(new SceneInitializedEvent());
            }

            base.OnUpdate();
        }

        protected override void OnEventApplied(IEventArgs @event)
        {
            if (@event is SceneInitializedEvent)
            {
                var s = (SceneSnapshot)Snapshot.Clone();

                var cubeId = new ActorId(2);
                var cubeDesc = new CubeDesc(cubeId, new Vector3(3.1f, 1.1f, 3.1f));
                cube = Context.CreateActor<Cube>(cubeDesc);
                AddActor(s, cube);

                var ballId = new ActorId(3);
                var ballDesc = new BallDesc(ballId, new Vector3(-3.1f, 2.6f, -3.1f));
                ball = Context.CreateActor<Ball>(ballDesc);
                AddActor(s, ball);

                Snapshot = s;
            }
            else
                base.OnEventApplied(@event);
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
