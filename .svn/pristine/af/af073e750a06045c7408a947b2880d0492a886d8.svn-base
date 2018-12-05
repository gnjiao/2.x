using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("Operations")]
    public class SumCalculateOperation : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            MeasureOutput measureOutput = new MeasureOutput();

            measureOutput.Value = Operations
                .Select(operation => operation.Calculate(measureResults, definition))
                .Select(output => output.Value)
                .ToList().Sum();
            return measureOutput;
        }

        public Collection<ICalculateOperation> Operations { get; set; } = new Collection<ICalculateOperation>();
    }
}