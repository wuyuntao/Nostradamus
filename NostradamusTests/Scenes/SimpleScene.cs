using Nostradamus.Tests.Actors;
using Nostradamus.Tests.Snapshots;

namespace Nostradamus.Tests.Scenes
{
	class SimpleScene : Scene
	{
		private SimpleCharacter character;

		protected internal override void OnUpdate(int deltaTime)
		{
			base.OnUpdate(deltaTime);

			if (Time == 0)
			{
				character = new SimpleCharacter(this, CreateActorId(), 0, new ActorSnapshot());

				AddActor(character);
			}
		}

		public SimpleCharacter Character
		{
			get { return character; }
		}
	}
}
