using System.Collections.Generic;

namespace Nostradamus.Client
{
	public sealed class CommandFrame
	{
		public ClientId ClientId { get; set; }

		public List<Command> Commands { get; set; }

		public CommandFrame(ClientId clientId)
		{
			ClientId = clientId;
			Commands = new List<Command>();
		}
	}
}