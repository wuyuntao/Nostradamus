using NLog;
using System.Collections.Generic;

namespace Nostradamus.Server
{
    public sealed class ServerActorContext : ActorContext
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Queue<Event> queuedEvents = new Queue<Event>();

        public ServerActorContext(Actor actor, ISnapshotArgs snapshot)
            : base(actor, snapshot)
        { }

        internal override void ApplyEvent(IEventArgs eventArgs)
        {
            base.ApplyEvent(eventArgs);

            var @event = new Event(Actor.Id, eventArgs);

            queuedEvents.Enqueue(@event);

            logger.Debug("{0} applied event {1}", Actor, @event);

            Timeline.AddPoint(Actor.Scene.Time + Actor.Scene.DeltaTime, Actor.Snapshot);
        }

        internal IEnumerable<Event> DequeueEvents()
        {
            while (queuedEvents.Count > 0)
                yield return queuedEvents.Dequeue();
        }
    }
}