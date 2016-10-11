using System;
using System.Collections.Generic;
using System.Linq;

namespace Nostradamus.Core2
{
    public abstract class Simulator
    {
        public Scene Scene { get; private set; }

        protected Simulator(Scene scene)
        {
            Scene = scene;
        }

        public void Update(IEnumerable<Command> commands)
        {
            foreach (var c in commands)
            {
                var actor = Scene.GetActor(c.ActorId);

                actor.ReceiveCommand(c.Args);
            }

            foreach (var a in Scene.Actors)
            {
                a.Update();
            }
        }

        public void Update(IEnumerable<Event> events)
        {
            foreach (var e in events)
            {
                var actor = Scene.GetActor(e.ActorId);

                actor.ApplyEvent(e.Args);
            }
        }

        public SimulatorSnapshot CreateSnapshot()
        {
            return new SimulatorSnapshot()
            {
                Actors = new List<Snapshot>(from a in Scene.Actors
                                            select new Snapshot(a.Desc.Id, a.Snapshot))
            };
        }

        public void RecoverSnapshot(SimulatorSnapshot snapshot)
        {
            foreach (var s in snapshot.Actors)
            {
                //var actor = RecoverActorFromSnapshot(s);
                throw new NotImplementedException();
            }
        }
    }

    public class ClientSimulator : Simulator
    {
        Timeline authoritativeTimeline = new Timeline();
        Timeline predictiveTimeline;

        public ClientSimulator(Scene scene)
            : base(scene)
        { }

        public void Update()
        {
            var lastFrameTime = 10;
            ReceiveDeltaSync(lastFrameTime, null);
            var authoritativeTimepoint = authoritativeTimeline.InterpolatePoint(lastFrameTime);

            var lastCommandTime = 0;

            var predictiveTimepoint = predictiveTimeline.InterpolatePoint(lastCommandTime);
            if (predictiveTimepoint == null)
                return;

            if (authoritativeTimepoint.Snapshot.IsApproximate(predictiveTimepoint.Snapshot))
                return;

            predictiveTimeline = new Timeline();
            predictiveTimeline.AddPoint(lastCommandTime, authoritativeTimepoint.Snapshot);

            RecoverSnapshot((SimulatorSnapshot)authoritativeTimepoint.Snapshot);

            var clientTime = 1000;
            var deltaTime = 50;
            for (var time = lastCommandTime; time < clientTime; time += deltaTime)
            {
                var commands = new List<Command>();

                Update(commands);

                predictiveTimeline.AddPoint(time, CreateSnapshot());
            }

            var newCommands = new List<Command>();
            Update(newCommands);

            var snapshot = CreateSnapshot();
            predictiveTimeline.AddPoint(clientTime + deltaTime, snapshot);

            // TODO 差值
        }

        public SimulatorSnapshot ReceiveFullSync(int time, SimulatorSnapshot snapshot)
        {
            authoritativeTimeline.AddPoint(time, snapshot);

            return snapshot;
        }

        public SimulatorSnapshot ReceiveDeltaSync(int time, IEnumerable<Event> events)
        {
            var snapshot = (SimulatorSnapshot)authoritativeTimeline.Last.Snapshot;

            RecoverSnapshot(snapshot);

            Update(events);

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

            Update(commands);

            predictiveTimeline.AddPoint(time, CreateSnapshot());
        }
    }
}
