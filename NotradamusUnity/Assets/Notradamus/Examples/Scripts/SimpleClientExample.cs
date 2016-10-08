using Nostradamus.Client;
using Nostradamus.Networking;
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
        }

        void OnDestroy()
        {
            client.Dispose();
        }
    }
}
