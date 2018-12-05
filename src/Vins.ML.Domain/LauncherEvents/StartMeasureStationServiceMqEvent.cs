namespace Vins.ML.Domain
{
    public class StartMeasureStationServiceMqEvent
    {
        public int StationIndex { get; set; }
        public string StationName { get; set; }
        public int ConfigIndex { get; set; }
    }
}