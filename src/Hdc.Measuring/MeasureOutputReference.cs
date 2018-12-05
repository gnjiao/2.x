using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class MeasureOutputReference
    {
        public int MeasureResultIndex { get; set; }
        public int MeasureOutputIndex { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string Name { get; set; }
        public bool UseMeasureEventXY { get; set; }
        public double Offset { get; set; }
    }
}