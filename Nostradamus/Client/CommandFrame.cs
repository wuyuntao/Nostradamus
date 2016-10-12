using System.Collections.Generic;

namespace Nostradamus.Client
{
	public sealed class CommandFrame
	{
        public readonly ClientId ClientId;

        public readonly List<Command> Commands;

		public CommandFrame(ClientId clientId)
		{
			ClientId = clientId;
			Commands = new List<Command>();
		}
	}
}