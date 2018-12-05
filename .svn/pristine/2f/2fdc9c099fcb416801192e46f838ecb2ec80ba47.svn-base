using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class MeasureOutput
    {
        public int Index { get; set; }
        public MeasureValidity Validity { get; set; }
        public MeasureOutputJudge Judge { get; set; }
        public double Value { get; set; }

        /// <summary>
        /// Notify message, DataCode result, Alarm...
        /// </summary>
        public string Message { get; set; }
        public long Tag { get; set; }
        public bool IsTuned { get; set; }
        public string Name { get; set; }
        public MeasureEvent MeasureEvent { get; set; }
    }
}