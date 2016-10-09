using BulletSharp.Math;
using FlatBuffers;
using Nostradamus.Client;
using Nostradamus.Examples;
using Nostradamus.Physics;
using Nostradamus.Server;
using System;
using System.Collections.Generic;

namespace Nostradamus.Networking
{
    public class Serializer
    {
        private static Dictionary<string, Serializer> serializers = new Dictionary<string, Serializer>();

        private Func<object, byte[]> serialize;
        private Func<byte[], object> deserialize;

        private Serializer(Func<object, byte[]> serializer, Func<byte[], object> deserializer)
        {
            this.serialize = serializer;
            this.deserialize = deserializer;
        }

        public static void Initialize()
        {
            AddSerializer<MessageEnvelope>(Serialize_MessageEnvelope, Deserialize_MessageEnvelope);
            AddSerializer<CommandFrame>(Serialize_CommandFrame, Deserialize_CommandFrame);
            AddSerializer<FullSyncFrame>(Serialize_FullSyncFrame, Deserialize_FullSyncFrame);
            AddSerializer<DeltaSyncFrame>(Serialize_DeltaSyncFrame, Deserialize_DeltaSyncFrame);
            AddSerializer<RigidBodyMovedEvent>(Serialize_RigidBodyMovedEvent, Deserialize_RigidBodyMovedEvent);
            AddSerializer<RigidBodySnapshot>(Serialize_RigidBodySnapshot, Deserialize_RigidBodySnapshot);

            AddSerializer<Login>(Serialize_Login, Deserialize_Login);

            AddSerializer<MoveBallCommand>(Serialize_MoveBallCommand, Deserialize_MoveBallCommand);
            AddSerializer<MoveCharacterCommand>(Serialize_MoveCharacterCommand, Deserialize_MoveCharacterCommand);
            AddSerializer<CharacterMovedEvent>(Serialize_CharacterMovedEvent, Deserialize_CharacterMovedEvent);
            AddSerializer<CharacterSnapshot>(Serialize_CharacterSnapshot, Deserialize_CharacterSnapshot);
        }

        #region Serialization

        public static void AddSerializer<T>(Func<T, byte[]> serialize, Func<byte[], T> deserialize)
        {
            var key = GetTypeKey(typeof(T));
            var serializer = new Serializer(obj => serialize((T)obj), data => deserialize(data));

            serializers.Add(key, serializer);
        }

        public static Serializer GetSerializer(string key)
        {
            Serializer serializer;
            if (!serializers.TryGetValue(key, out serializer))
                throw new KeyNotFoundException(key.ToString());

            return serializer;
        }

        private static string GetTypeKey(Type type)
        {
            return type.FullName;
        }

        public static byte[] Serialize<T>(T obj)
        {
            var key = GetTypeKey(typeof(T));
            var serializer = GetSerializer(key);

            return serializer.serialize(obj);
        }

        public static T Deserialize<T>(byte[] data)
        {
            var key = GetTypeKey(typeof(T));
            var serializer = GetSerializer(key);

            return (T)serializer.deserialize(data);
        }

        #endregion

        #region MessageEnvelope

        private static byte[] Serialize_MessageEnvelope(MessageEnvelope envelope)
        {
            var fbb = new FlatBufferBuilder(1024);
            var oEnvelope = Serialize_MessageEnvelope(fbb, envelope);
            fbb.Finish(oEnvelope.Value);

            return fbb.SizedByteArray();
        }

        private static Offset<Nostradamus.Schema.MessageEnvelope> Serialize_MessageEnvelope(FlatBufferBuilder fbb, MessageEnvelope envelope)
        {
            var key = GetTypeKey(envelope.Message.GetType());
            var serializer = GetSerializer(key);
            var data = serializer.serialize(envelope.Message);

            var oId = fbb.CreateString(key);
            var voData = Nostradamus.Schema.MessageEnvelope.CreateDataVector(fbb, data);
            var oEnvelope = Nostradamus.Schema.MessageEnvelope.CreateMessageEnvelope(fbb, oId, voData);

            return oEnvelope;
        }

        private static MessageEnvelope Deserialize_MessageEnvelope(byte[] data)
        {
            var buffer = new ByteBuffer(data);
            var envelope = Nostradamus.Schema.MessageEnvelope.GetRootAsMessageEnvelope(buffer);

            return Deserializer_MessageEnvelope(envelope);
        }

