using FlatBuffers;

namespace Nostradamus.Networking
{
    public class Login
    {
        public readonly ClientId ClientId;

        public Login(ClientId clientId)
        {
            ClientId = clientId;
        }
    }

    class LoginSerializer : Serializer<Login, Schema.Login>
    {
        public static readonly LoginSerializer Instance = new LoginSerializer();

        public override Offset<Schema.Login> Serialize(FlatBufferBuilder fbb, Login login)
        {
            var oClientId = ClientIdSerializer.Instance.Serialize(fbb, login.ClientId);

            return Schema.Login.CreateLogin(fbb, oClientId);
        }

        public override Login Deserialize(Schema.Login login)
        {
            var clientId = ClientIdSerializer.Instance.Deserialize(login.ClientId);

            return new Login(clientId);
        }

        public override Schema.Login ToFlatBufferObject(ByteBuffer buffer)
        {
            return Schema.Login.GetRootAsLogin(buffer);
        }
    }
}
