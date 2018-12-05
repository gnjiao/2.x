using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hdc.Measuring;

namespace Vins.ML.Domain
{
    public class InitializeStationMonitorMqResponse
    {
        public WorkpieceResult WorkpieceResult { get; set; }

        public IList<StationResult> StationMeasureResults { get; set; }  = new List<StationResult>();
        public Guid ClientGuid { get; set; }
    }
}