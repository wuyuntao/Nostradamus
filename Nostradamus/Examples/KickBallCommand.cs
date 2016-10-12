
namespace Nostradamus.Examples
{
    public class KickBallCommand : ICommandArgs
    {
        public readonly float InputX;

        public readonly float InputY;

        public readonly float InputZ;

        public KickBallCommand(float x = 0, float y = 0, float z = 0)
        {
            InputX = x;
            InputY = y;
            InputZ = z;
        }
    }
}
