using Nostradamus.Client;
using System.Collections.Generic;

namespace Nostradamus.Server
{
    public sealed class ServerSimulator : Simulator
    {
        private List<Command> commands = new List<Command>();
        private int time;
        private FullSyncFrame fullSyncFrame;
        private DeltaSyncFrame deltaSyncFrame;
        private Timeline compensationTimeline = new Timeline();

        public ServerSimulator()
        { }

        public void ReceiveCommandFrame(CommandFrame frame)
        {
            foreach (var command in frame.Commands)
                commands.Add(command);
        }

        public void Simulate()
        {
            var deltaTime = Scene.Desc.SimulationDeltaTime;

            fullSyncFrame = new FullSyncFrame(time, deltaTime);
            deltaSyncFrame = new DeltaSyncFrame(time, deltaTime);

            EventApplied += EnqueueEvent;

            Simulate(DequeueCommandsBefore(time + deltaTime));

            EventApplied -= EnqueueEvent;

            time += deltaTime;

            var snapshot = CreateSnapshot();

            fullSyncFrame.Snapshot = snapshot;

            compensationTimeline.AddPoint(time, snapshot);
        }

        private List<Command> DequeueCommandsBefore(int time)
        {
            var dequeuedCommands = new List<Command>();

            commands.RemoveAll(command =>
            {
                if (command.Time <= time)
                {
                    deltaSyncFrame.LastCommandSeqs[command.ClientId] = command.Sequence;

                    dequeuedCommands.Add(command);
                    return true;
                }
                else
                    return false;
            });

            return dequeuedCommands;
        }

        private void EnqueueEvent(Actor actor, IEventArgs @event)
        {
            deltaSyncFrame.Events.Add(new Event(actor.Desc.Id, @event));
        }

        public int Time
        {
            get { return time; }
        }

        public FullSyncFrame FullSyncFrame
        {
            get { return fullSyncFrame; }
        }

        public DeltaSyncFrame DeltaSyncFrame
        {
            get { return deltaSyncFrame; }
        }
    }
}
