using System.Collections.Generic;

namespace Nostradamus
{
	class ActorContext
	{
		private Actor actor;
		private Queue<Command> queuedCommands = new Queue<Command>();
		private Queue<Event> queuedEvents = new Queue<Event>();

		public ActorContext(Actor actor)
		{
			this.actor = actor;
		}

		public void EnqueueEvent(IEventArgs @event)
		{
			queuedEvents.Enqueue(new Event(actor.Id, @event));
		}

		public Actor Actor
		{
			get { return actor; }
		}
	}
}
