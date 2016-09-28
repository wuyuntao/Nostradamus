using Nostradamus.Physics;

namespace Nostradamus.Tests.Events
{
	class RigidBodyMovedEvent : IEventArgs
	{
		public RigidBodySnapshot NewSnapshot;
	}
}
