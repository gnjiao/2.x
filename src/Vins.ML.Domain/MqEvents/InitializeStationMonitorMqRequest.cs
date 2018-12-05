using System;

namespace Vins.ML.Domain
{
    public class InitializeStationMonitorMqRequest
    {
        public int StationIndex { get; set; }
        public string StationName { get; set; }
        public int WorkpieceTag { get; set; }
        public Guid ClientGuid { get; set; }
    }
}