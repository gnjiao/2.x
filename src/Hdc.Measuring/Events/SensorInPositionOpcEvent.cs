using System;

namespace Hdc.Measuring
{
    public class SensorInPositionOpcEvent
    {
        public DateTime? DateTime { get; set; }
        public int StationIndex { get; set; }
        public int PositionIndex { get; set; }
        public int PointIndex { get; set; }
        public string DeviceName { get; set; }

        public string Message { get; set; }

        public int WorkpieceTag { get; set; }
    }
}
