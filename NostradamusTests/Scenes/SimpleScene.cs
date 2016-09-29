using Nostradamus.Tests.Actors;
using Nostradamus.Tests.Snapshots;

namespace Nostradamus.Tests.Scenes
{
	class SimpleScene : Scene
	{
		private SimpleCharacter character;

		protected override void OnUpdate()
		{
			base.OnUpdate();

			if (Time == 0)
			{
				character = new SimpleCharacter(this, CreateActorId(), new ActorSnapshot());

				AddActor(character);
			}
		}

		public SimpleCharacter Character
		{
			get { return character; }
		}
	}
}