        private static MessageEnvelope Deserializer_MessageEnvelope(Nostradamus.Schema.MessageEnvelope envelope)
        {
            var serializer = GetSerializer(envelope.Id);
            var segment = envelope.GetDataBytes().Value;
            var dataArray = new byte[segment.Count];
            Array.Copy(segment.Array, segment.Offset, dataArray, 0, segment.Count);
            var message = serializer.deserialize(dataArray);

            return new MessageEnvelope() { Message = message };
        }

        #endregion

        #region CommandFrame

        private static byte[] Serialize_CommandFrame(CommandFrame frame)
        {
            var fbb = new FlatBufferBuilder(2014);

            var oCommands = new Offset<Nostradamus.Schema.Command>[frame.Commands.Count];
            for (int i = 0; i < frame.Commands.Count; i++)
            {
                var command = frame.Commands[i];

                var oActorIdDesc = string.IsNullOrEmpty(command.ActorId.Description) ? default(StringOffset) : fbb.CreateString(command.ActorId.Description);
                var oActorId = Nostradamus.Schema.ActorId.CreateActorId(fbb, command.ActorId.Value, oActorIdDesc);
                var oEnvelope = Serialize_MessageEnvelope(fbb, new MessageEnvelope() { Message = command.Args });
                var oCommand = Nostradamus.Schema.Command.CreateCommand(fbb, oActorId, command.Sequence, command.Time, command.DeltaTime, oEnvelope);

                oCommands[i] = oCommand;
            }
            var voCommands = Nostradamus.Schema.CommandFrame.CreateCommandsVector(fbb, oCommands);

            var oClientIdDesc = string.IsNullOrEmpty(frame.ClientId.Description) ? default(StringOffset) : fbb.CreateString(frame.ClientId.Description);
            var oClientId = Nostradamus.Schema.ClientId.CreateClientId(fbb, frame.ClientId.Value, oClientIdDesc);

            var oFrame = Nostradamus.Schema.CommandFrame.CreateCommandFrame(fbb, oClientId, voCommands);
            fbb.Finish(oFrame.Value);

            return fbb.SizedByteArray();
        }

        private static CommandFrame Deserialize_CommandFrame(byte[] data)
        {
            var buffer = new ByteBuffer(data);
            var frame = Nostradamus.Schema.CommandFrame.GetRootAsCommandFrame(buffer);

            var clientId = new ClientId(frame.ClientId.Value.Value, frame.ClientId.Value.Description);
            var commandFrame = new CommandFrame(clientId);

            for (int i = 0; i < frame.CommandsLength; i++)
            {
                var command = frame.Commands(i).Value;
                var actorId = new ActorId(command.ActorId.Value.Value, command.ActorId.Value.Description);

                var envelope = Deserializer_MessageEnvelope(command.Args.Value);

                commandFrame.Commands.Add(new Command(actorId, command.Sequence, command.Time, command.DeltaTime, (ICommandArgs)envelope.Message));
            }

            return commandFrame;
        }

        #endregion

        #region FullSyncFrame

        private static byte[] Serialize_FullSyncFrame(FullSyncFrame frame)
        {
            var fbb = new FlatBufferBuilder(2014);

            var oSnapshots = new Offset<Nostradamus.Schema.Snapshot>[frame.Snapshots.Count];
            for (int i = 0; i < frame.Snapshots.Count; i++)
            {
                var snapshot = frame.Snapshots[i];

                var oActorIdDesc = string.IsNullOrEmpty(snapshot.ActorId.Description) ? default(StringOffset) : fbb.CreateString(snapshot.ActorId.Description);
                var oActorId = Nostradamus.Schema.ActorId.CreateActorId(fbb, snapshot.ActorId.Value, oActorIdDesc);
                var oEnvelope = Serialize_MessageEnvelope(fbb, new MessageEnvelope() { Message = snapshot.Args });
                var oSnapshot = Nostradamus.Schema.Snapshot.CreateSnapshot(fbb, oActorId, oEnvelope);

                oSnapshots[i] = oSnapshot;
            }
            var voSnapshots = Nostradamus.Schema.FullSyncFrame.CreateSnapshotsVector(fbb, oSnapshots);

            var oFrame = Nostradamus.Schema.FullSyncFrame.CreateFullSyncFrame(fbb, frame.Time, frame.DeltaTime, voSnapshots);
            fbb.Finish(oFrame.Value);

            return fbb.SizedByteArray();
        }

