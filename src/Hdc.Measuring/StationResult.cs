using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hdc.Measuring
{
    [Serializable]
    public class StationResult
    {
        public long Id { get; set; }
//        public DateTime? TimeStamp { get; set; }
        public Guid? MessageId { get; set; }
        public int StationIndex { get; set; }
        public string StationName { get; set; }
        public string StationDescription { get; set; }
        public int WorkpieceTag { get; set; }
        public int MeasureTag { get; set; }
        public DateTime? MeasureDateTime { get; set; }
        public List<MeasureResult> MeasureResults { get; set; } = new List<MeasureResult>();
        public List<CalculateResult> CalculateResults { get; set; } = new List<CalculateResult>();
        public MeasureValidity Validity { get; set; }
        public string WorkpieceDataCode { get; set; }
        public string FixtureDataCode { get; set; }

        public bool IsNG
        {
            get
            {
                return CalculateResults.Any() && CalculateResults.Any(x => x.IsNG);
            }
        }

        public bool IsNG2
        {
            get
            {
                return CalculateResults.Any() && CalculateResults.Any(x => x.IsNG2);
            }
        }

        public bool IsOver20um
        {
            get
            {
                return CalculateResults.Any() && CalculateResults.Any(x => x.IsOver20um);
            }
        }

        public bool IsAbnormal
        {
            get
            {
                return CalculateResults.Any() && CalculateResults.Any(x => x.IsAbnormal);
            }
        }

        public bool HasError
        {
            get
            {
                return MeasureResults.Any() && MeasureResults.Any(x => x.HasError);
            }
        }

        public bool HasWarning
        {
            get
            {
                return MeasureResults.Any() && MeasureResults.Any(x => x.HasWarning);
            }
        }

        public int DisplayIndex => StationIndex + 1;
    }
}