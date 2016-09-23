using System;
using System.Collections.Generic;

namespace Nostradamus
{
	class Node
	{
		private readonly Branch branch;
		private readonly int time;
		private List<IEventArgs> events = new List<IEventArgs>();

		public Node(Branch branch, int time)
		{
			this.branch = branch;
			this.time = time;
		}

		public void AddEvent(IEventArgs @event)
		{
			events.Add( @event );
		}

		public Branch Branch
		{
			get { return branch; }
		}

		public int Time
		{
			get { return time; }
		}
	}
}
