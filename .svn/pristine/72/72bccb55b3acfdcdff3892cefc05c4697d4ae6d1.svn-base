using System;
using System.Collections.Generic;
using System.Linq;

namespace Hdc.Measuring
{
    [Serializable]
    public class GetMeasureOutputCalculateOperation : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            var measureOutput = measureResults.Single(x=>x.Index== MeasureResultIndex).Outputs[MeasureOutputIndex].DeepClone();

            measureOutput.Value += Offset;

            if (IsNegative)
                measureOutput.Value *= -1;
            return measureOutput;
        }

        public int MeasureResultIndex { get; set; }

        public int MeasureOutputIndex { get; set; }

        public bool IsNegative { get; set; }

        public double Offset { get; set; }
    }
}