using System;

namespace Nostradamus.Client
{
    public sealed class ClientSimulatorStats : Stats
    {
        public DateTime CreateTime;

        public int? LastUnacknowledgedCommandSeq;

        public int? ReceivedSyncFrameTime;

        public int? ReceivedSyncFrameDeltaTime;

        public int? LastAcknowledgedCommandSeq;

        public int Time;

        public int DeltaTime;

        public DateTime SimulateTime;

        public int SimulationInterval;

        public int? CommandAcknowledgeInterval;

        public int? SyncInterval;

        public override string ToCsv()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{7},{8},{9},{10},{11},{12}"
                    , ToDateTimeString(CreateTime), ToUnixTimestamp(CreateTime)
                    , LastUnacknowledgedCommandSeq, ReceivedSyncFrameTime, ReceivedSyncFrameDeltaTime
                    , LastAcknowledgedCommandSeq, Time, DeltaTime
                    , ToDateTimeString(SimulateTime), ToUnixTimestamp(SimulateTime)
                    , SimulationInterval, CommandAcknowledgeInterval, SyncInterval
                );
        }

        internal void AdvanceStats(StatsFrame frame)
        {
            SimulationInterval = (int)(ToUnixTimestamp(SimulateTime) - ToUnixTimestamp(CreateTime));

            if (LastAcknowledgedCommandSeq.HasValue)
            {
                var previousFrame = frame.FindPrevious(f => f.GetStats<ClientSimulatorStats>().LastUnacknowledgedCommandSeq == LastAcknowledgedCommandSeq);
                if (previousFrame != null)
                {
                    var previousStats = previousFrame.GetStats<ClientSimulatorStats>();

                    CommandAcknowledgeInterval = (int)(ToUnixTimestamp(SimulateTime) - ToUnixTimestamp(previousStats.SimulateTime));
                }
            }

            if (ReceivedSyncFrameTime.HasValue)
            {
                var previousFrame = frame.FindPrevious(f => f.GetStats<ClientSimulatorStats>().ReceivedSyncFrameTime.HasValue);
                if (previousFrame != null)
                {
                    var previousStats = previousFrame.GetStats<ClientSimulatorStats>();

                    SyncInterval = (int)(ToUnixTimestamp(SimulateTime) - ToUnixTimestamp(previousStats.SimulateTime));
                }
            }
        }

        private static string ToDateTimeString(DateTime time)
        {
            return time.ToString("HH:mm:ss.ffffff");
        }

        private static long ToUnixTimestamp(DateTime time)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)Math.Round((time - unixEpoch).TotalMilliseconds);
        }
    }
}
