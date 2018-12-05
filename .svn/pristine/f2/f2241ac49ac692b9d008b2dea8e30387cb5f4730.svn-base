using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Markup;
using Hdc;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("Operations")]
    public class GetLimitedValueCalculationOperation : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            MeasureOutput measureOutput = new MeasureOutput();

            foreach (var calculateOperation in Operations)
            {
                measureOutput = calculateOperation.Calculate(measureResults, definition);

                var finalValue = measureOutput.Value;

                var isLimitedValue = finalValue.CheckIsLimitedValue(UpperValue,LowerValue);

                if (!isLimitedValue)
                {
                    if (IsNegative)
                        measureOutput.Value = (-measureOutput.Value);

                    return measureOutput;
                }
            }


            return measureOutput;
        }
        
        public Collection<ICalculateOperation> Operations { get; set; } = new Collection<ICalculateOperation>();

        public double UpperValue { get; set; }
        public double LowerValue { get; set; }

        public bool IsNegative { get; set; }
    }
}