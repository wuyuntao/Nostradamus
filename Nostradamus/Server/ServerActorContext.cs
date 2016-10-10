using NLog;
using System.Collections.Generic;

namespace Nostradamus.Server
{
    public sealed class ServerActorContext : ActorContext
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Queue<Event> eventsApplied = new Queue<Event>();

        internal ServerActorContext(Actor actor, ISnapshotArgs snapshot)
            : base(actor, snapshot)
        { }

        internal override void ApplyEvent(IEventArgs eventArgs)
        {
            base.ApplyEvent(eventArgs);

            // Enqueue events
            var @event = new Event(Actor.Id, eventArgs);

            eventsApplied.Enqueue(@event);

            logger.Debug("{0} applied event {1}", Actor, @event);

            // Save current snapshot to timeline
            Timeline.AddPoint(Actor.Scene.Time + Actor.Scene.DeltaTime, ActorSnapshot);
        }

        internal IEnumerable<Event> FetchAllEvents()
        {
            while (eventsApplied.Count > 0)
                yield return eventsApplied.Dequeue();
        }
    }
}