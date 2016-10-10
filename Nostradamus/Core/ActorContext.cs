using NLog;
using System;
using System.Collections.Generic;

namespace Nostradamus
{
    public abstract class ActorContext
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private Actor actor;
        private ISnapshotArgs actorSnapshot;
        private Timeline timeline;

        private int? lastCommandSeq;
        private Queue<Command> queuedCommands = new Queue<Command>();

        protected ActorContext(Actor actor, ISnapshotArgs snapshot)
        {
            if (actor == null)
                throw new ArgumentNullException("actor");

            this.actor = actor;
            this.actorSnapshot = snapshot;

            timeline = new Timeline();
            timeline.AddPoint(actor.Scene.Time, snapshot);
        }

        internal virtual void ApplyEvent(IEventArgs @event)
        {
            var newSnapshot = actor.OnEventApplied(@event);
            if (newSnapshot == null)
                throw new InvalidOperationException("Snapshot cannot be null");

            actorSnapshot = newSnapshot;
        }

        public virtual ISnapshotArgs InterpolateSnapshot(int time)
        {
            var timepoint = timeline.InterpolatePoint(time);

            return timepoint != null ? timepoint.Snapshot : null;
        }

        public void EnqueueCommand(Command command)
        {
            queuedCommands.Enqueue(command);
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

        internal ISnapshotArgs ActorSnapshot
        {
            get
            {
                actor.Scene.Context.ThrowUnlessSimulating();

                return actorSnapshot;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("snapshot");

                actor.Scene.Context.ThrowUnlessSimulating();

                actorSnapshot = value;
            }
        }

        protected Timeline Timeline
        {
            get { return timeline; }
        }

        public int? LastCommandSeq
        {
            get { return lastCommandSeq; }
        }
    }
}