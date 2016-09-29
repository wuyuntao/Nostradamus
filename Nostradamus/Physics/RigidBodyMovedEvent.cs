using BulletSharp.Math;
using ProtoBuf;

namespace Nostradamus.Physics
{
	[ProtoContract]
	public class RigidBodyMovedEvent : IEventArgs
	{
		[ProtoMember(1)]
		public Vector3 Position { get; set; }

		[ProtoMember(2)]
		public Quaternion Rotation { get; set; }

		[ProtoMember(3)]
		public Vector3 LinearVelocity { get; set; }

		[ProtoMember(4)]
		public Vector3 AngularVelocity { get; set; }
	}
}
