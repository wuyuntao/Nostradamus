using NLog;
using Nostradamus.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Nostradamus
{
	public class Scene : Disposable
	{
		protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

		private readonly Dictionary<ActorId, ActorContext> actors = new Dictionary<ActorId, ActorContext>();

		internal ActorContext CreateActorContext(Actor actor)
		{
			var context = new ActorContext(actor);

			actors.Add(actor.Id, context);

			return context;
		}

		protected internal virtual void OnUpdate()
		{
			foreach (var context in actors.Values)
				context.Actor.OnUpdate();
		}

		public IEnumerable<Actor> Actors
		{
			get
			{
				return from context in actors.Values
					   select context.Actor;
			}
		}

		public int Time { get; internal set; }

		public int DeltaTime { get; internal set; }
	}
}
