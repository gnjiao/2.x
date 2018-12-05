using System;

namespace Vins.ML.Domain
{
    public class LauncherWatchdogMqEvent
    {
        public int StationIndex { get; set; }
        public string StationName { get; set; }
        public DateTime DateTime { get; set; }

        public string ProcessFileName { get; set; }
    }
}