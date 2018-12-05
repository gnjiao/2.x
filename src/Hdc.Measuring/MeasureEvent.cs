using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class MeasureEvent
    {
        public string Message { get; set; }
        public DateTime? DateTime { get; set; }
        public int StationIndex { get; set; }
        public int WorkpieceTag { get; set; }
        public int PositionIndex { get; set; }
        public int PointIndex { get; set; }
        public int EventNumber { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double U { get; set; }
//        public string DeviceName { get; set; }
    }
}