        private static FullSyncFrame Deserialize_FullSyncFrame(byte[] data)
        {
            var buffer = new ByteBuffer(data);
            var frame = Nostradamus.Schema.FullSyncFrame.GetRootAsFullSyncFrame(buffer);

            var fullSyncFrame = new FullSyncFrame(frame.Time, frame.DeltaTime);

            for (int i = 0; i < frame.SnapshotsLength; i++)
            {
                var command = frame.Snapshots(i).Value;
                var actorId = new ActorId(command.ActorId.Value.Value, command.ActorId.Value.Description);

                var envelope = Deserializer_MessageEnvelope(command.Args.Value);

                fullSyncFrame.Snapshots.Add(new Snapshot(actorId, (ISnapshotArgs)envelope.Message));
            }

            return fullSyncFrame;
        }

        #endregion

        #region DeltaSyncFrame

        private static byte[] Serialize_DeltaSyncFrame(DeltaSyncFrame frame)
        {
            var fbb = new FlatBufferBuilder(2014);

            var oEvents = new Offset<Nostradamus.Schema.Event>[frame.Events.Count];
            for (int i = 0; i < frame.Events.Count; i++)
            {
                var @event = frame.Events[i];

                var oActorIdDesc = string.IsNullOrEmpty(@event.ActorId.Description) ? default(StringOffset) : fbb.CreateString(@event.ActorId.Description);
                var oActorId = Nostradamus.Schema.ActorId.CreateActorId(fbb, @event.ActorId.Value, oActorIdDesc);
                var oEnvelope = Serialize_MessageEnvelope(fbb, new MessageEnvelope() { Message = @event.Args });
                var oEvent = Nostradamus.Schema.Event.CreateEvent(fbb, oActorId, oEnvelope);

                oEvents[i] = oEvent;
            }
            var voEvents = Nostradamus.Schema.DeltaSyncFrame.CreateEventsVector(fbb, oEvents);

            var oLastCommandSeqs = new Offset<Nostradamus.Schema.CommandSeq>[frame.LastCommandSeqs.Count];
            int j = 0;
            foreach (var commandSeq in frame.LastCommandSeqs)
            {
                var oClientIdDesc = string.IsNullOrEmpty(commandSeq.Key.Description) ? default(StringOffset) : fbb.CreateString(commandSeq.Key.Description);
                var oClientId = Nostradamus.Schema.ClientId.CreateClientId(fbb, commandSeq.Key.Value, oClientIdDesc);

                var oCommandSeq = Nostradamus.Schema.CommandSeq.CreateCommandSeq(fbb, oClientId, commandSeq.Value);

                oLastCommandSeqs[j] = oCommandSeq;
                j++;
            }
            var voLastCommandSeqs = Nostradamus.Schema.DeltaSyncFrame.CreateLastCommandSeqsVector(fbb, oLastCommandSeqs);

            var oFrame = Nostradamus.Schema.DeltaSyncFrame.CreateDeltaSyncFrame(fbb, frame.Time, frame.DeltaTime, voEvents, voLastCommandSeqs);
            fbb.Finish(oFrame.Value);

            return fbb.SizedByteArray();
        }

        private static DeltaSyncFrame Deserialize_DeltaSyncFrame(byte[] data)
        {
            var buffer = new ByteBuffer(data);
            var frame = Nostradamus.Schema.DeltaSyncFrame.GetRootAsDeltaSyncFrame(buffer);

            var deltaSyncFrame = new DeltaSyncFrame(frame.Time, frame.DeltaTime);

            for (int i = 0; i < frame.EventsLength; i++)
            {
                var command = frame.Events(i).Value;
                var actorId = new ActorId(command.ActorId.Value.Value, command.ActorId.Value.Description);

                var envelope = Deserializer_MessageEnvelope(command.Args.Value);

                deltaSyncFrame.Events.Add(new Event(actorId, (IEventArgs)envelope.Message));
            }

            for (int j = 0; j < frame.LastCommandSeqsLength; j++)
            {
                var commandSeq = frame.LastCommandSeqs(j).Value;
                var clientId = new ClientId(commandSeq.ClientId.Value.Value, commandSeq.ClientId.Value.Description);

                deltaSyncFrame.LastCommandSeqs.Add(clientId, commandSeq.Sequence);
            }

            return deltaSyncFrame;
        }

        #endregion

        #region RigidBodyMovedEvent

