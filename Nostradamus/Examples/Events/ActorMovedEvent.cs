
namespace Nostradamus.Examples
{
	public class ActorMovedEvent : IEventArgs
	{
		public float PositionX { get; set; }

		public float PositionY { get; set; }
	}
}