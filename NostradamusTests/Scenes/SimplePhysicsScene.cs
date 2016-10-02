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

		public SimplePhysicsScene(Simulator simulator)
			: base(simulator, CreatePhysicsSceneDesc())
		{
			var clientId = new ClientId(1);

			var cubeId = new ActorId(1, "Cube");
			var cubePosition = new Vector3(0.1f, 1.5f, 0.1f);
			cube = new SimpleCube(this, cubeId, clientId, cubePosition);

			var ballId = new ActorId(2, "Ball");
			var ballPosition = new Vector3(-0.1f, 6f, -0.1f);
			ball = new SimpleBall(this, ballId, clientId, ballPosition);
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