        private static byte[] Serialize_RigidBodyMovedEvent(RigidBodyMovedEvent e)
        {
            var fbb = new FlatBufferBuilder(2014);

            var oPosition = Nostradamus.Schema.Vector3.CreateVector3(fbb, e.Position.X, e.Position.Y, e.Position.Z);
            var oRotation = Nostradamus.Schema.Quaternion.CreateQuaternion(fbb, e.Rotation.X, e.Rotation.Y, e.Rotation.Z, e.Rotation.W);
            var oLinearVelocity = Nostradamus.Schema.Vector3.CreateVector3(fbb, e.LinearVelocity.X, e.LinearVelocity.Y, e.LinearVelocity.Z);
            var oAngularVelocity = Nostradamus.Schema.Vector3.CreateVector3(fbb, e.AngularVelocity.X, e.AngularVelocity.Y, e.AngularVelocity.Z);

            var oEvent = Nostradamus.Schema.RigidBodyMovedEvent.CreateRigidBodyMovedEvent(fbb, oPosition, oRotation, oLinearVelocity, oAngularVelocity);
            fbb.Finish(oEvent.Value);

            return fbb.SizedByteArray();
        }

        private static RigidBodyMovedEvent Deserialize_RigidBodyMovedEvent(byte[] data)
        {
            var buffer = new ByteBuffer(data);
            var e = Nostradamus.Schema.RigidBodyMovedEvent.GetRootAsRigidBodyMovedEvent(buffer);

            return new RigidBodyMovedEvent()
            {
                Position = new Vector3(e.Position.Value.X, e.Position.Value.Y, e.Position.Value.Z),
                Rotation = new Quaternion(e.Rotation.Value.X, e.Rotation.Value.Y, e.Rotation.Value.Z, e.Rotation.Value.W),
                LinearVelocity = new Vector3(e.LinearVelocity.Value.X, e.LinearVelocity.Value.Y, e.LinearVelocity.Value.Z),
                AngularVelocity = new Vector3(e.AngularVelocity.Value.X, e.AngularVelocity.Value.Y, e.AngularVelocity.Value.Z),
            };
        }

        #endregion

        #region RigidBodySnapshot

        private static byte[] Serialize_RigidBodySnapshot(RigidBodySnapshot e)
        {
            var fbb = new FlatBufferBuilder(2014);

            var oPosition = Nostradamus.Schema.Vector3.CreateVector3(fbb, e.Position.X, e.Position.Y, e.Position.Z);
            var oRotation = Nostradamus.Schema.Quaternion.CreateQuaternion(fbb, e.Rotation.X, e.Rotation.Y, e.Rotation.Z, e.Rotation.W);
            var oLinearVelocity = Nostradamus.Schema.Vector3.CreateVector3(fbb, e.LinearVelocity.X, e.LinearVelocity.Y, e.LinearVelocity.Z);
            var oAngularVelocity = Nostradamus.Schema.Vector3.CreateVector3(fbb, e.AngularVelocity.X, e.AngularVelocity.Y, e.AngularVelocity.Z);

            var oSNapshot = Nostradamus.Schema.RigidBodySnapshot.CreateRigidBodySnapshot(fbb, oPosition, oRotation, oLinearVelocity, oAngularVelocity);
            fbb.Finish(oSNapshot.Value);

            return fbb.SizedByteArray();
        }

        private static RigidBodySnapshot Deserialize_RigidBodySnapshot(byte[] data)
        {
            var buffer = new ByteBuffer(data);
            var snapshot = Nostradamus.Schema.RigidBodySnapshot.GetRootAsRigidBodySnapshot(buffer);

            return new RigidBodySnapshot()
            {
                Position = new Vector3(snapshot.Position.Value.X, snapshot.Position.Value.Y, snapshot.Position.Value.Z),
                Rotation = new Quaternion(snapshot.Rotation.Value.X, snapshot.Rotation.Value.Y, snapshot.Rotation.Value.Z, snapshot.Rotation.Value.W),
                LinearVelocity = new Vector3(snapshot.LinearVelocity.Value.X, snapshot.LinearVelocity.Value.Y, snapshot.LinearVelocity.Value.Z),
                AngularVelocity = new Vector3(snapshot.AngularVelocity.Value.X, snapshot.AngularVelocity.Value.Y, snapshot.AngularVelocity.Value.Z),
            };
        }

        #endregion

        #region Login

