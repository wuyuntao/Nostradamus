using System;
using System.Collections.Generic;

namespace Nostradamus.Client
{
    public sealed class ClientStatsStream
    {
        public readonly List<ClientStatsFrame> Frames = new List<ClientStatsFrame>();

        public void AddFrame()
        {
            Frames.Add(new ClientStatsFrame() { CreateTime = DateTime.UtcNow });
        }

        public ClientStatsFrame LastFrame
        {
            get { return Frames[Frames.Count - 1]; }
        }
    }

    public sealed class ClientStatsFrame
    {
        public DateTime CreateTime;

        public int? LastUnacknowledgedCommandSeq;

        public int? ReceivedSyncFrameTime;

        public int? ReceivedSyncFrameDeltaTime;

        public int? LastAcknowledgedCommandSeq;

        public int Time;

        public int DeltaTime;

        public DateTime SimulateTime;

        public string ToCsvRow()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{7},{8},{9}"
                    , ToDateTimeString(CreateTime), ToUnixTimestamp(CreateTime)
                    , LastUnacknowledgedCommandSeq, ReceivedSyncFrameTime, ReceivedSyncFrameDeltaTime
                    , LastAcknowledgedCommandSeq, Time, DeltaTime
                    , ToDateTimeString(SimulateTime), ToUnixTimestamp(SimulateTime));
        }

        static string ToDateTimeString(DateTime time)
        {
            return time.ToString("HH:mm:ss.ffffff");
        }

        static long ToUnixTimestamp(DateTime time)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)Math.Round((time - unixEpoch).TotalMilliseconds);
        }
    }
}
