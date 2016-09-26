using System.Collections.Generic;

namespace Nostradamus.Client
{
	public sealed class ClientSynchronizationFrame
	{
		public readonly int ClientId;
		public readonly int Time;
		public readonly List<Command> Commands = new List<Command>();

		public ClientSynchronizationFrame(int clientId, int time, IEnumerable<Command> commands = null)
		{
			ClientId = clientId;
			Time = time;

			if (commands != null)
				Commands.AddRange(commands);
		}
	}
}
