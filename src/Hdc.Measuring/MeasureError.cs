using System;

namespace Hdc.Measuring
{
    public class MeasureError
    {
        public int ErrorCode { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }

        public MeasureEvent MeasureEvent { get; set; }
    }
}