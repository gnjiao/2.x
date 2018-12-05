using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using Vins.ML.Domain;

// ReSharper disable InconsistentNaming

namespace Vins.ML.MeasureStationLanucher
{
    [Serializable]
    [ContentProperty(nameof(MeasureSchemaInfos))]
    public class Config
    {
        // EasyNetQ
        public string EasyNetQ_Host { get; set; } = "192.168.100.100";
        public string EasyNetQ_VirtualHost { get; set; } = "vinsml";
        public string EasyNetQ_Username { get; set; } = "hdc";
        public string EasyNetQ_Password { get; set; } = "hdc";

        // Station
        public string StationName { get; set; }
        public int StationIndex { get; set; }

        // ProcessStart
        public string ProcessStart_FileName { get; set; }
        public string ProcessStart_Arguments { get; set; } = null;
        public bool ProcessStart_CreateNoWindow { get; set; } = false;
        public bool ProcessStart_UseShellExecute { get; set; } = true;

        /// <summary>
        /// In miliseconds
        /// </summary>
        public int WatchDogInterval { get; set; } = 2000;

        // TopShelf
        public string TopShelf_ServiceName { get; set; }

        public string ProcessToKill1 { get; set; }
        public string ProcessToKill2 { get; set; }
        public string ProcessToKill3 { get; set; }

        //
        public Collection<MeasureSchemaInfo> MeasureSchemaInfos { get; set; } = new Collection<MeasureSchemaInfo>();

        public Collection<ProcessStartConfig> ProcessStartInfos { get; set; } = new Collection<ProcessStartConfig>();
    }

    // ReSharper restore InconsistentNaming
}