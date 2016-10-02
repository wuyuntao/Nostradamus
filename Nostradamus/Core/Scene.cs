using NLog;
using Nostradamus.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Nostradamus
{
	public class Scene : Disposable
	{
		protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

		private readonly Simulator simulator;
		private readonly Dictionary<ActorId, ActorContext> actors = new Dictionary<ActorId, ActorContext>();

		public Scene(Simulator simulator)
		{
			this.simulator = simulator;

			simulator.InitializeScene(this);
		}

		protected override void DisposeManaged()
		{
			foreach (var context in actors.Values)
				context.Actor.Dispose();

			actors.Clear();

			base.DisposeManaged();
		}

		internal ActorContext CreateActorContext(Actor actor)
		{
			var context = simulator.CreateActorContext(actor);

			actors.Add(actor.Id, context);

			return context;
		}

		internal ActorContext GetActorContext(ActorId actorId)
		{
			ActorContext actor;
			actors.TryGetValue(actorId, out actor);
			return actor;
		}

		internal void Update(int time, int deltaTime)
		{
			Time = time;
			DeltaTime = deltaTime;

			foreach (var context in actors.Values)
				context.Update();

			OnUpdate();
		}

		protected internal virtual void OnUpdate()
		{ }

		public IEnumerable<Actor> Actors
		{
			get
			{
				return from context in actors.Values
					   select context.Actor;
			}
		}

		internal IEnumerable<ActorContext> ActorContexts
		{
			get { return actors.Values; }
		}

		public int Time { get; internal set; }

		public int DeltaTime { get; internal set; }
	}
}
