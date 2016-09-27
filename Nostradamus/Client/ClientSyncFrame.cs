using System.Collections.Generic;

namespace Nostradamus.Client
{
	public sealed class ClientSyncFrame
	{
		public readonly ClientId ClientId;
		public readonly int Time;
		public readonly List<Command> Commands = new List<Command>();

		public ClientSyncFrame(ClientId clientId, int time, IEnumerable<Command> commands = null)
		{
			ClientId = clientId;
			Time = time;

			if (commands != null)
				Commands.AddRange(commands);
		}
	}
}
