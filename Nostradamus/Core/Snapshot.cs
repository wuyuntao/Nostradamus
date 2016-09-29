using ProtoBuf;

namespace Nostradamus
{
	public interface ISnapshotArgs
	{
		ISnapshotArgs Clone();

		ISnapshotArgs Interpolate(ISnapshotArgs snapshot, float factor);

		ISnapshotArgs Extrapolate(int deltaTime);

		bool IsApproximate(ISnapshotArgs snapshot);
	}

	[ProtoContract]
	public sealed class Snapshot
	{
		[ProtoMember(1)]
		public ActorId ActorId { get; set; }

		[ProtoMember(2)]
		public int Time { get; set; }

		[ProtoMember(3)]

		public object Args { get; set; }

		public Snapshot(ActorId actorId, int time, ISnapshotArgs args)
		{
			ActorId = actorId;
			Time = time;
			Args = args;
		}

		public Snapshot() { }

		public ISnapshotArgs GetArgs()
		{
			return (ISnapshotArgs)Args;
		}

		public TArgs GetArgs<TArgs>()
			where TArgs : ISnapshotArgs
		{
			return (TArgs)Args;
		}
	}
}
