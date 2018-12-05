using System;
using System.Diagnostics;
using System.Linq;

namespace Hdc.Measuring
{
    [Serializable]
    public class GetFixtureDataCodeStationResultProcessor : IStationResultProcessor
    {
        public void Process(StationResult stationResult)
        {
            try
            {
                if (!DebugEnable)
                {
                    var fixtureDataCodeMeasureResult = stationResult.MeasureResults
                        .Single(x => x.Index == MeasureResultIndex);
                    var fixtureDataCodeMeasureOutput = fixtureDataCodeMeasureResult
                        .Outputs[MeasureOutputIndex];
                	stationResult.FixtureDataCode = fixtureDataCodeMeasureOutput.Message;
                }
                else
                {
                    stationResult.FixtureDataCode = DebugDataCode;
                }
            }
            catch (Exception)
            {
                var msg = $"{nameof(GetFixtureDataCodeStationResultProcessor)} error." +
                             $"MeasureResultIndex: " + MeasureResultIndex +
                             $"MeasureOutputIndex: " + MeasureOutputIndex;
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
        }

        public int MeasureResultIndex { get; set; }

        public int MeasureOutputIndex { get; set; }

        public bool DebugEnable { get; set; } = false;

        public string DebugDataCode { get; set; }
    }
}