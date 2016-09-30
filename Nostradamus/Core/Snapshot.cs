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

		[ProtoMember(2, DynamicType = true)]

		public object Args { get; set; }

		public Snapshot(ActorId actorId, ISnapshotArgs args)
		{
			ActorId = actorId;
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
