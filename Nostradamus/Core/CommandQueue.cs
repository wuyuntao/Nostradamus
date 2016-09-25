using System.Collections.Generic;

namespace Nostradamus
{
	class CommandQueue
	{
		int maxCommandSequence;
		Queue<Command> commands = new Queue<Command>();

		public Command Enqueue(int time, ICommandArgs commandArgs)
		{
			var command = new Command( time, ++maxCommandSequence, commandArgs );

			commands.Enqueue( command );

			return command;
		}

		public Command Dequeue(int sequence)
		{
			Command command = null;
			while( commands.Count > 0 && commands.Peek().Sequence <= sequence )
				command = commands.Dequeue();

			return command;
		}

		public int MaxCommandSequence
		{
			get { return maxCommandSequence; }
		}
	}
}
