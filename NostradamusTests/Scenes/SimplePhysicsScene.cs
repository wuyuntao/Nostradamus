using BulletSharp;
using BulletSharp.Math;
using Nostradamus.Physics;
using Nostradamus.Tests.Actors;

namespace Nostradamus.Tests.Scenes
{
	class SimplePhysicsScene : PhysicsScene
	{
		private SimpleBall ball;
		private SimpleCube cube;

		public SimplePhysicsScene()
			: base(CreatePhysicsSceneDesc())
		{ }

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

		protected override void OnUpdate()
		{
			if (Time == 0)
			{
				var cubeId = CreateActorId("Cube");
				var cubePosition = new Vector3(0.1f, 1.5f, 0.1f);
				cube = new SimpleCube(this, cubeId, cubePosition);

				AddActor(cube);

				var ballId = CreateActorId("Ball");
				var ballPosition = new Vector3(-0.1f, 6f, -0.1f);
				ball = new SimpleBall(this, ballId, ballPosition);

				AddActor(ball);
			}

			base.OnUpdate();
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
