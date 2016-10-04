using Lidgren.Network;
using NLog;
using Nostradamus.Client;
using Nostradamus.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Nostradamus.Networking
{
    public sealed class ReliableUdpServer
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ServerSimulator simulator;
        private readonly NetServer server;
        private readonly SortedList<ClientId, Client> clients = new SortedList<ClientId, Client>();
        private readonly int simulateDeltaTime;
        private bool stopRequest;

        public ReliableUdpServer(ServerSimulator simulator, int simulateDeltaTime, int listeningPort, string appIdentifier = "Nostradamus")
        {
            this.simulator = simulator;
            this.simulateDeltaTime = simulateDeltaTime;

            var serverConf = new NetPeerConfiguration(appIdentifier);
            serverConf.Port = listeningPort;

            server = new NetServer(serverConf);
        }

        public void Start()
        {
            if (server.Status != NetPeerStatus.NotRunning)
                throw new InvalidOperationException("Already started");

            var timer = Stopwatch.StartNew();

            server.Start();

            ThreadPool.QueueUserWorkItem(LoopThread, timer);
        }

        public void Stop()
        {
            if (stopRequest)
                throw new InvalidOperationException("Already stopped");

            stopRequest = true;
        }

        private void LoopThread(object state)
        {
            var timer = (Stopwatch)state;

            for (var i = 0; !stopRequest; i++)
            {
                var simulateTime = simulateDeltaTime * i;
                var waitTime = simulateTime - (int)timer.ElapsedMilliseconds;
                if (waitTime > 0)
                    Thread.Sleep(waitTime);

                for (var msg = server.ReadMessage(); msg != null; msg = server.ReadMessage())
                    OnServerMessage(msg);

                simulator.Simulate(simulateDeltaTime);

                var deltaSyncFrame = simulator.FetchDeltaSyncFrame();
                SendMessageToAll(deltaSyncFrame);
            }
        }

        private void OnServerMessage(NetIncomingMessage msg)
        {
            switch (msg.MessageType)
            {
                case NetIncomingMessageType.StatusChanged:
                    {
                        var status = (NetConnectionStatus)msg.ReadByte();

                        OnServerMessage_StatusChanged(msg, status);
                    }
                    break;

                case NetIncomingMessageType.Data:
                    {
                        var envelope = Serializer.Deserialize<MessageEnvelope>(msg.Data);

                        if (envelope.Message is CommandFrame)
                        {
                            OnServerMessage_ClientSyncFrame(msg, (CommandFrame)envelope.Message);
                        }
                        else if (envelope.Message is Login)
                        {
                            OnServerMessage_LoginRequest(msg, (Login)envelope.Message);
                        }
                        else
                            logger.Error("Unexpected message: {0}", envelope.Message);
                    }
                    break;

                case NetIncomingMessageType.DebugMessage:
                    logger.Debug(msg.ReadString());
                    break;

                case NetIncomingMessageType.VerboseDebugMessage:
                    logger.Info(msg.ReadString());
                    break;

                case NetIncomingMessageType.WarningMessage:
                    logger.Warn(msg.ReadString());
                    break;

                case NetIncomingMessageType.ErrorMessage:
                    logger.Error(msg.ReadString());
                    break;

                default:
                    logger.Trace("Unhandled message '{0}'", msg);
                    break;
            }
        }

        private void OnServerMessage_StatusChanged(NetIncomingMessage msg, NetConnectionStatus status)
        {
            logger.Trace("Status of {0} changed to {1}", msg.SenderConnection, status);
        }

        private void OnServerMessage_LoginRequest(NetIncomingMessage msg, Login message)
        {
            var client = new Client(message.ClientId, msg.SenderConnection);
            clients.Add(message.ClientId, client);

            var frame = simulator.FetchFullSyncFrame();
            SendMessage(client, frame);
        }

        private void OnServerMessage_ClientSyncFrame(NetIncomingMessage msg, CommandFrame frame)
        {
            simulator.ReceiveCommandFrame(frame);
        }

        private void SendMessage(Client client, object message)
        {
            var bytes = Serializer.Serialize(new MessageEnvelope() { Message = message });

            var msg = server.CreateMessage();
            msg.Write(bytes);

            client.Connection.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void SendMessageToAll(object message)
        {
            var bytes = Serializer.Serialize(new MessageEnvelope() { Message = message });

            var msg = server.CreateMessage();
            msg.Write(bytes);

            foreach (var client in clients.Values)
            {
                client.Connection.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        class Client
        {
            public readonly ClientId Id;
            public readonly NetConnection Connection;

            public Client(ClientId id, NetConnection connection)
            {
                Id = id;
                Connection = connection;
            }
        }
    }
}
