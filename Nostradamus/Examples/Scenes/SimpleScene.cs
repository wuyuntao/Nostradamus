using System;

namespace Nostradamus.Examples
{
	public class SimpleScene : Scene
	{
		private SimpleCharacter character;

		public SimpleScene(Simulator simulator)
			: base(simulator)
		{
			character = new SimpleCharacter(this, new ActorId(1), new ClientId(1), new CharacterSnapshot());
		}

		protected override Actor CreateActor(ActorId actorId, ISnapshotArgs snapshot)
		{
			if (actorId.Value == 1)
				return new SimpleCharacter(this, new ActorId(1), new ClientId(1), new CharacterSnapshot());

			throw new NotSupportedException(actorId.ToString());
		}

		public SimpleCharacter Character
		{
			get { return character; }
		}
	}
}
