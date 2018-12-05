using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
//    [ContentProperty("Operations")]
    public class FlatnessErrorOfACalculationOperation : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            MeasureOutput measureOutput = new MeasureOutput();

            var xs = XOperations
                .Select(operation => operation.Calculate(measureResults, definition))
                .Select(output => output.Value)
                .ToArray();

            var ys = YOperations
                .Select(operation => operation.Calculate(measureResults, definition))
                .Select(output => output.Value)
                .ToArray();

            var zs = ZOperations
                .Select(operation => operation.Calculate(measureResults, definition))
                .Select(output => output.Value)
                .ToArray();

            var xValues = xs;
            var yValues = ys;
            var zValues = zs;

            double[] distances;
            double f;
            FlatnessErrorOfA.FlatnessErrorOfACalculate(xValues, yValues, zValues, out distances, out f);

            return measureOutput;
        }

        public Collection<ICalculateOperation> XOperations { get; set; } = new Collection<ICalculateOperation>();
        public Collection<ICalculateOperation> YOperations { get; set; } = new Collection<ICalculateOperation>();
        public Collection<ICalculateOperation> ZOperations { get; set; } = new Collection<ICalculateOperation>();
    }
}