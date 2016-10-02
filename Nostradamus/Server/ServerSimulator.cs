using Nostradamus.Client;
using System.Collections.Generic;
using System.Linq;

namespace Nostradamus.Server
{
	public sealed class ServerSimulator : Simulator
	{
		private DeltaSyncFrame deltaSyncFrame;
		private readonly Queue<CommandFrame> clientSyncFrames = new Queue<CommandFrame>();

		public ServerSimulator()
		{ }

		internal override ActorContext CreateActorContext(Actor actor)
		{
			return new ServerActorContext(actor);
		}

		public void ReceiveCommandFrame(CommandFrame frame)
		{
			foreach (var command in frame.Commands)
			{
				// TODO: Avoid get actor context for every command
				var actorContext = Scene.GetActorContext(command.ActorId);
				if (actorContext == null)
				{
					logger.Warn("Cannot find actor '{0}' to enqueue command '{1}'",
							command.ActorId, command.GetArgs());
				}
				else if (actorContext.Actor.OwnerId != frame.ClientId)
				{
					logger.Warn("Client id not match. '{0}' != '{1}'",
							actorContext.Actor.OwnerId, frame.ClientId);
				}
				else
				{
					actorContext.EnqueueCommand(command);
				}
			}
		}

		public void Simulate(int deltaTime)
		{
			Scene.Update(Time, deltaTime);

			Time += deltaTime;
		}

		public DeltaSyncFrame FetchDeltaSyncFrame()
		{
			var frame = new DeltaSyncFrame(Scene.Time, Scene.DeltaTime);

			foreach (var actorContext in ActorContexts)
			{
				frame.Events.AddRange(actorContext.DequeueEvents());

				if (actorContext.LastCommandSeq != null)
				{
					frame.LastCommandSeqs[actorContext.Actor.OwnerId] = actorContext.LastCommandSeq.Value;
				}
			}

			return frame;
		}

		public FullSyncFrame FetchFullSyncFrame()
		{
			var frame = new FullSyncFrame(Scene.Time, Scene.DeltaTime);

			frame.Snapshots.AddRange(from actorContext in ActorContexts
									 select actorContext.CreateSnapshot());

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
