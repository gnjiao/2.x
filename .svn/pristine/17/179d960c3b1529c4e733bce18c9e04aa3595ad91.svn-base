using System;
using System.Collections.Generic;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("Operation")]
    public class DivCalculateOperation : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            var op1 = Operation.Calculate(measureResults, definition);

            var value = op1.Value/Div;

            var measureOut = new MeasureOutput()
            {
                Validity = MeasureValidity.Valid,
                Value = value,
            };
            return measureOut;
        }

        public ICalculateOperation Operation { get; set; }
        public double Div { get; set; }
    }
}