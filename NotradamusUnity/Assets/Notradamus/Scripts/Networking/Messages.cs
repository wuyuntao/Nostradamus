using Nostradamus.Server;

namespace Nostradamus.Networking
{
	public class MessageEnvelope
	{
		public object Message;
	}

	public class Login
	{
		public ClientId ClientId { get; set; }
	}
}
