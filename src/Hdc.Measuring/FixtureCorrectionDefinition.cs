using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class FixtureCorrectionDefinition
    {
        public string FixtureDataCode { get; set; }

        public double CorrectionValue { get; set; }

//        public int StationIndex { get; set; }
//
//        public string StationName { get; set; }
//
//        public string CalculationSipNo { get; set; }
//
//        public string CalculationName { get; set; }
    }
}