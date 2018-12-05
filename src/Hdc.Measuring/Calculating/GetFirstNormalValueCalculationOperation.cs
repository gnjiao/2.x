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
    public class GetFirstNormalValueCalculationOperation : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            MeasureOutput measureOutput = new MeasureOutput();

            foreach (var calculateOperation in Operations)
            {
                measureOutput = calculateOperation.Calculate(measureResults, definition);

                var finalValue = measureOutput.Value + definition.SystemOffsetValue;

                var isAbnormalValue = finalValue.CheckIsAbnormalValue(
                    definition.StandardValue,
                    definition.ToleranceUpperWithAbnormalValueLimitUpper,
                    definition.ToleranceLowerWithAbnormalValueLimitLower);

                if (!isAbnormalValue)
                {
                    return measureOutput;
                }
            }

            return measureOutput;
        }
        
        public Collection<ICalculateOperation> Operations { get; set; } = new Collection<ICalculateOperation>();
    }
}