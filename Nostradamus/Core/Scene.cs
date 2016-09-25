using NLog;
using System.Collections.Generic;
using System;

namespace Nostradamus
{
	public class Scene
	{
		protected static Logger logger = LogManager.GetCurrentClassLogger();

		private readonly SceneContext context;
		private int maxActorId;
		private readonly Dictionary<ActorId, Actor> actors = new Dictionary<ActorId, Actor>();
		private readonly Dictionary<ActorId, Actor> actorsToAdd = new Dictionary<ActorId, Actor>();
		private readonly Dictionary<ActorId, Actor> actorsToRemove = new Dictionary<ActorId, Actor>();
		private int time;
		private int deltaTime;

		public Scene(SceneContext context)
		{
			this.context = context;

			context.Initialize( this );
		}

		public ActorId CreateActorId(string description = null)
		{
			return new ActorId( ++maxActorId, description );
		}

		internal Actor GetActor(ActorId actorId)
		{
			Actor actor;
			if( !actors.TryGetValue( actorId, out actor ) )
				actorsToAdd.TryGetValue( actorId, out actor );

			return actor;
		}

		public void AddActor(Actor actor)
		{
			actorsToAdd.Add( actor.Id, actor );
		}

		public void RemoveActor(Actor actor)
		{
			actorsToRemove.Add( actor.Id, actor );
		}

		internal protected virtual void OnUpdate(int deltaTime)
		{
			this.deltaTime = deltaTime;

			if( actorsToAdd.Count > 0 )
			{
				foreach( var actor in actorsToAdd.Values )
					actors.Add( actor.Id, actor );

				actorsToAdd.Clear();
			}

			if( actorsToRemove.Count > 0 )
			{
				foreach( var actor in actorsToRemove.Values )
					actors.Remove( actor.Id );

				actorsToRemove.Clear();
			}
		}

		internal protected virtual void OnLateUpdate()
		{
			time += deltaTime;
		}

		internal SceneContext Context
		{
			get { return context; }
		}

		public int Time
		{
			get { return time; }
		}

		public int DeltaTime
		{
			get { return deltaTime; }
		}

		public IEnumerable<Actor> Actors
		{
			get { return actors.Values; }
		}
	}
}
