using NLog;
using System;
using System.Collections.Generic;

namespace Nostradamus
{
    abstract class ActorContext
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private Actor actor;
        private int? lastCommandSeq;
        private Queue<Command> queuedCommands = new Queue<Command>();
        private Queue<Event> queuedEvents = new Queue<Event>();

        protected ActorContext(Actor actor)
        {
            if (actor == null)
                throw new ArgumentNullException("actor");

            this.actor = actor;
        }

        public void EnqueueCommand(Command command)
        {
            queuedCommands.Enqueue(command);
        }

        public void EnqueueEvent(IEventArgs eventArgs)
        {
            var @event = new Event(actor.Id, eventArgs);

            queuedEvents.Enqueue(@event);

            logger.Debug("{0} applied event {1}", actor, @event);
        }

        public IEnumerable<Event> DequeueEvents()
        {
            while (queuedEvents.Count > 0)
                yield return queuedEvents.Dequeue();
        }

        public void Update()
        {
            lastCommandSeq = null;
            while (queuedCommands.Count > 0)
            {
                var command = queuedCommands.Dequeue();

                actor.OnCommandReceived(command.Args);

                lastCommandSeq = command.Sequence;

                logger.Debug("{0} received command {1}", actor, command);
            }

            actor.OnUpdate();
        }

        public Actor Actor
        {
            get { return actor; }
        }

        public int? LastCommandSeq
        {
            get { return lastCommandSeq; }
        }
    }
}