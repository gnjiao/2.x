using System;
using System.Collections.ObjectModel;

namespace Hdc.Measuring
{
    [Serializable]
    public class MeasureResult
    {
        public int Index { get; set; }

        public Collection<MeasureOutput> Outputs { get; set; } = new Collection<MeasureOutput>();

        public DateTime? MeasureDateTime { get; set; }

        /// <summary>
        /// Error: can do next measure
        /// </summary>
        public bool HasWarning { get; set; }

        /// <summary>
        /// Fault: must stop next measure
        /// </summary>
        public bool HasError { get; set; }

        public string Message { get; set; }

        public int Tag { get; set; }

        public MeasureEvent MeasureEvent { get; set; }
    }
}