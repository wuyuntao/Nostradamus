using System.Collections.Generic;
using System.Linq;

namespace Nostradamus.Networking
{
	class ServerSceneContext : SceneContext
	{
		private List<ClientSynchronizationFrame> frames = new List<ClientSynchronizationFrame>();
		private List<Event> events = new List<Event>();

		public ServerSceneContext(Scene scene)
			: base(scene)
		{
		}

		internal override ActorContext CreateActorContext(Actor actor, int time, ISnapshotArgs snapshot)
		{
			return new ServerActorContext(this, actor, time, snapshot);
		}

		public void OnSynchronization(ClientSynchronizationFrame frame)
		{
			frames.Add(frame);
		}

		public ServerSynchronizationFrame Update(int deltaTime)
		{
			events.Clear();

			EnqueueCommands(Scene.Time + Scene.DeltaTime + deltaTime);

			Scene.OnUpdate(deltaTime);

			var actors = Scene.Actors.ToArray();

			foreach (var actor in actors)
			{
				actor.Context.Update();
			}

			return new ServerSynchronizationFrame(Scene.Time + Scene.DeltaTime, events.ToArray());
		}

		private void EnqueueCommands(int time)
		{
			frames.RemoveAll(frame =>
			{
				if (frame.Time < time)
				{
					foreach (var command in frame.Commands)
					{
						var actor = Scene.GetActor(command.ActorId);
						if (actor == null)
						{
							logger.Warn("Cannot find actor {0}", command.ActorId);
							continue;
						}

						actor.CommandQueue.Enqueue(Scene.Time, command.Args);
					}

					return true;
				}
				else
					return false;
			});
		}

		internal void EnqueueEvent(Event @event)
		{
			events.Add(@event);
		}
	}
}
