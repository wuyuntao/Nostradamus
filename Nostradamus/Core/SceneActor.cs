namespace Nostradamus
{
    public abstract class SceneActor : Actor
    {
        public Simulator Simulator
        {
            get { return (Simulator)Context.ActorManager; }
        }

        public Scene Scene
        {
            get { return Simulator.Scene; }
        }
    }
}
