using System;
using System.Collections.Generic;
using System.Linq;

namespace Hdc.Measuring
{
    [Serializable]
    public class WorkpieceResult
    {
        public long Id { get; set; }
        public int Tag { get; set; }
        public IList<StationResult> StationResults { get; set; } = new List<StationResult>();
        public MeasureValidity MeasureValidity { get; set; }
        public string WorkpieceDataCode { get; set; }
        public string FixtureDataCode { get; set; }

        public bool IsNG
        {
            get
            {
                return StationResults.Any() && StationResults.Any(x => x.IsNG);
            }
        }

        public bool IsNG2
        {
            get
            {
                return StationResults.Any() && StationResults.Any(x => x.IsNG2);
            }
        }

        public bool IsOver20um
        {
            get
            {
                return StationResults.Any() && StationResults.Any(x => x.IsOver20um);
            }
        }

        public bool IsLevelA
        {
            get
            {
                return StationResults.Any() && StationResults.All(x => !x.IsNG2);
            }
        }

        public bool IsLevelB
        {
            get
            {
                return StationResults.Any() && IsNG2 && StationResults.All(x => !x.IsOver20um);
            }
        }

        public bool IsAbnormal
        {
            get
            {
                return StationResults.Any() && StationResults.Any(x => x.IsAbnormal);
            }
        }

        public DateTime? MeasureCompletedDateTime
        {
            get
            {
                return !StationResults.Any() ? null : StationResults.Select(x => x.MeasureDateTime).Max();
            }
        }
    }
}