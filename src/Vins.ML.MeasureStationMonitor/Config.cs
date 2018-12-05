using System;
using Hdc.Measuring;

// ReSharper disable InconsistentNaming

namespace Vins.ML.MeasureStationMonitor
{
    [Serializable]
    public class Config
    {
        // Common
        public int StationIndex { get; set; }
        public string StationName { get; set; }
        public int StationPositionOffset { get; set; }

        // EasyNetQ
        public string EasyNetQ_Host { get; set; } = "192.168.100.100";
        public string EasyNetQ_VirtualHost { get; set; } = "vinsml";
        public string EasyNetQ_Username { get; set; } = "hdc";
        public string EasyNetQ_Password { get; set; } = "hdc";
    }

    // ReSharper restore InconsistentNaming
}