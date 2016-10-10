using Nostradamus.Examples;
using Nostradamus.Physics;
using Nostradamus.Server;
using NUnit.Framework;

namespace Nostradamus.Tests.Physics
{
    public class PhysicsSceneTest
    {
        const float FloatAppromiateThreshold = 0.0001f;

        [Test]
        public void TestSimplePhysicsScene()
        {
            var sceneDesc = SimplePhysicsScene.CreateSceneDesc();
            sceneDesc.Mode = SceneMode.Server;
            sceneDesc.SimulationDeltaTime = 50;

            var scene = new SimplePhysicsScene(sceneDesc);
            var sceneContext = (ServerSceneContext)scene.Context;

            // Time: 0
            sceneContext.Simulate();
            var frame0 = sceneContext.FetchDeltaSyncFrame();
            Assert.AreEqual(0, scene.Time);
            Assert.AreEqual(50, scene.DeltaTime);
            Assert.AreEqual(0, frame0.Time);
            Assert.AreEqual(50, frame0.DeltaTime);
            Assert.AreEqual(2, frame0.Events.Count);

            var ballSnapshot0 = (RigidBodySnapshot)scene.Ball.Snapshot;
            var ballEvent0 = (RigidBodyMovedEvent)frame0.Events.Find(e => e.ActorId == scene.Ball.Id).Args;
            Assert.That(ballSnapshot0.Position.X, Is.EqualTo(-0.1f).Within(FloatAppromiateThreshold));
            Assert.That(ballSnapshot0.Position.Y, Is.EqualTo(5.99727488f).Within(FloatAppromiateThreshold));
            Assert.That(ballSnapshot0.Position.Z, Is.EqualTo(-0.1f).Within(FloatAppromiateThreshold));
            Assert.That(ballSnapshot0.Position.X, Is.EqualTo(ballEvent0.Position.X).Within(FloatAppromiateThreshold));
            Assert.That(ballSnapshot0.Position.Y, Is.EqualTo(ballEvent0.Position.Y).Within(FloatAppromiateThreshold));
            Assert.That(ballSnapshot0.Position.Z, Is.EqualTo(ballEvent0.Position.Z).Within(FloatAppromiateThreshold));

            var cubeSnapshot0 = (RigidBodySnapshot)scene.Cube.Snapshot;
            var cubeEvent0 = (RigidBodyMovedEvent)frame0.Events.Find(e => e.ActorId == scene.Cube.Id).Args;
            Assert.That(cubeSnapshot0.Position.X, Is.EqualTo(0.1f).Within(FloatAppromiateThreshold));
            Assert.That(cubeSnapshot0.Position.Y, Is.EqualTo(1.49727499f).Within(FloatAppromiateThreshold));
            Assert.That(cubeSnapshot0.Position.Z, Is.EqualTo(0.1f).Within(FloatAppromiateThreshold));
            Assert.That(cubeSnapshot0.Position.X, Is.EqualTo(cubeEvent0.Position.X).Within(FloatAppromiateThreshold));
            Assert.That(cubeSnapshot0.Position.Y, Is.EqualTo(cubeEvent0.Position.Y).Within(FloatAppromiateThreshold));
            Assert.That(cubeSnapshot0.Position.Z, Is.EqualTo(cubeEvent0.Position.Z).Within(FloatAppromiateThreshold));

            // Time: 50
            sceneContext.Simulate();
            var frame50 = sceneContext.FetchDeltaSyncFrame();
            Assert.AreEqual(50, scene.Time);
            Assert.AreEqual(50, scene.DeltaTime);
            Assert.AreEqual(50, frame50.Time);
            Assert.AreEqual(50, frame50.DeltaTime);
            Assert.AreEqual(2, frame50.Events.Count);

            var ballSnapshot50 = (RigidBodySnapshot)scene.Ball.Snapshot;
            var ballEvent50 = (RigidBodyMovedEvent)frame50.Events.Find(e => e.ActorId == scene.Ball.Id).Args;
            Assert.That(ballSnapshot50.Position.X, Is.EqualTo(-0.1f).Within(FloatAppromiateThreshold));
            Assert.That(ballSnapshot50.Position.Y, Is.EqualTo(5.9918251f).Within(FloatAppromiateThreshold));
            Assert.That(ballSnapshot50.Position.Z, Is.EqualTo(-0.1f).Within(FloatAppromiateThreshold));
            Assert.That(ballSnapshot50.Position.X, Is.EqualTo(ballEvent50.Position.X).Within(FloatAppromiateThreshold));
            Assert.That(ballSnapshot50.Position.Y, Is.EqualTo(ballEvent50.Position.Y).Within(FloatAppromiateThreshold));
            Assert.That(ballSnapshot50.Position.Z, Is.EqualTo(ballEvent50.Position.Z).Within(FloatAppromiateThreshold));

            var cubeSnapshot50 = (RigidBodySnapshot)scene.Cube.Snapshot;
            var cubeEvent50 = (RigidBodyMovedEvent)frame50.Events.Find(e => e.ActorId == scene.Cube.Id).Args;
            Assert.That(cubeSnapshot50.Position.X, Is.EqualTo(0.1f).Within(FloatAppromiateThreshold));
            Assert.That(cubeSnapshot50.Position.Y, Is.EqualTo(1.49182498f).Within(FloatAppromiateThreshold));
            Assert.That(cubeSnapshot50.Position.Z, Is.EqualTo(0.1f).Within(FloatAppromiateThreshold));
            Assert.That(cubeSnapshot50.Position.X, Is.EqualTo(cubeEvent50.Position.X).Within(FloatAppromiateThreshold));
            Assert.That(cubeSnapshot50.Position.Y, Is.EqualTo(cubeEvent50.Position.Y).Within(FloatAppromiateThreshold));
            Assert.That(cubeSnapshot50.Position.Z, Is.EqualTo(cubeEvent50.Position.Z).Within(FloatAppromiateThreshold));

            // Time: 100
            sceneContext.Simulate();
            var frame100 = sceneContext.FetchDeltaSyncFrame();
            Assert.AreEqual(100, scene.Time);
            Assert.AreEqual(50, scene.DeltaTime);
            Assert.AreEqual(100, frame100.Time);
            Assert.AreEqual(50, frame100.DeltaTime);
            Assert.AreEqual(2, frame100.Events.Count);

            var ballSnapshot100 = (RigidBodySnapshot)scene.Ball.Snapshot;
            var ballEvent100 = (RigidBodyMovedEvent)frame100.Events.Find(e => e.ActorId == scene.Ball.Id).Args;
            Assert.That(ballSnapshot100.Position.X, Is.EqualTo(-0.1f).Within(FloatAppromiateThreshold));
            Assert.That(ballSnapshot100.Position.Y, Is.EqualTo(5.98365021f).Within(FloatAppromiateThreshold));
            Assert.That(ballSnapshot100.Position.Z, Is.EqualTo(-0.1f).Within(FloatAppromiateThreshold));
            Assert.That(ballSnapshot100.Position.X, Is.EqualTo(ballEvent100.Position.X).Within(FloatAppromiateThreshold));
            Assert.That(ballSnapshot100.Position.Y, Is.EqualTo(ballEvent100.Position.Y).Within(FloatAppromiateThreshold));
            Assert.That(ballSnapshot100.Position.Z, Is.EqualTo(ballEvent100.Position.Z).Within(FloatAppromiateThreshold));

            var cubeSnapshot100 = (RigidBodySnapshot)scene.Cube.Snapshot;
            var cubeEvent100 = (RigidBodyMovedEvent)frame100.Events.Find(e => e.ActorId == scene.Cube.Id).Args;
            Assert.That(cubeSnapshot100.Position.X, Is.EqualTo(0.1f).Within(FloatAppromiateThreshold));
            Assert.That(cubeSnapshot100.Position.Y, Is.EqualTo(1.48364997f).Within(FloatAppromiateThreshold));
            Assert.That(cubeSnapshot100.Position.Z, Is.EqualTo(0.1f).Within(FloatAppromiateThreshold));
            Assert.That(cubeSnapshot100.Position.X, Is.EqualTo(cubeEvent100.Position.X).Within(FloatAppromiateThreshold));
            Assert.That(cubeSnapshot100.Position.Y, Is.EqualTo(cubeEvent100.Position.Y).Within(FloatAppromiateThreshold));
            Assert.That(cubeSnapshot100.Position.Z, Is.EqualTo(cubeEvent100.Position.Z).Within(FloatAppromiateThreshold));
        }
    }
}
