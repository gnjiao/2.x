using System;
using System.Collections.Generic;

namespace Hdc.Measuring
{
    [Serializable]
    public class ConstantValueCalculateOperation : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            return new MeasureOutput()
            {
                Value = Value,
            };
        }

        public double Value { get; set; }
    }
}