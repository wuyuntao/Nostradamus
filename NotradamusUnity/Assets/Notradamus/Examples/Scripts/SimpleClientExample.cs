using NLog;
using Nostradamus.Client;
using Nostradamus.Networking;
using Nostradamus.Physics;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Nostradamus.Examples
{
    public class SimpleClientExample : MonoBehaviour
    {
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

        private ClientId clientId;
        private ExampleScene scene;
        private ClientSimulator simulator;
        private ReliableUdpClient client;
        private Dictionary<ActorId, RigidBodyInterpolate> actors = new Dictionary<ActorId, RigidBodyInterpolate>();

        void Awake()
        {
            Serializer.Initialize();

            clientId = new ClientId(1);
            simulator = new ClientSimulator(clientId);
            simulator.RegisterActorFactory<ExampleSceneDesc, ExampleScene>(desc => new ExampleScene());
            simulator.RegisterActorFactory<BallDesc, Ball>(desc => new Ball());
            simulator.RegisterActorFactory<CubeDesc, Cube>(desc => new Cube());

            var sceneDesc = new ExampleSceneDesc(20, 50);
            scene = simulator.CreateScene<ExampleScene>(sceneDesc);
            client = new ReliableUdpClient(simulator, new IPEndPoint(IPAddress.Loopback, 9000));
            client.Start();
        }

        void FixedUpdate()
        {
            if (scene.Ball != null)
            {
                var x = Input.GetAxis("Horizontal");
                var y = Input.GetAxis("Jump");
                var z = Input.GetAxis("Vertical");
                if (x != 0 || y != 0 || z != 0)
                {
                    var command = new KickBallCommand(x, y, z);
                    simulator.ReceiveCommand(scene.Ball, command);
                }
            }

            client.Update();

            foreach (var actor in scene.Actors)
            {
                if (!actors.ContainsKey(actor.Desc.Id))
                {
                    var actorName = actor.GetType().Name;
                    var go = ObjectPool.Instantiate(actorName, transform);
                    if (go == null)
                        continue;

                    if (actorName == "Ball")
                    {
                        var camera = GameObject.Find("Main Camera").GetComponent<CameraFollower>();
                        camera.followTarget = go.transform;

                        logger.Info("Set camera to {0}", go.name);
                    }

                    actors.Add(actor.Desc.Id, RigidBodyInterpolate.Initialize(go, (RigidBodyActor)actor));
                }
            }
        }

        void OnDestroy()
        {
            client.Dispose();
        }
    }
}
