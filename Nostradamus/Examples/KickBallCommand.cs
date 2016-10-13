using FlatBuffers;
using Nostradamus.Networking;

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

    public class KickBallCommandSerializer : Serializer<KickBallCommand, Schema.KickBallCommand>
    {
        public static readonly KickBallCommandSerializer Instance = new KickBallCommandSerializer();

        public override Offset<Schema.KickBallCommand> Serialize(FlatBufferBuilder fbb, KickBallCommand command)
        {
            return Schema.KickBallCommand.CreateKickBallCommand(fbb, command.InputX, command.InputY, command.InputZ);
        }

        public override KickBallCommand Deserialize(Schema.KickBallCommand command)
        {
            return new KickBallCommand(command.InputX, command.InputY, command.InputZ);
        }

        public override Schema.KickBallCommand ToFlatBufferObject(ByteBuffer buffer)
        {
            return Schema.KickBallCommand.GetRootAsKickBallCommand(buffer);
        }
    }
}
