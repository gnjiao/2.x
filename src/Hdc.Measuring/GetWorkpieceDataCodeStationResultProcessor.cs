using System;
using System.Diagnostics;
using System.Linq;

namespace Hdc.Measuring
{
    [Serializable]
    public class GetWorkpieceDataCodeStationResultProcessor : IStationResultProcessor
    {
        public void Process(StationResult stationResult)
        {
            try
            {
                var workpieceDataCodeMeasureResult = stationResult.MeasureResults
                    .Single(x => x.Index == MeasureResultIndex);
                var workpieceDataCodeMeasureOutput = workpieceDataCodeMeasureResult
                    .Outputs[MeasureOutputIndex];
                stationResult.WorkpieceDataCode = workpieceDataCodeMeasureOutput.Message;
            }
            catch (Exception)
            {
                var msg = $"{nameof(GetWorkpieceDataCodeStationResultProcessor)} error." +
                             $"MeasureResultIndex: " + MeasureResultIndex +
                             $"MeasureOutputIndex: " + MeasureOutputIndex;
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
        }

        public int MeasureResultIndex { get; set; }

        public int MeasureOutputIndex { get; set; }
    }
}