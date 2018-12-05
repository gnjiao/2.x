using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Markup;
using Hdc.Collections.Generic;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("Operations")]
    public class GetValueFromMaxMinusMinCalculateOperation : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            List<double> value = new List<double>();
            foreach (var operation in Operations)
            {
                if (operation.Calculate(measureResults, definition).Value <= MaxValue && operation.Calculate(measureResults, definition).Value >= MinValue)
                {
                    value.Add(operation.Calculate(measureResults, definition).Value);
                }
            }
            var measureOut = new MeasureOutput();
            try
            {
                measureOut.Validity = MeasureValidity.Valid;
                measureOut.Value = value.Max() - value.Min();
            }
            catch (Exception)
            {

                measureOut.Validity = MeasureValidity.Valid;
                measureOut.Value = 999.9999;
            }


            return measureOut;
        }
        public Collection<ICalculateOperation> Operations { get; set; } = new Collection<ICalculateOperation>();
        public double Standrd { get; set; } = 0;
        public double MinValue { get; set; } = -99999999;
        public double MaxValue { get; set; } = 99999999;
    }
    
}

