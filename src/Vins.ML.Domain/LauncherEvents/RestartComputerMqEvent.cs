namespace Vins.ML.Domain
{
    public class RestartComputerMqEvent
    {
        public int StationIndex { get; set; }
        public string StationName { get; set; }
    }
}