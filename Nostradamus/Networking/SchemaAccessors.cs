// Automatically generated by fbagen, do not modify
using FlatBuffers;
using FlatBuffers.Schema;
using System;

namespace Nostradamus.Schema.Mutable
{
    public partial class MessageEnvelope
    {
        public string Id { get; set; }

        public byte[] Data { get; set; }

    }

    public partial class ActorId
    {
        public int Value { get; set; }

        public string Description { get; set; }

    }

    public partial class ClientId
    {
        public int Value { get; set; }

        public string Description { get; set; }

    }

    public partial class Command
    {
        public ClientId ClientId { get; set; }

        public ActorId ActorId { get; set; }

        public int Sequence { get; set; }

        public int Time { get; set; }

        public int DeltaTime { get; set; }

        public MessageEnvelope Args { get; set; }

    }

    public partial class Event
    {
        public ActorId ActorId { get; set; }

        public MessageEnvelope Args { get; set; }

    }

    public partial class ActorSnapshot
    {
        public MessageEnvelope Desc { get; set; }

        public MessageEnvelope Args { get; set; }

    }

    public partial class SimulatorSnapshot
    {
        public ActorSnapshot[] Actors { get; set; }

    }

    public partial class CommandFrame
    {
        public ClientId ClientId { get; set; }

        public Command[] Commands { get; set; }

    }

    public partial class FullSyncFrame
    {
        public int Time { get; set; }

        public int DeltaTime { get; set; }

        public SimulatorSnapshot Snapshot { get; set; }

    }

    public partial class CommandSeq
    {
        public ClientId ClientId { get; set; }

        public int Sequence { get; set; }

    }

    public partial class DeltaSyncFrame
    {
        public int Time { get; set; }

        public int DeltaTime { get; set; }

        public Event[] Events { get; set; }

        public CommandSeq[] LastCommandSeqs { get; set; }

    }

    public partial class Login
    {
        public ClientId ClientId { get; set; }

    }

    public partial class Vector3
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

    }

    public partial class Quaternion
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float W { get; set; }

    }

    public partial class RigidBodyMovedEvent
    {
        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public Vector3 LinearVelocity { get; set; }

        public Vector3 AngularVelocity { get; set; }

    }

    public partial class RigidBodySnapshot
    {
        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public Vector3 LinearVelocity { get; set; }

        public Vector3 AngularVelocity { get; set; }

    }

    public partial class BallDesc
    {
        public ActorId Id { get; set; }

        public Vector3 InitialPosition { get; set; }

    }

    public partial class CubeDesc
    {
        public ActorId Id { get; set; }

        public Vector3 InitialPosition { get; set; }

    }

    public partial class KickBallCommand
    {
        public float InputX { get; set; }

        public float InputY { get; set; }

        public float InputZ { get; set; }

    }

    public partial class SceneInitializedEvent
    {
        public byte CubeRows { get; set; }

        public byte CubeColumns { get; set; }

    }

}

namespace Nostradamus.Schema.Serialization
{
    public class MessageEnvelopeSerializer : Serializer<Nostradamus.Schema.Mutable.MessageEnvelope, MessageEnvelope>
    {
        public static readonly MessageEnvelopeSerializer Instance = new MessageEnvelopeSerializer();

        public override Offset<MessageEnvelope> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.MessageEnvelope obj)
        {
            MessageEnvelope.StartMessageEnvelope(fbb);
            if (!string.IsNullOrEmpty(obj.Id))
                 MessageEnvelope.AddId(fbb, fbb.CreateString(obj.Id));
            MessageEnvelope.AddData(fbb, MessageEnvelope.CreateDataVector(fbb, obj.Data));
            return MessageEnvelope.EndMessageEnvelope(fbb);
        }

        protected override MessageEnvelope GetRootAs(ByteBuffer buffer)
        {
            return MessageEnvelope.GetRootAsMessageEnvelope(buffer);
        }

