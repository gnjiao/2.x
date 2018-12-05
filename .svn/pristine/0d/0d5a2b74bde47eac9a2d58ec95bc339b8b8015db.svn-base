using System;
using System.Collections.Generic;
using System.Linq;

namespace Hdc.Measuring
{
    [Serializable]
    public class XyGetMeasureOutputCalculateOperation : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            MeasureOutput measureOutput;

            var measureResult = measureResults.SingleOrDefault(x => x.Index == MeasureResultIndex1);

            if (measureResult != null)
            {
                measureOutput = measureResult.Outputs[MeasureOutputIndex1].DeepClone();
            }
            else
            {
                var single = measureResults.Single(x => x.Index == MeasureResultIndex2);
                measureOutput = single.Outputs[MeasureOutputIndex2].DeepClone();
            }

            measureOutput.Value += Offset;

            if (IsNegative)
                measureOutput.Value *= -1;
            return measureOutput;
        }

        public int MeasureResultIndex1 { get; set; }
        public int MeasureResultIndex2 { get; set; }

        public int MeasureOutputIndex1 { get; set; }
        public int MeasureOutputIndex2 { get; set; }
  

        public bool IsNegative { get; set; }
        
        public double Offset { get; set; }
    }
}