using Nostradamus.Physics;
using Nostradamus.Server;
using Nostradamus.Tests.Events;
using Nostradamus.Tests.Scenes;
using NUnit.Framework;

namespace Nostradamus.Tests.Physics
{
	public class PhysicsSceneTest
	{
		[Test]
		public void TestSimplePhysicsScene()
		{
			var scene = new SimplePhysicsScene();
			var simulator = new ServerSimulator(scene);

			// Time: 0
			var frame0 = simulator.Simulate(50);
			Assert.AreEqual(0, scene.Time);
			Assert.AreEqual(50, scene.DeltaTime);
			Assert.AreEqual(0, frame0.Time);
			Assert.AreEqual(50, frame0.DeltaTime);
			Assert.AreEqual(2, frame0.Events.Count);

			var ballSnapshot0 = (RigidBodySnapshot)scene.Ball.Snapshot;
			var ballEvent0 = (RigidBodyMovedEvent)frame0.Events.Find(e => e.ActorId == scene.Ball.Id).Args;
			Assert.AreEqual(-0.1f, ballSnapshot0.Position.X);
			Assert.AreEqual(6f, ballSnapshot0.Position.Y);
			Assert.AreEqual(-0.1f, ballSnapshot0.Position.Z);
			Assert.AreEqual(ballSnapshot0.Position.X, ballEvent0.NewSnapshot.Position.X);
			Assert.AreEqual(ballSnapshot0.Position.Y, ballEvent0.NewSnapshot.Position.Y);
			Assert.AreEqual(ballSnapshot0.Position.Z, ballEvent0.NewSnapshot.Position.Z);

			var cubeSnapshot0 = (RigidBodySnapshot)scene.Cube.Snapshot;
			var cubeEvent0 = (RigidBodyMovedEvent)frame0.Events.Find(e => e.ActorId == scene.Cube.Id).Args;
			Assert.AreEqual(0.1f, cubeSnapshot0.Position.X);
			Assert.AreEqual(1.5f, cubeSnapshot0.Position.Y);
			Assert.AreEqual(0.1f, cubeSnapshot0.Position.Z);
			Assert.AreEqual(cubeSnapshot0.Position.X, cubeEvent0.NewSnapshot.Position.X);
			Assert.AreEqual(cubeSnapshot0.Position.Y, cubeEvent0.NewSnapshot.Position.Y);
			Assert.AreEqual(cubeSnapshot0.Position.Z, cubeEvent0.NewSnapshot.Position.Z);

			// Time: 50
			var frame50 = simulator.Simulate(50);
			Assert.AreEqual(50, scene.Time);
			Assert.AreEqual(50, scene.DeltaTime);
			Assert.AreEqual(50, frame50.Time);
			Assert.AreEqual(50, frame50.DeltaTime);
			Assert.AreEqual(2, frame50.Events.Count);

			var ballSnapshot50 = (RigidBodySnapshot)scene.Ball.Snapshot;
			var ballEvent50 = (RigidBodyMovedEvent)frame0.Events.Find(e => e.ActorId == scene.Ball.Id).Args;
			Assert.AreEqual(-0.1f, ballSnapshot0.Position.X);
			Assert.AreEqual(5.99727488f, ballSnapshot0.Position.Y);
			Assert.AreEqual(-0.1f, ballSnapshot0.Position.Z);
			Assert.AreEqual(ballSnapshot50.Position.X, ballEvent50.NewSnapshot.Position.X);
			Assert.AreEqual(ballSnapshot50.Position.Y, ballEvent50.NewSnapshot.Position.Y);
			Assert.AreEqual(ballSnapshot50.Position.Z, ballEvent50.NewSnapshot.Position.Z);

			var cubeSnapshot50 = (RigidBodySnapshot)scene.Cube.Snapshot;
			var cubeEvent50 = (RigidBodyMovedEvent)frame0.Events.Find(e => e.ActorId == scene.Cube.Id).Args;
			Assert.AreEqual(0.1f, cubeSnapshot0.Position.X);
			Assert.AreEqual(1.49727499f, cubeSnapshot0.Position.Y);
			Assert.AreEqual(0.1f, cubeSnapshot0.Position.Z);
			Assert.AreEqual(cubeSnapshot50.Position.X, cubeEvent50.NewSnapshot.Position.X);
			Assert.AreEqual(cubeSnapshot50.Position.Y, cubeEvent50.NewSnapshot.Position.Y);
			Assert.AreEqual(cubeSnapshot50.Position.Z, cubeEvent50.NewSnapshot.Position.Z);

			// Time: 100
			var frame100 = simulator.Simulate(50);
			Assert.AreEqual(100, scene.Time);
			Assert.AreEqual(50, scene.DeltaTime);
			Assert.AreEqual(100, frame100.Time);
			Assert.AreEqual(50, frame100.DeltaTime);
			Assert.AreEqual(2, frame100.Events.Count);

			var ballSnapshot100 = (RigidBodySnapshot)scene.Ball.Snapshot;
			var ballEvent100 = (RigidBodyMovedEvent)frame0.Events.Find(e => e.ActorId == scene.Ball.Id).Args;
			Assert.AreEqual(-0.1f, ballSnapshot0.Position.X);
			Assert.AreEqual(5.99727488f, ballSnapshot0.Position.Y);
			Assert.AreEqual(-0.1f, ballSnapshot0.Position.Z);
			Assert.AreEqual(ballSnapshot100.Position.X, ballEvent100.NewSnapshot.Position.X);
			Assert.AreEqual(ballSnapshot100.Position.Y, ballEvent100.NewSnapshot.Position.Y);
			Assert.AreEqual(ballSnapshot100.Position.Z, ballEvent100.NewSnapshot.Position.Z);

			var cubeSnapshot100 = (RigidBodySnapshot)scene.Cube.Snapshot;
			var cubeEvent100 = (RigidBodyMovedEvent)frame0.Events.Find(e => e.ActorId == scene.Cube.Id).Args;
			Assert.AreEqual(0.1f, cubeSnapshot0.Position.X);
			Assert.AreEqual(1.49727499f, cubeSnapshot0.Position.Y);
			Assert.AreEqual(0.1f, cubeSnapshot0.Position.Z);
			Assert.AreEqual(cubeSnapshot100.Position.X, cubeEvent100.NewSnapshot.Position.X);
			Assert.AreEqual(cubeSnapshot100.Position.Y, cubeEvent100.NewSnapshot.Position.Y);
			Assert.AreEqual(cubeSnapshot100.Position.Z, cubeEvent100.NewSnapshot.Position.Z);
		}
	}
}