        public override Nostradamus.Schema.Mutable.MessageEnvelope Deserialize(MessageEnvelope obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.MessageEnvelope();
            accessor.Id = obj.Id;
            accessor.Data = DeserializeScalar(obj.DataLength, obj.Data);
            return accessor;
        }
    }

    public class ActorIdSerializer : Serializer<Nostradamus.Schema.Mutable.ActorId, ActorId>
    {
        public static readonly ActorIdSerializer Instance = new ActorIdSerializer();

        public override Offset<ActorId> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.ActorId obj)
        {
            ActorId.StartActorId(fbb);
            ActorId.AddValue(fbb, obj.Value);
            if (!string.IsNullOrEmpty(obj.Description))
                 ActorId.AddDescription(fbb, fbb.CreateString(obj.Description));
            return ActorId.EndActorId(fbb);
        }

        protected override ActorId GetRootAs(ByteBuffer buffer)
        {
            return ActorId.GetRootAsActorId(buffer);
        }

        public override Nostradamus.Schema.Mutable.ActorId Deserialize(ActorId obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.ActorId();
            accessor.Value = obj.Value;
            accessor.Description = obj.Description;
            return accessor;
        }
    }

    public class ClientIdSerializer : Serializer<Nostradamus.Schema.Mutable.ClientId, ClientId>
    {
        public static readonly ClientIdSerializer Instance = new ClientIdSerializer();

        public override Offset<ClientId> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.ClientId obj)
        {
            ClientId.StartClientId(fbb);
            ClientId.AddValue(fbb, obj.Value);
            if (!string.IsNullOrEmpty(obj.Description))
                 ClientId.AddDescription(fbb, fbb.CreateString(obj.Description));
            return ClientId.EndClientId(fbb);
        }

        protected override ClientId GetRootAs(ByteBuffer buffer)
        {
            return ClientId.GetRootAsClientId(buffer);
        }

        public override Nostradamus.Schema.Mutable.ClientId Deserialize(ClientId obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.ClientId();
            accessor.Value = obj.Value;
            accessor.Description = obj.Description;
            return accessor;
        }
    }

    public class CommandSerializer : Serializer<Nostradamus.Schema.Mutable.Command, Command>
    {
        public static readonly CommandSerializer Instance = new CommandSerializer();

        public override Offset<Command> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.Command obj)
        {
            Command.StartCommand(fbb);
            Command.AddClientId(fbb, ClientIdSerializer.Instance.Serialize(fbb, obj.ClientId));
            Command.AddActorId(fbb, ActorIdSerializer.Instance.Serialize(fbb, obj.ActorId));
            Command.AddSequence(fbb, obj.Sequence);
            Command.AddTime(fbb, obj.Time);
            Command.AddDeltaTime(fbb, obj.DeltaTime);
            Command.AddArgs(fbb, MessageEnvelopeSerializer.Instance.Serialize(fbb, obj.Args));
            return Command.EndCommand(fbb);
        }

        protected override Command GetRootAs(ByteBuffer buffer)
        {
            return Command.GetRootAsCommand(buffer);
        }

        public override Nostradamus.Schema.Mutable.Command Deserialize(Command obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.Command();
            accessor.ClientId = ClientIdSerializer.Instance.Deserialize(obj.ClientId);
            accessor.ActorId = ActorIdSerializer.Instance.Deserialize(obj.ActorId);
            accessor.Sequence = obj.Sequence;
            accessor.Time = obj.Time;
            accessor.DeltaTime = obj.DeltaTime;
            accessor.Args = MessageEnvelopeSerializer.Instance.Deserialize(obj.Args);
            return accessor;
        }
    }

    public class EventSerializer : Serializer<Nostradamus.Schema.Mutable.Event, Event>
    {
        public static readonly EventSerializer Instance = new EventSerializer();

        public override Offset<Event> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.Event obj)
        {
            Event.StartEvent(fbb);
            Event.AddActorId(fbb, ActorIdSerializer.Instance.Serialize(fbb, obj.ActorId));
            Event.AddArgs(fbb, MessageEnvelopeSerializer.Instance.Serialize(fbb, obj.Args));
            return Event.EndEvent(fbb);
        }

        protected override Event GetRootAs(ByteBuffer buffer)
        {
            return Event.GetRootAsEvent(buffer);
        }

        public override Nostradamus.Schema.Mutable.Event Deserialize(Event obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.Event();
            accessor.ActorId = ActorIdSerializer.Instance.Deserialize(obj.ActorId);
            accessor.Args = MessageEnvelopeSerializer.Instance.Deserialize(obj.Args);
            return accessor;
        }
    }

    public class ActorSnapshotSerializer : Serializer<Nostradamus.Schema.Mutable.ActorSnapshot, ActorSnapshot>
    {
        public static readonly ActorSnapshotSerializer Instance = new ActorSnapshotSerializer();

        public override Offset<ActorSnapshot> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.ActorSnapshot obj)
        {
            ActorSnapshot.StartActorSnapshot(fbb);
            ActorSnapshot.AddDesc(fbb, MessageEnvelopeSerializer.Instance.Serialize(fbb, obj.Desc));
            ActorSnapshot.AddArgs(fbb, MessageEnvelopeSerializer.Instance.Serialize(fbb, obj.Args));
            return ActorSnapshot.EndActorSnapshot(fbb);
        }

        protected override ActorSnapshot GetRootAs(ByteBuffer buffer)
        {
            return ActorSnapshot.GetRootAsActorSnapshot(buffer);
        }

        public override Nostradamus.Schema.Mutable.ActorSnapshot Deserialize(ActorSnapshot obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.ActorSnapshot();
            accessor.Desc = MessageEnvelopeSerializer.Instance.Deserialize(obj.Desc);
            accessor.Args = MessageEnvelopeSerializer.Instance.Deserialize(obj.Args);
            return accessor;
        }
    }

    public class SimulatorSnapshotSerializer : Serializer<Nostradamus.Schema.Mutable.SimulatorSnapshot, SimulatorSnapshot>
    {
        public static readonly SimulatorSnapshotSerializer Instance = new SimulatorSnapshotSerializer();

        public override Offset<SimulatorSnapshot> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.SimulatorSnapshot obj)
        {
            SimulatorSnapshot.StartSimulatorSnapshot(fbb);
            SimulatorSnapshot.AddActors(fbb, SimulatorSnapshot.CreateActorsVector(fbb, ActorSnapshotSerializer.Instance.Serialize(fbb, obj.Actors)));
            return SimulatorSnapshot.EndSimulatorSnapshot(fbb);
        }

        protected override SimulatorSnapshot GetRootAs(ByteBuffer buffer)
        {
            return SimulatorSnapshot.GetRootAsSimulatorSnapshot(buffer);
        }

        public override Nostradamus.Schema.Mutable.SimulatorSnapshot Deserialize(SimulatorSnapshot obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.SimulatorSnapshot();
            accessor.Actors = ActorSnapshotSerializer.Instance.Deserialize(obj.ActorsLength, obj.Actors);
            return accessor;
        }
    }

    public class CommandFrameSerializer : Serializer<Nostradamus.Schema.Mutable.CommandFrame, CommandFrame>
    {
        public static readonly CommandFrameSerializer Instance = new CommandFrameSerializer();

        public override Offset<CommandFrame> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.CommandFrame obj)
        {
            CommandFrame.StartCommandFrame(fbb);
            CommandFrame.AddClientId(fbb, ClientIdSerializer.Instance.Serialize(fbb, obj.ClientId));
            CommandFrame.AddCommands(fbb, CommandFrame.CreateCommandsVector(fbb, CommandSerializer.Instance.Serialize(fbb, obj.Commands)));
            return CommandFrame.EndCommandFrame(fbb);
        }

        protected override CommandFrame GetRootAs(ByteBuffer buffer)
        {
            return CommandFrame.GetRootAsCommandFrame(buffer);
        }

        public override Nostradamus.Schema.Mutable.CommandFrame Deserialize(CommandFrame obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.CommandFrame();
            accessor.ClientId = ClientIdSerializer.Instance.Deserialize(obj.ClientId);
            accessor.Commands = CommandSerializer.Instance.Deserialize(obj.CommandsLength, obj.Commands);
            return accessor;
        }
    }

    public class FullSyncFrameSerializer : Serializer<Nostradamus.Schema.Mutable.FullSyncFrame, FullSyncFrame>
    {
        public static readonly FullSyncFrameSerializer Instance = new FullSyncFrameSerializer();

        public override Offset<FullSyncFrame> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.FullSyncFrame obj)
        {
            FullSyncFrame.StartFullSyncFrame(fbb);
            FullSyncFrame.AddTime(fbb, obj.Time);
            FullSyncFrame.AddDeltaTime(fbb, obj.DeltaTime);
            FullSyncFrame.AddSnapshot(fbb, SimulatorSnapshotSerializer.Instance.Serialize(fbb, obj.Snapshot));
            return FullSyncFrame.EndFullSyncFrame(fbb);
        }

        protected override FullSyncFrame GetRootAs(ByteBuffer buffer)
        {
            return FullSyncFrame.GetRootAsFullSyncFrame(buffer);
        }

        public override Nostradamus.Schema.Mutable.FullSyncFrame Deserialize(FullSyncFrame obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.FullSyncFrame();
            accessor.Time = obj.Time;
            accessor.DeltaTime = obj.DeltaTime;
            accessor.Snapshot = SimulatorSnapshotSerializer.Instance.Deserialize(obj.Snapshot);
            return accessor;
        }
    }

    public class CommandSeqSerializer : Serializer<Nostradamus.Schema.Mutable.CommandSeq, CommandSeq>
    {
        public static readonly CommandSeqSerializer Instance = new CommandSeqSerializer();

        public override Offset<CommandSeq> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.CommandSeq obj)
        {
            CommandSeq.StartCommandSeq(fbb);
            CommandSeq.AddClientId(fbb, ClientIdSerializer.Instance.Serialize(fbb, obj.ClientId));
            CommandSeq.AddSequence(fbb, obj.Sequence);
            return CommandSeq.EndCommandSeq(fbb);
        }

        protected override CommandSeq GetRootAs(ByteBuffer buffer)
        {
            return CommandSeq.GetRootAsCommandSeq(buffer);
        }

        public override Nostradamus.Schema.Mutable.CommandSeq Deserialize(CommandSeq obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.CommandSeq();
            accessor.ClientId = ClientIdSerializer.Instance.Deserialize(obj.ClientId);
            accessor.Sequence = obj.Sequence;
            return accessor;
        }
    }

    public class DeltaSyncFrameSerializer : Serializer<Nostradamus.Schema.Mutable.DeltaSyncFrame, DeltaSyncFrame>
    {
        public static readonly DeltaSyncFrameSerializer Instance = new DeltaSyncFrameSerializer();

        public override Offset<DeltaSyncFrame> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.DeltaSyncFrame obj)
        {
            DeltaSyncFrame.StartDeltaSyncFrame(fbb);
            DeltaSyncFrame.AddTime(fbb, obj.Time);
            DeltaSyncFrame.AddDeltaTime(fbb, obj.DeltaTime);
            DeltaSyncFrame.AddEvents(fbb, DeltaSyncFrame.CreateEventsVector(fbb, EventSerializer.Instance.Serialize(fbb, obj.Events)));
            DeltaSyncFrame.AddLastCommandSeqs(fbb, DeltaSyncFrame.CreateLastCommandSeqsVector(fbb, CommandSeqSerializer.Instance.Serialize(fbb, obj.LastCommandSeqs)));
            return DeltaSyncFrame.EndDeltaSyncFrame(fbb);
        }

        protected override DeltaSyncFrame GetRootAs(ByteBuffer buffer)
        {
            return DeltaSyncFrame.GetRootAsDeltaSyncFrame(buffer);
        }

        public override Nostradamus.Schema.Mutable.DeltaSyncFrame Deserialize(DeltaSyncFrame obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.DeltaSyncFrame();
            accessor.Time = obj.Time;
            accessor.DeltaTime = obj.DeltaTime;
            accessor.Events = EventSerializer.Instance.Deserialize(obj.EventsLength, obj.Events);
            accessor.LastCommandSeqs = CommandSeqSerializer.Instance.Deserialize(obj.LastCommandSeqsLength, obj.LastCommandSeqs);
            return accessor;
        }
    }

    public class LoginSerializer : Serializer<Nostradamus.Schema.Mutable.Login, Login>
    {
        public static readonly LoginSerializer Instance = new LoginSerializer();

        public override Offset<Login> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.Login obj)
        {
            Login.StartLogin(fbb);
            Login.AddClientId(fbb, ClientIdSerializer.Instance.Serialize(fbb, obj.ClientId));
            return Login.EndLogin(fbb);
        }

        protected override Login GetRootAs(ByteBuffer buffer)
        {
            return Login.GetRootAsLogin(buffer);
        }

        public override Nostradamus.Schema.Mutable.Login Deserialize(Login obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.Login();
            accessor.ClientId = ClientIdSerializer.Instance.Deserialize(obj.ClientId);
            return accessor;
        }
    }

    public class Vector3Serializer : Serializer<Nostradamus.Schema.Mutable.Vector3, Vector3>
    {
        public static readonly Vector3Serializer Instance = new Vector3Serializer();

        public override Offset<Vector3> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.Vector3 obj)
        {
            Vector3.StartVector3(fbb);
            Vector3.AddX(fbb, obj.X);
            Vector3.AddY(fbb, obj.Y);
            Vector3.AddZ(fbb, obj.Z);
            return Vector3.EndVector3(fbb);
        }

        protected override Vector3 GetRootAs(ByteBuffer buffer)
        {
            return Vector3.GetRootAsVector3(buffer);
        }

        public override Nostradamus.Schema.Mutable.Vector3 Deserialize(Vector3 obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.Vector3();
            accessor.X = obj.X;
            accessor.Y = obj.Y;
            accessor.Z = obj.Z;
            return accessor;
        }
    }

    public class QuaternionSerializer : Serializer<Nostradamus.Schema.Mutable.Quaternion, Quaternion>
    {
        public static readonly QuaternionSerializer Instance = new QuaternionSerializer();

        public override Offset<Quaternion> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.Quaternion obj)
        {
            Quaternion.StartQuaternion(fbb);
            Quaternion.AddX(fbb, obj.X);
            Quaternion.AddY(fbb, obj.Y);
            Quaternion.AddZ(fbb, obj.Z);
            Quaternion.AddW(fbb, obj.W);
            return Quaternion.EndQuaternion(fbb);
        }

        protected override Quaternion GetRootAs(ByteBuffer buffer)
        {
            return Quaternion.GetRootAsQuaternion(buffer);
        }

        public override Nostradamus.Schema.Mutable.Quaternion Deserialize(Quaternion obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.Quaternion();
            accessor.X = obj.X;
            accessor.Y = obj.Y;
            accessor.Z = obj.Z;
            accessor.W = obj.W;
            return accessor;
        }
    }

    public class RigidBodyMovedEventSerializer : Serializer<Nostradamus.Schema.Mutable.RigidBodyMovedEvent, RigidBodyMovedEvent>
    {
        public static readonly RigidBodyMovedEventSerializer Instance = new RigidBodyMovedEventSerializer();

        public override Offset<RigidBodyMovedEvent> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.RigidBodyMovedEvent obj)
        {
            RigidBodyMovedEvent.StartRigidBodyMovedEvent(fbb);
            RigidBodyMovedEvent.AddPosition(fbb, Vector3Serializer.Instance.Serialize(fbb, obj.Position));
            RigidBodyMovedEvent.AddRotation(fbb, QuaternionSerializer.Instance.Serialize(fbb, obj.Rotation));
            RigidBodyMovedEvent.AddLinearVelocity(fbb, Vector3Serializer.Instance.Serialize(fbb, obj.LinearVelocity));
            RigidBodyMovedEvent.AddAngularVelocity(fbb, Vector3Serializer.Instance.Serialize(fbb, obj.AngularVelocity));
            return RigidBodyMovedEvent.EndRigidBodyMovedEvent(fbb);
        }

        protected override RigidBodyMovedEvent GetRootAs(ByteBuffer buffer)
        {
            return RigidBodyMovedEvent.GetRootAsRigidBodyMovedEvent(buffer);
        }

        public override Nostradamus.Schema.Mutable.RigidBodyMovedEvent Deserialize(RigidBodyMovedEvent obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.RigidBodyMovedEvent();
            accessor.Position = Vector3Serializer.Instance.Deserialize(obj.Position);
            accessor.Rotation = QuaternionSerializer.Instance.Deserialize(obj.Rotation);
            accessor.LinearVelocity = Vector3Serializer.Instance.Deserialize(obj.LinearVelocity);
            accessor.AngularVelocity = Vector3Serializer.Instance.Deserialize(obj.AngularVelocity);
            return accessor;
        }
    }

    public class RigidBodySnapshotSerializer : Serializer<Nostradamus.Schema.Mutable.RigidBodySnapshot, RigidBodySnapshot>
    {
        public static readonly RigidBodySnapshotSerializer Instance = new RigidBodySnapshotSerializer();

        public override Offset<RigidBodySnapshot> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.RigidBodySnapshot obj)
        {
            RigidBodySnapshot.StartRigidBodySnapshot(fbb);
            RigidBodySnapshot.AddPosition(fbb, Vector3Serializer.Instance.Serialize(fbb, obj.Position));
            RigidBodySnapshot.AddRotation(fbb, QuaternionSerializer.Instance.Serialize(fbb, obj.Rotation));
            RigidBodySnapshot.AddLinearVelocity(fbb, Vector3Serializer.Instance.Serialize(fbb, obj.LinearVelocity));
            RigidBodySnapshot.AddAngularVelocity(fbb, Vector3Serializer.Instance.Serialize(fbb, obj.AngularVelocity));
            return RigidBodySnapshot.EndRigidBodySnapshot(fbb);
        }

        protected override RigidBodySnapshot GetRootAs(ByteBuffer buffer)
        {
            return RigidBodySnapshot.GetRootAsRigidBodySnapshot(buffer);
        }

        public override Nostradamus.Schema.Mutable.RigidBodySnapshot Deserialize(RigidBodySnapshot obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.RigidBodySnapshot();
            accessor.Position = Vector3Serializer.Instance.Deserialize(obj.Position);
            accessor.Rotation = QuaternionSerializer.Instance.Deserialize(obj.Rotation);
            accessor.LinearVelocity = Vector3Serializer.Instance.Deserialize(obj.LinearVelocity);
            accessor.AngularVelocity = Vector3Serializer.Instance.Deserialize(obj.AngularVelocity);
            return accessor;
        }
    }

    public class BallDescSerializer : Serializer<Nostradamus.Schema.Mutable.BallDesc, BallDesc>
    {
        public static readonly BallDescSerializer Instance = new BallDescSerializer();

        public override Offset<BallDesc> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.BallDesc obj)
        {
            BallDesc.StartBallDesc(fbb);
            BallDesc.AddId(fbb, ActorIdSerializer.Instance.Serialize(fbb, obj.Id));
            BallDesc.AddInitialPosition(fbb, Vector3Serializer.Instance.Serialize(fbb, obj.InitialPosition));
            return BallDesc.EndBallDesc(fbb);
        }

        protected override BallDesc GetRootAs(ByteBuffer buffer)
        {
            return BallDesc.GetRootAsBallDesc(buffer);
        }

        public override Nostradamus.Schema.Mutable.BallDesc Deserialize(BallDesc obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.BallDesc();
            accessor.Id = ActorIdSerializer.Instance.Deserialize(obj.Id);
            accessor.InitialPosition = Vector3Serializer.Instance.Deserialize(obj.InitialPosition);
            return accessor;
        }
    }

    public class CubeDescSerializer : Serializer<Nostradamus.Schema.Mutable.CubeDesc, CubeDesc>
    {
        public static readonly CubeDescSerializer Instance = new CubeDescSerializer();

        public override Offset<CubeDesc> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.CubeDesc obj)
        {
            CubeDesc.StartCubeDesc(fbb);
            CubeDesc.AddId(fbb, ActorIdSerializer.Instance.Serialize(fbb, obj.Id));
            CubeDesc.AddInitialPosition(fbb, Vector3Serializer.Instance.Serialize(fbb, obj.InitialPosition));
            return CubeDesc.EndCubeDesc(fbb);
        }

        protected override CubeDesc GetRootAs(ByteBuffer buffer)
        {
            return CubeDesc.GetRootAsCubeDesc(buffer);
        }

        public override Nostradamus.Schema.Mutable.CubeDesc Deserialize(CubeDesc obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.CubeDesc();
            accessor.Id = ActorIdSerializer.Instance.Deserialize(obj.Id);
            accessor.InitialPosition = Vector3Serializer.Instance.Deserialize(obj.InitialPosition);
            return accessor;
        }
    }

    public class KickBallCommandSerializer : Serializer<Nostradamus.Schema.Mutable.KickBallCommand, KickBallCommand>
    {
        public static readonly KickBallCommandSerializer Instance = new KickBallCommandSerializer();

        public override Offset<KickBallCommand> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.KickBallCommand obj)
        {
            KickBallCommand.StartKickBallCommand(fbb);
            KickBallCommand.AddInputX(fbb, obj.InputX);
            KickBallCommand.AddInputY(fbb, obj.InputY);
            KickBallCommand.AddInputZ(fbb, obj.InputZ);
            return KickBallCommand.EndKickBallCommand(fbb);
        }

        protected override KickBallCommand GetRootAs(ByteBuffer buffer)
        {
            return KickBallCommand.GetRootAsKickBallCommand(buffer);
        }

        public override Nostradamus.Schema.Mutable.KickBallCommand Deserialize(KickBallCommand obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.KickBallCommand();
            accessor.InputX = obj.InputX;
            accessor.InputY = obj.InputY;
            accessor.InputZ = obj.InputZ;
            return accessor;
        }
    }

    public class SceneInitializedEventSerializer : Serializer<Nostradamus.Schema.Mutable.SceneInitializedEvent, SceneInitializedEvent>
    {
        public static readonly SceneInitializedEventSerializer Instance = new SceneInitializedEventSerializer();

        public override Offset<SceneInitializedEvent> Serialize(FlatBufferBuilder fbb, Nostradamus.Schema.Mutable.SceneInitializedEvent obj)
        {
            SceneInitializedEvent.StartSceneInitializedEvent(fbb);
            SceneInitializedEvent.AddCubeRows(fbb, obj.CubeRows);
            SceneInitializedEvent.AddCubeColumns(fbb, obj.CubeColumns);
            return SceneInitializedEvent.EndSceneInitializedEvent(fbb);
        }

        protected override SceneInitializedEvent GetRootAs(ByteBuffer buffer)
        {
            return SceneInitializedEvent.GetRootAsSceneInitializedEvent(buffer);
        }

        public override Nostradamus.Schema.Mutable.SceneInitializedEvent Deserialize(SceneInitializedEvent obj)
        {
            var accessor = new Nostradamus.Schema.Mutable.SceneInitializedEvent();
            accessor.CubeRows = obj.CubeRows;
            accessor.CubeColumns = obj.CubeColumns;
            return accessor;
        }
    }

}
