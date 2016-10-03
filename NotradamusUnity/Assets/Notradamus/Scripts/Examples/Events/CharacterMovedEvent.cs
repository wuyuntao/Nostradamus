
namespace Nostradamus.Examples
{
	public class CharacterMovedEvent : IEventArgs
	{
		public float PositionX { get; set; }

		public float PositionY { get; set; }
	}
}