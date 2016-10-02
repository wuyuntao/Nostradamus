using ProtoBuf;
using System.Collections.Generic;

namespace Nostradamus.Server
{
	[ProtoContract]
	public sealed class FullSyncFrame
	{
		[ProtoMember(1)]
		public int Time { get; set; }

		[ProtoMember(2)]
		public int DeltaTime { get; set; }

		[ProtoMember(3, IsRequired = false)]
		public List<Snapshot> Snapshots { get; set; }

		public FullSyncFrame(int time, int deltaTime)
		{
			Time = time;
			DeltaTime = deltaTime;
		}

		public FullSyncFrame() { }
	}
}