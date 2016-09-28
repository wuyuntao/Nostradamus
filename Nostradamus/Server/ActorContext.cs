using Nostradamus.Utils;

namespace Nostradamus.Server
{
	class ActorContext : Disposable
	{
		private Actor actor;

		public ActorContext(Actor actor)
		{
			this.actor = actor;
		}

		protected override void DisposeManaged()
		{
			SafeDispose(ref actor);

			base.DisposeManaged();
		}

		public Actor Actor
		{
			get { return actor; }
		}
	}
}
