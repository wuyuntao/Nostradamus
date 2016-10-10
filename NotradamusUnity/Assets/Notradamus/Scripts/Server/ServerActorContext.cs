namespace Nostradamus.Server
{
    class ServerActorContext : ActorContext
    {
        public ServerActorContext(Actor actor, ISnapshotArgs snapshot)
            : base(actor, snapshot)
        { }

        public void CreateTimepoint()
        {
            Timeline.AddPoint(Actor.Scene.Time + Actor.Scene.DeltaTime, Actor.Snapshot);
        }
    }
}