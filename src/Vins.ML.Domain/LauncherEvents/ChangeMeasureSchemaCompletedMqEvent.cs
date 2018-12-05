namespace Vins.ML.Domain
{
    public class ChangeMeasureSchemaCompletedMqEvent
    {
        public int StationIndex { get; set; }
        public int MeasureSchemaIndex { get; set; }
        public string MeasureSchemaName { get; set; }
        public MeasureSchemaInfo MeasureSchemaInfo { get; set; }
    }
}