        private static byte[] Serialize_Login(Login msg)
        {
            var fbb = new FlatBufferBuilder(2014);

            var oClientIdDesc = string.IsNullOrEmpty(msg.ClientId.Description) ? default(StringOffset) : fbb.CreateString(msg.ClientId.Description);
            var oClientId = Nostradamus.Schema.ClientId.CreateClientId(fbb, msg.ClientId.Value, oClientIdDesc);

            var oMsg = Nostradamus.Schema.Login.CreateLogin(fbb, oClientId);
            fbb.Finish(oMsg.Value);

            return fbb.SizedByteArray();
        }

        private static Login Deserialize_Login(byte[] data)
        {
            var buffer = new ByteBuffer(data);
            var msg = Nostradamus.Schema.Login.GetRootAsLogin(buffer);

            return new Login()
            {
                ClientId = new ClientId(msg.ClientId.Value.Value, msg.ClientId.Value.Description)
            };
        }

        #endregion

        #region MoveBallCommand

        private static byte[] Serialize_MoveBallCommand(MoveBallCommand command)
        {
            var fbb = new FlatBufferBuilder(2014);

            var oCommand = Nostradamus.Examples.Schema.MoveBallCommand.CreateMoveBallCommand(fbb, command.InputX, command.InputY, command.InputZ);
            fbb.Finish(oCommand.Value);

            return fbb.SizedByteArray();
        }

        private static MoveBallCommand Deserialize_MoveBallCommand(byte[] data)
        {
            var buffer = new ByteBuffer(data);
            var command = Nostradamus.Examples.Schema.MoveBallCommand.GetRootAsMoveBallCommand(buffer);

            return new MoveBallCommand()
            {
                InputX = command.InputX,
                InputY = command.InputY,
                InputZ = command.InputZ,
            };
        }

        #endregion

        #region MoveCharacterCommand

        private static byte[] Serialize_MoveCharacterCommand(MoveCharacterCommand command)
        {
            var fbb = new FlatBufferBuilder(2014);

            var oCommand = Nostradamus.Examples.Schema.MoveCharacterCommand.CreateMoveCharacterCommand(fbb, command.DeltaX, command.DeltaY);
            fbb.Finish(oCommand.Value);

            return fbb.SizedByteArray();
        }

        private static MoveCharacterCommand Deserialize_MoveCharacterCommand(byte[] data)
        {
            var buffer = new ByteBuffer(data);
            var command = Nostradamus.Examples.Schema.MoveCharacterCommand.GetRootAsMoveCharacterCommand(buffer);

            return new MoveCharacterCommand()
            {
                DeltaX = command.DeltaX,
                DeltaY = command.DeltaY,
            };
        }

        #endregion

        #region CharacterMovedEvent

        private static byte[] Serialize_CharacterMovedEvent(CharacterMovedEvent e)
        {
            var fbb = new FlatBufferBuilder(2014);

            var oEvent = Nostradamus.Examples.Schema.CharacterMovedEvent.CreateCharacterMovedEvent(fbb, e.PositionX, e.PositionY);
            fbb.Finish(oEvent.Value);

            return fbb.SizedByteArray();
        }

        private static CharacterMovedEvent Deserialize_CharacterMovedEvent(byte[] data)
        {
            var buffer = new ByteBuffer(data);
            var e = Nostradamus.Examples.Schema.CharacterMovedEvent.GetRootAsCharacterMovedEvent(buffer);

            return new CharacterMovedEvent()
            {
                PositionX = e.PositionX,
                PositionY = e.PositionY,
            };
        }

        #endregion

        #region CharacterSnapshot

        private static byte[] Serialize_CharacterSnapshot(CharacterSnapshot e)
        {
            var fbb = new FlatBufferBuilder(2014);

            var oSnapshot = Nostradamus.Examples.Schema.CharacterSnapshot.CreateCharacterSnapshot(fbb, e.PositionX, e.PositionY);
            fbb.Finish(oSnapshot.Value);

            return fbb.SizedByteArray();
        }

        private static CharacterSnapshot Deserialize_CharacterSnapshot(byte[] data)
        {
            var buffer = new ByteBuffer(data);
            var snapshot = Nostradamus.Examples.Schema.CharacterSnapshot.GetRootAsCharacterSnapshot(buffer);

            return new CharacterSnapshot()
            {
                PositionX = snapshot.PositionX,
                PositionY = snapshot.PositionY,
            };
        }

        #endregion
    }
}