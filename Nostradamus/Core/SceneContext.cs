using NLog;
using System;

namespace Nostradamus
{
	public abstract class SceneContext
	{
		protected static Logger logger = LogManager.GetCurrentClassLogger();

		private Scene scene;

		internal virtual void Initialize(Scene scene)
		{
			if( this.scene != null )
				throw new InvalidOperationException( "Already initialized" );

			this.scene = scene;
		}

		internal virtual void Update(int deltaTime)
		{
			if( scene == null )
				throw new InvalidOperationException( "Not initialized yet" );

			scene.OnUpdate( deltaTime );

			foreach( var actor in scene.Actors )
				actor.Context.Update();

			scene.OnLateUpdate();
		}

		public void EnqueueCommand(ActorId actorId, int time, ICommandArgs command)
		{
			var actor = scene.GetActor( actorId );
			if( actor == null )
				throw new ArgumentException( string.Format( "{0} not exist", actorId ) );

			actor.CommandQueue.Enqueue( time, command );
		}

		protected Scene Scene
		{
			get { return scene; }
		}
	}
}