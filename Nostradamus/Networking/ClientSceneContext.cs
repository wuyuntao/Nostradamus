using System;
using System.Collections.Generic;
using System.Linq;

namespace Nostradamus.Networking
{
	class ClientSceneContext : SceneContext
	{
		private int clientId;
		private Queue<ServerSynchronizationFrame> frames = new Queue<ServerSynchronizationFrame>();
		private List<Command> commands = new List<Command>();

		public ClientSceneContext(Scene scene, int clientId)
			: base(scene)
		{
			this.clientId = clientId;
		}

		internal override ActorContext CreateActorContext(Actor actor, int time, ISnapshotArgs snapshot)
		{
			return new ClientActorContext(this, actor, time, snapshot);
		}

		public void OnSynchronization(ServerSynchronizationFrame frame)
		{
			frames.Enqueue(frame);
		}

		public ClientSynchronizationFrame Update(int deltaTime)
		{
			Synchronize();

			Scene.OnUpdate(deltaTime);

			var actors = Scene.Actors.ToArray();

			foreach (var actor in actors)
			{
				actor.Context.Update();
			}

			var frame = new ClientSynchronizationFrame(clientId, Scene.Time + Scene.DeltaTime, commands.ToArray());
			commands.Clear();

			return frame;
		}

		private void Synchronize()
		{
			while (frames.Count > 0)
			{
				var frame = frames.Dequeue();

				// Enqueue events
				if (frame.Events != null && frame.Events.Length > 0)
				{
					foreach (var @event in frame.Events)
					{
						var actor = Scene.GetActor(@event.ActorId);
						if (actor == null)
							throw new InvalidOperationException(string.Format("Cannot find actor {0}", @event.ActorId));

						var actorContext = (ClientActorContext)actor.Context;
						actorContext.EnqueueAuthoritativeEvent(@event);
					}
				}

				// Apply events
				foreach (var actor in Scene.Actors)
				{
					var actorContext = (ClientActorContext)actor.Context;
					var snapshot = actorContext.ApplyAuthoritativeEvents(frame.Time);
				}
			}
		}

		public void EnqueueCommand(ActorId actorId, int time, ICommandArgs commandArgs)
		{
			var actor = Scene.GetActor(actorId);
			if (actor == null)
				throw new InvalidOperationException(string.Format("Cannot find actor {0}", actorId));

			var command = actor.CommandQueue.Enqueue(time, commandArgs);
			commands.Add(command);
		}
	}
}
