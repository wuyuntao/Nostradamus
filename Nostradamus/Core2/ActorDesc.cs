namespace Nostradamus.Core2
{
    public abstract class ActorDesc
    {
        public ActorId Id;

        protected internal abstract ISnapshotArgs InitSnapshot();
    }
}
