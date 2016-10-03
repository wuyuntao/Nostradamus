using Lidgren.Network;
using NLog;
using Nostradamus.Client;
using Nostradamus.Server;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace Nostradamus.Networking
{
	class ReliableUdpClient
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		private readonly ClientSimulator simulator;
		private readonly IPEndPoint serverEndPoint;
		private readonly NetClient client;
		private readonly int simulateDeltaTime;
		private bool stopRequest;

		public ReliableUdpClient(ClientSimulator simulator, int simulateDeltaTime, IPEndPoint serverEndPoint, string appIdentifier = "Nostradamus", int simulateLatency = 0, int simulateLoss = 0)
		{
			var clientConf = new NetPeerConfiguration(appIdentifier);
			clientConf.AutoFlushSendQueue = true;
			//clientConf.SimulatedRandomLatency = simulateLatency;
			//clientConf.SimulatedLoss = simulateLoss;

			client = new NetClient(clientConf);
		}

		public void Start()
		{
			if (client.Status != NetPeerStatus.NotRunning)
				throw new InvalidOperationException("Already started");

			var timer = Stopwatch.StartNew();

			client.Start();
			client.Connect(serverEndPoint);
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

				for (var msg = client.ReadMessage(); msg != null; msg = client.ReadMessage())
					OnClientMessage(msg);

				simulator.Simulate(simulateDeltaTime);

				var commandFrame = simulator.FetchCommandFrame();
				SendMessage(commandFrame);
			}
		}

		private void OnClientMessage(NetIncomingMessage msg)
		{
			switch (msg.MessageType)
			{
				case NetIncomingMessageType.StatusChanged:
					{
						var status = (NetConnectionStatus)msg.ReadByte();

						OnClientMessage_StatusChanged(msg, status);
					}
					break;

				case NetIncomingMessageType.Data:
					{
						var envelope = Serializer.Deserialize<MessageEnvelope>(msg.Data);

						if (envelope.Message is DeltaSyncFrame)
						{
							OnServerMessage_DeltaSyncFrame(msg, (DeltaSyncFrame)envelope.Message);
						}
						else if (envelope.Message is FullSyncFrame)
						{
							OnServerMessage_FullSyncFrame(msg, (FullSyncFrame)envelope.Message);
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

		private void OnServerMessage_DeltaSyncFrame(NetIncomingMessage msg, DeltaSyncFrame message)
		{
			simulator.ReceiveDeltaSyncFrame(message);
		}

		private void OnServerMessage_FullSyncFrame(NetIncomingMessage msg, FullSyncFrame message)
		{
			simulator.ReceiveFullSyncFrame(message);
		}

		private void OnClientMessage_StatusChanged(NetIncomingMessage msg, NetConnectionStatus status)
		{
			logger.Trace("Status of {0} changed to {1}", msg.SenderConnection, status);

			switch (status)
			{

				case NetConnectionStatus.Connected:
					SendMessage(new Login() { ClientId = simulator.ClientId });
					break;

				case NetConnectionStatus.Disconnected:
					Stop();
					break;
			}
		}

		private void SendMessage(object message)
		{
			var bytes = Serializer.Serialize(new MessageEnvelope() { Message = message });

			var msg = client.CreateMessage();
			msg.Write(bytes);

			client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
		}
	}
}
