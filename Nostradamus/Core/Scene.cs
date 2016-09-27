using NLog;
using System.Collections.Generic;

namespace Nostradamus
{
	public class Scene
	{
		protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

		private int maxActorId;
		private readonly Dictionary<ActorId, Actor> actors = new Dictionary<ActorId, Actor>();

		internal delegate void CreateEventDelegate(Event @event);

		internal event CreateEventDelegate OnEventCreated;

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
				actor.OnUpdate();
		}

		public IEnumerable<Actor> Actors
		{
			get { return actors.Values; }
		}

		internal Event CreateEvent(Actor actor, int lastCommandSeq, IEventArgs @event)
		{
			var e = new Event(actor.Id, actor.OwnerId, Time, lastCommandSeq, @event);

			if (OnEventCreated != null)
				OnEventCreated(e);

			return e;
		}

		public int Time { get; internal set; }

		public int DeltaTime { get; internal set; }
	}
}
