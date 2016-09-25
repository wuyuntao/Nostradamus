namespace Nostradamus
{
	public abstract class Actor
	{
		private ActorId id;
		private Scene scene;

		protected Actor(Scene scene, ActorId id)
		{
			this.scene = scene;
			this.id = id;
		}

		protected Scene Scene
		{
			get { return scene; }
		}

		protected ActorId Id
		{
			get { return id; }
		}
	}
}
