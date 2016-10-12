using NLog;
using Nostradamus.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nostradamus.Client
{
    public sealed class ClientSimulator : Simulator
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private ClientId clientId;
        private CommandFrame commandFrame;
        private CommandFrame nextCommandFrame;
        private Timeline authoritativeTimeline = new Timeline();
        private int lastCommandSeq;
        private int? lastAcknowledgedCommandSeq;
        private Timeline predictiveTimeline;
        private int? nextConvergenceTime;
        private Queue<Command> unacknowledgedCommands = new Queue<Command>();
        private int time;

        public ClientSimulator(ClientId clientId)
        {
            this.clientId = clientId;
        }

        public void ReceiveCommand(Actor actor, ICommandArgs commandArgs)
        {
            var command = new Command(clientId, actor.Desc.Id, ++lastCommandSeq, time, Scene.Desc.SimulationDeltaTime, commandArgs);

            logger.Debug("ReceiveCommand: {0}", command);

            if (nextCommandFrame == null)
                nextCommandFrame = new CommandFrame(clientId);
            nextCommandFrame.Commands.Add(command);

            unacknowledgedCommands.Enqueue(command);
        }

        public void ReceiveFullSyncFrame(FullSyncFrame frame)
        {
            logger.Debug("ReceiveFullSyncFrame: {0}", frame);

            authoritativeTimeline.AddPoint(frame.Time + frame.DeltaTime, frame.Snapshot);
        }

        public void ReceiveDeltaSyncFrame(DeltaSyncFrame frame)
        {
            logger.Debug("ReceiveDeltaSyncFrame: {0}", frame);

            var currentSnapshot = CreateSnapshot();

            var lastTimepoint = authoritativeTimeline.Last;

            if (lastTimepoint.Time != frame.Time)
                throw new InvalidOperationException();       //TODO: Message

            RecoverSnapshot((SimulatorSnapshot)lastTimepoint.Snapshot);

            ApplyEvents(frame.Events);

            var snapshot = CreateSnapshot();

            authoritativeTimeline.AddPoint(frame.Time + frame.DeltaTime, snapshot);

            int lastCommandSeq;
            if (frame.LastCommandSeqs.TryGetValue(clientId, out lastCommandSeq))
                lastAcknowledgedCommandSeq = lastCommandSeq;

            RecoverSnapshot(currentSnapshot);
        }

        public void Simulate()
        {
            if (authoritativeTimeline.Last == null)     // Not initialized yet
                return;

            logger.Debug("Simulate: {0}: {1}, {2}: {3}"
                    , nameof(time), time
                    , nameof(nextCommandFrame), nextCommandFrame != null ? nextCommandFrame.Commands.Count : 0);

            int lastAcknowledgedCommandTime = -1;       // Dequeue acknowledge commands and get last time that command is predicted
            if (lastAcknowledgedCommandSeq != null)
            {
                lastAcknowledgedCommandTime = DequeueAcknowledgedCommands();
            }

            if (lastAcknowledgedCommandTime >= 0)
            {
                var authoritativeSnapshot = authoritativeTimeline.Last.Snapshot;

                var predictiveTimepoint = predictiveTimeline.InterpolatePoint(lastAcknowledgedCommandTime);
                if (predictiveTimepoint == null)
                    throw new InvalidOperationException();          // TODO: Message

                var currentSnapshot = predictiveTimeline.Last.Snapshot;

                if (!authoritativeSnapshot.IsApproximate(predictiveTimepoint.Snapshot))        // Rollback and replay
                {
                    predictiveTimeline = new Timeline();
                    predictiveTimeline.AddPoint(lastAcknowledgedCommandTime, authoritativeSnapshot);

                    RecoverSnapshot((SimulatorSnapshot)authoritativeSnapshot);

                    // TODO: Use SceneDesc.ReconciliationDeltaTime to optimize replay performance
                    var deltaTime = Scene.Desc.SimulationDeltaTime;
                    for (var replayTime = lastAcknowledgedCommandTime; replayTime < time; replayTime += deltaTime)
                    {
                        if (replayTime + deltaTime > time)
                            deltaTime = time - replayTime;

                        Simulate(from command in unacknowledgedCommands
                                 where command.Time >= replayTime && command.Time < replayTime + deltaTime
                                 select command);

                        predictiveTimeline.AddPoint(replayTime + deltaTime, CreateSnapshot());
                    }
                }

                // TODO: Get jitter detail for analysis
                if (!currentSnapshot.IsApproximate(predictiveTimeline.Last.Snapshot))
                    logger.Warn("Correction done with possible jitter");
            }

            if (nextCommandFrame != null || predictiveTimeline != null)
            {
                SimulatorSnapshot snapshot;
                if (predictiveTimeline != null)
                {
                    snapshot = (SimulatorSnapshot)predictiveTimeline.InterpolatePoint(time).Snapshot;

                    logger.Debug("RecoverSnapshot from predictive timeline");
                }
                else
                {
                    snapshot = (SimulatorSnapshot)authoritativeTimeline.InterpolatePoint(time).Snapshot;

                    predictiveTimeline = new Timeline();
                    predictiveTimeline.AddPoint(time, snapshot);

                    logger.Debug("RecoverSnapshot from authoritative timeline");
                }

                RecoverSnapshot(snapshot);

                Simulate(nextCommandFrame != null ? nextCommandFrame.Commands : null);

                snapshot = CreateSnapshot();

                if (unacknowledgedCommands.Count > 0)
                {
                    predictiveTimeline.AddPoint(time + Scene.Desc.SimulationDeltaTime, snapshot);
                    nextConvergenceTime = time + Scene.Desc.ConvergenceTime;

                    logger.Debug("Simulate and CreateSnapshot for predictive timeline");
                }
                else if (time < nextConvergenceTime)
                {
                    var authoritativeSnapshot = authoritativeTimeline.InterpolatePoint(time + Scene.Desc.SimulationDeltaTime).Snapshot;

                    snapshot = (SimulatorSnapshot)((ISnapshotArgs)snapshot).Interpolate(authoritativeSnapshot, Scene.Desc.ConvergenceRate);

                    predictiveTimeline.AddPoint(time + Scene.Desc.SimulationDeltaTime, snapshot);

                    logger.Debug("All commands are acknowledged and convergence predictive timeline");
                }
                else
                {
                    predictiveTimeline = null;

                    snapshot = (SimulatorSnapshot)authoritativeTimeline.InterpolatePoint(time + Scene.Desc.SimulationDeltaTime).Snapshot;

                    logger.Debug("RecoverSnapshot from authoritative timeline and remove predictive timeline");
                }

                RecoverSnapshot(snapshot);
            }
            else
            {
                var snapshot = (SimulatorSnapshot)authoritativeTimeline.InterpolatePoint(time + Scene.Desc.SimulationDeltaTime).Snapshot;

                logger.Debug("RecoverSnapshot from authoritative timeline");

                RecoverSnapshot(snapshot);
            }

            time += Scene.Desc.SimulationDeltaTime;

            commandFrame = nextCommandFrame;
            nextCommandFrame = null;

            lastAcknowledgedCommandSeq = null;
        }

        private int DequeueAcknowledgedCommands()
        {
            while (unacknowledgedCommands.Count > 0 && unacknowledgedCommands.Peek().Sequence <= lastAcknowledgedCommandSeq)
            {
                var command = unacknowledgedCommands.Dequeue();

                if (command.Sequence == lastAcknowledgedCommandSeq)
                {
                    logger.Debug("DequeueAcknowledgedCommands: {0}", command);
                    return command.Time + command.DeltaTime;
                }
            }

            throw new InvalidOperationException(string.Format("Cannot find acknowledged command #{0}", lastAcknowledgedCommandSeq));
        }

        public ClientId ClientId
        {
            get { return clientId; }
        }

        public CommandFrame CommandFrame
        {
            get { return commandFrame; }
        }

        public int Time
        {
            get { return time; }
        }
    }
}
