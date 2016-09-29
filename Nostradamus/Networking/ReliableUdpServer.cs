using Lidgren.Network;
using NLog;
using Nostradamus.Client;
using Nostradamus.Server;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Nostradamus.Networking
{
	public sealed class ReliableUdpServer
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		private readonly ServerSimulator simulator;
		private readonly NetServer server;
		private readonly SortedList<ClientId, Client> clients = new SortedList<ClientId, Client>();
		private int simulateDeltaTime;
		private bool stopRequest;

		public ReliableUdpServer(ServerSimulator simulator, int simulateDeltaTime, string appIdentifier, int listeningPort)
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
				var waitTime = (int)timer.ElapsedMilliseconds - simulateTime;
				if (waitTime > 0)
					Thread.Sleep(waitTime);

				for (var msg = server.ReadMessage(); msg != null; msg = server.ReadMessage())
					OnServerMessage(msg);

				var syncFrame = simulator.Simulate(simulateDeltaTime);
				SendServerSyncFrame(syncFrame);
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
						using (var stream = new MemoryStream(msg.Data))
						{
							var envelope = Serializer.Deserialize<MessageEnvelope>(stream);

							if (envelope.Message is ClientSyncFrame)
							{
								OnServerMessage_ClientSyncFrame(msg, (ClientSyncFrame)envelope.Message);
							}
							else if (envelope.Message is LoginRequest)
							{
								OnServerMessage_LoginRequest(msg, (LoginRequest)envelope.Message);
							}
							else
								logger.Error("Unexpected message: {0}", envelope.Message);
						}
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

		private void OnServerMessage_LoginRequest(NetIncomingMessage msg, LoginRequest message)
		{
			clients.Add(message.ClientId, new Client(message.ClientId, msg.SenderConnection));
		}

		private void OnServerMessage_ClientSyncFrame(NetIncomingMessage msg, ClientSyncFrame frame)
		{
			simulator.AddClientSyncFrame(frame);
		}

		private void SendServerSyncFrame(ServerSyncFrame frame)
		{
			using (var stream = new MemoryStream())
			{
				Serializer.Serialize(stream, new MessageEnvelope() { Message = frame });

				var msg = server.CreateMessage();
				msg.Write(stream.ToArray());

				foreach (var client in clients.Values)
				{
					client.Connection.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
				}
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
