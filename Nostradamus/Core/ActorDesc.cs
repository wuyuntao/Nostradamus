namespace Nostradamus
{
    public abstract class ActorDesc
    {
        public ActorId Id;

        protected internal abstract ISnapshotArgs InitSnapshot();
    }
}
