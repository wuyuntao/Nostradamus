using NLog;
using Nostradamus.Utils;

namespace Nostradamus
{
	public abstract class Simulator : Disposable
	{
		protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

		private Scene scene;

		protected Simulator(Scene scene)
		{
			this.scene = scene;
		}

		protected override void DisposeManaged()
		{
			SafeDispose(ref scene);

			base.DisposeManaged();
		}

		protected Scene Scene
		{
			get { return scene; }
		}
	}
}
