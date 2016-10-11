using Nostradamus.Client;
using Nostradamus.Server;
using System.Collections.Generic;

namespace Nostradamus.Core2
{
    public sealed class ServerSimulator : Simulator
    {
        private List<Command> commands = new List<Command>();
        private int time;
        private FullSyncFrame fullSyncFrame;
        private DeltaSyncFrame deltaSyncFrame;
        private Timeline compensationTimeline = new Timeline();

        public ServerSimulator(PhysicsSceneDesc desc)
            : base(desc)
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

            Simulate(DequeueCommandsBefore(time + deltaTime));

            time += deltaTime;

            var snapshot = CreateSnapshot();

            fullSyncFrame.Snapshots = snapshot.Actors;

            compensationTimeline.AddPoint(time, snapshot);
        }

        private List<Command> DequeueCommandsBefore(int time)
        {
            var dequeuedCommands = new List<Command>();

            commands.RemoveAll(command =>
            {
                if (command.Time <= time)
                {
                    dequeuedCommands.Add(command);
                    return true;
                }
                else
                    return false;
            });

            return dequeuedCommands;
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
