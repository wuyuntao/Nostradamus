using FlatBuffers;
using FlatBuffers.Schema;
using Nostradamus.Networking;

namespace Nostradamus
{
    public interface ICommandArgs
    {
    }

    public sealed class Command
    {
        public readonly ClientId ClientId;

        public readonly ActorId ActorId;

        public readonly int Sequence;

        public readonly ICommandArgs Args;

        public int Time { get; private set; }

        public int DeltaTime { get; private set; }

        internal Command(ClientId clientId, ActorId actorId, int sequence, int time, int deltaTime, ICommandArgs args)
        {
            ClientId = clientId;
            ActorId = actorId;
            Time = time;
            DeltaTime = deltaTime;
            Sequence = sequence;
            Args = args;
        }

        internal Command(ClientId clientId, ActorId actorId, int sequence, ICommandArgs args)
            : this(clientId, actorId, sequence, 0, 0, args)
        { }

        internal void ChangeApplyTime(int time, int deltaTime)
        {
            Time = time;
            DeltaTime = deltaTime;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}, #{2})", GetType().Name, ActorId, Sequence);
        }
    }

    class CommandSerializer : Serializer<Command, Schema.Command>
    {
        public static readonly CommandSerializer Instance = new CommandSerializer();

        public override Offset<Schema.Command> Serialize(FlatBufferBuilder fbb, Command command)
        {
            var oClientIdDesc = string.IsNullOrEmpty(command.ClientId.Description) ? default(StringOffset) : fbb.CreateString(command.ClientId.Description);
            var oClientId = Schema.ClientId.CreateClientId(fbb, command.ClientId.Value, oClientIdDesc);

            var oActorIdDesc = string.IsNullOrEmpty(command.ActorId.Description) ? default(StringOffset) : fbb.CreateString(command.ActorId.Description);
            var oActorId = Schema.ActorId.CreateActorId(fbb, command.ActorId.Value, oActorIdDesc);

            var oArgs = MessageEnvelopeSerializer.Instance.Serialize(fbb, new Networking.MessageEnvelope(command.Args));

            return Schema.Command.CreateCommand(fbb, oClientId, oActorId, command.Sequence, command.Time, command.DeltaTime, oArgs);
        }

        public override Command Deserialize(Schema.Command command)
        {
            var clientId = ClientIdSerializer.Instance.Deserialize(command.ClientId.Value);
            var actorId = ActorIdSerializer.Instance.Deserialize(command.ActorId.Value);
            var args = MessageEnvelopeSerializer.Instance.Deserialize(command.Args.Value);

            return new Command(clientId, actorId, command.Sequence, (ICommandArgs)args.Message);
        }

        protected override Schema.Command GetRootAs(ByteBuffer buffer)
        {
            return Schema.Command.GetRootAsCommand(buffer);
        }
    }
}