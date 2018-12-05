using System;

namespace Vins.ML.Domain
{
    public class StationWatchdogMqEvent
    {
        public int StationIndex { get; set; }
        public string StationName { get; set; }
        public DateTime DateTime { get; set; }

        public int ConfigIndex { get; set; }
        public string ConfigName { get; set; }
        public string ConfigDisplayName { get; set; }
        public string ConfigComment { get; set; }
    }
}