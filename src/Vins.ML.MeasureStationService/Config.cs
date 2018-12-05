using System;
using Hdc.Measuring;

// ReSharper disable InconsistentNaming

namespace Vins.ML.MeasureStationService
{
    [Serializable]
    public class Config
    {
        // EasyNetQ
        public string EasyNetQ_Host { get; set; } = "192.168.100.100";
        public string EasyNetQ_VirtualHost { get; set; } = "vinsml";
        public string EasyNetQ_Username { get; set; } = "hdc";
        public string EasyNetQ_Password { get; set; } = "hdc";

        //
        public int StationPositionOffset { get; set; }
    }

    // ReSharper restore InconsistentNaming
}