using BulletSharp;

namespace Nostradamus.Physics
{
    public abstract class PhysicsScene : Scene
    {
        private DefaultCollisionConfiguration collisionConf;
        private CollisionDispatcher dispatcher;
        private DbvtBroadphase broadphase;
        private DiscreteDynamicsWorld world;

        internal override void Initialize(ActorContext context, ActorDesc actorDesc)
        {
            base.Initialize(context, actorDesc);

            var desc = (PhysicsSceneDesc)actorDesc;

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

            PhysicsUpdate();
        }

        protected void PhysicsUpdate()
        {
            var timeStep = Desc.SimulationDeltaTime / 1000f;
            world.StepSimulation(timeStep, 1, timeStep);

            foreach (var actor in Actors)
            {
                if (actor is RigidBodyActor)
                {
                    ((RigidBodyActor)actor).OnPhysicsUpdate();
                }
            }
        }

        internal DiscreteDynamicsWorld World
        {
            get { return world; }
        }
    }
}
