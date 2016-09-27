using NLog;
using System.Collections.Generic;

namespace Nostradamus
{
	public class Scene
	{
		protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

		private int maxActorId;
		private readonly Dictionary<ActorId, Actor> actors = new Dictionary<ActorId, Actor>();

		public ActorId CreateActorId(string description = null)
		{
			return new ActorId(++maxActorId, description);
		}

		internal Actor GetActor(ActorId actorId)
		{
			Actor actor;
			actors.TryGetValue(actorId, out actor);
			return actor;
		}

		public void AddActor(Actor actor)
		{
			actors.Add(actor.Id, actor);
		}

		public void RemoveActor(Actor actor)
		{
			actors.Remove(actor.Id);
		}

		internal protected virtual void OnUpdate()
		{
			foreach (var actor in actors.Values)
				actor.OnUpdate(time, deltaTime);
		}

		public IEnumerable<Actor> Actors
		{
			get { return actors.Values; }
		}

		public int Time { get; internal set; }

		public int DeltaTime { get; internal set; }
	}
}
