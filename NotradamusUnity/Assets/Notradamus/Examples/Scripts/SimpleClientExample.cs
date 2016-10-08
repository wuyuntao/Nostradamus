using Nostradamus.Client;
using Nostradamus.Networking;
using Nostradamus.Physics;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Nostradamus.Examples
{
    public class SimpleClientExample : MonoBehaviour
    {
        private ClientId clientId;
        private SimplePhysicsScene scene;
        private ClientSimulator simulator;
        private ReliableUdpClient client;
        private Dictionary<ActorId, RigidBodyInterpolate> actors = new Dictionary<ActorId, RigidBodyInterpolate>();

        void Awake()
        {
            Serializer.Initialize();

            clientId = new ClientId(1);
            simulator = new ClientSimulator(clientId, 50);
            scene = new SimplePhysicsScene(simulator);
            client = new ReliableUdpClient(simulator, 20, new IPEndPoint(IPAddress.Loopback, 9000));
            client.Start();
        }

        void FixedUpdate()
        {
            client.Update();

            foreach (var actor in scene.Actors)
            {
                if (!actors.ContainsKey(actor.Id))
                {
                    var go = ObjectPool.Instantiate(actor.GetType().Name, transform);
                    if (go == null)
                        continue;

                    actors.Add(actor.Id, RigidBodyInterpolate.Initialize(go, (RigidBodyActor)actor));
                }
            }
        }

        void OnDestroy()
        {
            client.Dispose();
        }
    }
}
