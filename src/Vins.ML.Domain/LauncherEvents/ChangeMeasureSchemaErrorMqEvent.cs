using System;

namespace Vins.ML.Domain
{
    public class ChangeMeasureSchemaErrorMqEvent
    {
        public int StationIndex { get; set; }
        public int MeasureSchemaIndex { get; set; }
        public string MeasureSchemaName { get; set; }
        public MeasureSchemaInfo MeasureSchemaInfo { get; set; }
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }
    }
}