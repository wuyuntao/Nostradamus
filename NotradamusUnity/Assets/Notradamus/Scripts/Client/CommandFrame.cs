using FlatBuffers;
using Nostradamus.Networking;
using System.Collections.Generic;

namespace Nostradamus.Client
{
    public sealed class CommandFrame
    {
        public readonly ClientId ClientId;

        public readonly List<Command> Commands;

        public CommandFrame(ClientId clientId)
        {
            ClientId = clientId;
            Commands = new List<Command>();
        }

        public override string ToString()
        {
            return string.Format("{0} (Commands: {1})", GetType().Name, Commands.Count);
        }
    }

    class CommandFrameSerializer : Serializer<CommandFrame, Schema.CommandFrame>
    {
        public static readonly CommandFrameSerializer Instance = new CommandFrameSerializer();

        public override Offset<Schema.CommandFrame> Serialize(FlatBufferBuilder fbb, CommandFrame frame)
        {
            var oCommands = new Offset<Schema.Command>[frame.Commands.Count];
            for (int i = 0; i < frame.Commands.Count; i++)
            {
                var oCommand = CommandSerializer.Instance.Serialize(fbb, frame.Commands[i]);

                oCommands[i] = oCommand;
            }
            var voCommands = Schema.CommandFrame.CreateCommandsVector(fbb, oCommands);

            var oClientIdDesc = string.IsNullOrEmpty(frame.ClientId.Description) ? default(StringOffset) : fbb.CreateString(frame.ClientId.Description);
            var oClientId = Schema.ClientId.CreateClientId(fbb, frame.ClientId.Value, oClientIdDesc);

            return Schema.CommandFrame.CreateCommandFrame(fbb, oClientId, voCommands);
        }

        public override CommandFrame Deserialize(Schema.CommandFrame frame)
        {
            var clientId = new ClientId(frame.ClientId.Value.Value, frame.ClientId.Value.Description);
            var commandFrame = new CommandFrame(clientId);

            for (int i = 0; i < frame.CommandsLength; i++)
            {
                var command = frame.Commands(i).Value;

                commandFrame.Commands.Add(CommandSerializer.Instance.Deserialize(command));
            }

            return commandFrame;
        }

        public override Schema.CommandFrame ToFlatBufferObject(ByteBuffer buffer)
        {
            return Schema.CommandFrame.GetRootAsCommandFrame(buffer);
        }
    }
}
