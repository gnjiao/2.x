using System;

// ReSharper disable InconsistentNaming

namespace Vins.ML.MeasureLineDataService
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
        public string DbFileName { get; set; } = "Vins.ML.MeasureData.db";
        public string RepositoryType { get; set; } = "Siaqodb";
        public int MaxStationResultsCount { get; set; } = 20000;

        public string XamlExportDirectoryPath { get; set; }
        public int XamlExportMaxDays { get; set; } = 0;
    }

    // ReSharper restore InconsistentNaming
}