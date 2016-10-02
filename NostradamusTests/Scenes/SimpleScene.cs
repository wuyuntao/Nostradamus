using Nostradamus.Tests.Actors;
using Nostradamus.Tests.Snapshots;

namespace Nostradamus.Tests.Scenes
{
	class SimpleScene : Scene
	{
		private SimpleCharacter character;

		public SimpleScene(Simulator simulator)
			: base(simulator)
		{

			character = new SimpleCharacter(this, new ActorId(1), new ClientId(1), new CharacterSnapshot());
		}

		public SimpleCharacter Character
		{
			get { return character; }
		}
	}
}
