using System.Collections.Generic;

namespace Nostradamus
{
	class CommandQueue
	{
		private Actor actor;
		private int maxCommandSequence;
		private Queue<Command> commands = new Queue<Command>();

		public CommandQueue(Actor actor)
		{
			this.actor = actor;
		}

		public Command Enqueue(int time, ICommandArgs commandArgs)
		{
			var command = new Command( actor.Id, time, ++maxCommandSequence, commandArgs );

			commands.Enqueue( command );

			return command;
		}

		public IEnumerable<Command> DequeueBefore(int time)
		{
			while( commands.Count > 0 )
			{
				var command = commands.Peek();
				if( command.Time < time )
				{
					yield return commands.Dequeue();
				}
				else
				{
					yield break;
				}
			}
		}

		public int MaxCommandSequence
		{
			get { return maxCommandSequence; }
		}
	}
}
