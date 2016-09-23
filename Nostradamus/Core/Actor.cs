namespace Nostradamus
{
	public abstract class Actor
	{
		private Scene scene;
		private WorldLine worldLine = new WorldLine();

		protected Actor(Scene scene)
		{
			this.scene = scene;
		}

		protected Scene Scene
		{
			get { return scene; }
		}

		internal WorldLine WorldLine
		{
			get { return worldLine; }
		}
	}
}
