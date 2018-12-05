using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class PointPosition
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int MeasureResultIndex { get; set; }
        public int MeasureOutputIndex { get; set; }
    }
}