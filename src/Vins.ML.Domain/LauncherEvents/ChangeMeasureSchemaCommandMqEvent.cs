namespace Vins.ML.Domain
{
    public class ChangeMeasureSchemaCommandMqEvent
    {
        public int StationIndex { get; set; }
        public int MeasureSchemaIndex { get; set; }
        public string MeasureSchemaName { get; set; } 
    }
}