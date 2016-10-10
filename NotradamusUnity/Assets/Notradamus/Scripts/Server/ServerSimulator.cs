using Nostradamus.Client;
using System.Collections.Generic;
using System.Linq;

namespace Nostradamus.Server
{
    public sealed class ServerSimulator : Simulator
    {
        internal override ActorContext CreateActorContext(Actor actor, ISnapshotArgs snapshot)
        {
            return new ServerActorContext(actor, snapshot);
        }

        public void ReceiveCommandFrame(CommandFrame frame)
        {
            foreach (var command in frame.Commands)
            {
                // TODO: Avoid get actor context for every command
                var context = Scene.GetActorContext(command.ActorId);
                if (context == null)
                {
                    logger.Warn("Cannot find actor '{0}' to enqueue command '{1}'",
                            command.ActorId, command.Args);
                }
                else if (context.Actor.OwnerId != frame.ClientId)
                {
                    logger.Warn("Client id not match. '{0}' != '{1}'",
                            context.Actor.OwnerId, frame.ClientId);
                }
                else
                {
                    context.EnqueueCommand(command);
                }
            }
        }

        public void Simulate(int deltaTime)
        {
            Scene.Update(Time, deltaTime);

            foreach (var context in ActorContexts)
                context.CreateTimepoint();

            logger.Debug("Simulated {0} / {1}", Time, deltaTime);

            Time += deltaTime;
        }

        public FullSyncFrame FetchFullSyncFrame()
        {
            var frame = new FullSyncFrame(Scene.Time, Scene.DeltaTime);

            frame.Snapshots.AddRange(from context in ActorContexts
                                     select context.Actor into actor
                                     select new Snapshot(actor.Id, actor.Snapshot));

            return frame;
        }

        public DeltaSyncFrame FetchDeltaSyncFrame()
        {
            var frame = new DeltaSyncFrame(Scene.Time, Scene.DeltaTime);

            foreach (var context in ActorContexts)
            {
                frame.Events.AddRange(context.DequeueEvents());

                if (context.LastCommandSeq != null)
                {
                    frame.LastCommandSeqs[context.Actor.OwnerId] = context.LastCommandSeq.Value;
                }
            }

            return frame;
        }

        private IEnumerable<ServerActorContext> ActorContexts
        {
            get
            {
                return from context in Scene.ActorContexts
                       select (ServerActorContext)context;
            }
        }
    }
}
