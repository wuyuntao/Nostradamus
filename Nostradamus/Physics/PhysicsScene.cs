using BulletSharp;

namespace Nostradamus.Physics
{
	public abstract class PhysicsScene : Scene
	{
		private DefaultCollisionConfiguration collisionConf;
		private CollisionDispatcher dispatcher;
		private DbvtBroadphase broadphase;
		private DiscreteDynamicsWorld world;

		public PhysicsScene(Simulator simulator, PhysicsSceneDesc desc)
			: base(simulator)
		{
			// TODO: Enable custom initialization of physics world
			collisionConf = new DefaultCollisionConfiguration();
			dispatcher = new CollisionDispatcher(collisionConf);
			broadphase = new DbvtBroadphase();
			world = new DiscreteDynamicsWorld(dispatcher, broadphase, null, collisionConf);
			world.Gravity = desc.Gravity;

			if (desc.Colliders != null)
			{
				foreach (var collider in desc.Colliders)
					AddCollider(collider);
			}
		}

		protected override void DisposeManaged()
		{
			SafeDispose(ref world);
			SafeDispose(ref broadphase);
			SafeDispose(ref dispatcher);
			SafeDispose(ref collisionConf);

			base.DisposeManaged();
		}

		private void AddCollider(SceneColliderDesc desc)
		{
			var motionState = new DefaultMotionState(desc.Transform);

			using (var rbInfo = new RigidBodyConstructionInfo(0, motionState, desc.Shape))
			{
				var rigidBody = new RigidBody(rbInfo);
				rigidBody.UserObject = this;

				rigidBody.CollisionFlags |= CollisionFlags.StaticObject;

				world.AddRigidBody(rigidBody);
			}
		}

		protected internal override void OnUpdate()
		{
			base.OnUpdate();

			// TODO: Is maxSubSteps and fixedTimestep necessary?
			world.StepSimulation(DeltaTime / 1000f);

			foreach (var actor in Actors)
			{
				if (actor is RigidBodyActor)
				{
					((RigidBodyActor)actor).ApplyMovedEvent();
				}
			}
		}

		internal DiscreteDynamicsWorld World
		{
			get { return world; }
		}
	}
}
