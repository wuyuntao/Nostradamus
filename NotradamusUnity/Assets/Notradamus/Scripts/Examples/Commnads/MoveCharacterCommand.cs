
namespace Nostradamus.Examples
{
	public class MoveCharacterCommand : ICommandArgs
	{
		public float DeltaX { get; set; }

		public float DeltaY { get; set; }
	}
}
