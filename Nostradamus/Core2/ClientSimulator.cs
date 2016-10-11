using Nostradamus.Client;
using Nostradamus.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nostradamus.Core2
{
    public sealed class ClientSimulator : Simulator
    {
        private ClientId clientId;
        private CommandFrame commandFrame;
        private Timeline authoritativeTimeline = new Timeline();
        private int? lastAcknowledgedCommandSeq;
        private Timeline predictiveTimeline;
        private Queue<Command> unacknowledgedCommands = new Queue<Command>();
        private int time;

        public ClientSimulator(PhysicsSceneDesc desc, ClientId clientId)
            : base(desc)
        {
            this.clientId = clientId;
        }

        public void ReceiveCommand(Command command)
        {
            if (commandFrame == null)
                commandFrame = new CommandFrame(clientId);
            commandFrame.Commands.Add(command);

            unacknowledgedCommands.Enqueue(command);

            if (predictiveTimeline != null)
                predictiveTimeline = new Timeline();
        }

        public void ReceiveFullSyncFrame(FullSyncFrame frame)
        {
            var snapshot = new SimulatorSnapshot() { Actors = frame.Snapshots };

            authoritativeTimeline.AddPoint(frame.Time + frame.DeltaTime, snapshot);
        }

        public void ReceiveDeltaFullFrame(DeltaSyncFrame frame)
        {
            var lastTimepoint = authoritativeTimeline.Last;

            if (lastTimepoint.Time != frame.Time)
                throw new InvalidOperationException();       //TODO: Message

            RecoverSnapshot((SimulatorSnapshot)lastTimepoint.Snapshot);

            ApplyEvents(frame.Events);

            var snapshot = CreateSnapshot();

            authoritativeTimeline.AddPoint(frame.Time + frame.DeltaTime, snapshot);

            lastAcknowledgedCommandSeq = null;
            int lastCommandSeq;
            if (frame.LastCommandSeqs.TryGetValue(clientId, out lastCommandSeq))
                lastAcknowledgedCommandSeq = lastCommandSeq;
        }

        public void Simulate()
        {
            if (authoritativeTimeline.Last == null)     // Not initialized yet
                return;

            int lastAcknowledgedCommandTime = -1;       // Dequeue acknowledge commaands and get last time that command is predicted
            if (lastAcknowledgedCommandSeq != null)
            {
                while (unacknowledgedCommands.Count > 0 && unacknowledgedCommands.Peek().Sequence <= lastAcknowledgedCommandSeq)
                {
                    var command = unacknowledgedCommands.Dequeue();

                    if (command.Sequence == lastAcknowledgedCommandSeq)
                        lastAcknowledgedCommandTime = command.Time + command.DeltaTime;
                }

                throw new InvalidOperationException(string.Format("Cannot find acknowledged command #{0}", lastAcknowledgedCommandSeq));
            }

            if (lastAcknowledgedCommandTime >= 0)
            {
                var authoritativeSnapshot = authoritativeTimeline.Last.Snapshot;

                var predictiveTimepoint = predictiveTimeline.InterpolatePoint(lastAcknowledgedCommandTime);
                if (predictiveTimepoint == null)
                    throw new InvalidOperationException();          // TODO: Message

                var predictiveSnapshot = predictiveTimepoint.Snapshot;
                if (authoritativeSnapshot.IsApproximate(predictiveSnapshot))        // Prediction is almost correct
                    return;

                predictiveTimeline = new Timeline();
                predictiveTimeline.AddPoint(lastAcknowledgedCommandTime, authoritativeSnapshot);

                RecoverSnapshot((SimulatorSnapshot)authoritativeSnapshot);

                var deltaTime = Scene.Desc.ReconciliationDeltaTime;
                for (var replayTime = lastAcknowledgedCommandTime; replayTime < time; replayTime += deltaTime)
                {
                    if (replayTime + deltaTime > time)
                        deltaTime = time - replayTime;

                    Simulate(replayTime, deltaTime);

                    predictiveTimeline.AddPoint(replayTime, CreateSnapshot());
                }
            }

            Simulate(time, Scene.Desc.SimulationDeltaTime);

            var snapshot = CreateSnapshot();
            predictiveTimeline.AddPoint(time + Scene.Desc.SimulationDeltaTime, snapshot);

            time += Scene.Desc.SimulationDeltaTime;
        }

        private void Simulate(int time, int deltaTime)
        {
            Simulate(from command in unacknowledgedCommands
                     where command.Time > time && command.Time <= time + deltaTime
                     select command);
        }

        public SimulatorSnapshot ReceiveDeltaSync(int time, IEnumerable<Event> events)
        {
            var snapshot = (SimulatorSnapshot)authoritativeTimeline.Last.Snapshot;

            RecoverSnapshot(snapshot);

            ApplyEvents(events);

            snapshot = CreateSnapshot();

            authoritativeTimeline.AddPoint(time, snapshot);

            return snapshot;
        }

        public void ReceiveCommands(int time, IEnumerable<Command> commands)
        {
            if (predictiveTimeline == null)
            {
                predictiveTimeline = new Timeline();
                predictiveTimeline.AddPoint(time, authoritativeTimeline.InterpolatePoint(time).Snapshot);
            }

            var snapshot = (SimulatorSnapshot)predictiveTimeline.Last.Snapshot;

            RecoverSnapshot(snapshot);

            Simulate(commands);

            predictiveTimeline.AddPoint(time, CreateSnapshot());
        }

        public CommandFrame CommandFrame
        {
            get { return commandFrame; }
        }
    }
}
