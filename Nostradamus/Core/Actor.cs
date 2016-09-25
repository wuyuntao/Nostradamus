namespace Nostradamus
{
	public abstract class Actor
	{
		private Scene scene;

		protected Actor(Scene scene)
		{
			this.scene = scene;
		}

		protected Scene Scene
		{
			get { return scene; }
		}
	}
}
