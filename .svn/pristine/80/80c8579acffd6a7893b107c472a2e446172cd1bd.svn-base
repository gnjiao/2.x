using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("Operations")]
    public class MinCalculateOperation2 : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            var effectiveOutputs = Operations
                .Select(operation => operation.Calculate(measureResults, definition))
                .Select(output =>
                {
                    if (double.IsNaN(output.Value) || double.IsInfinity(output.Value))
                    {
                        return new
                        {
                            Value = output.Value,
                        };
                    }
                    else
                    {
                        return new
                        {
                            Value = output.Value,
                        };
                    }
                    
                })
                .Where(x => x.Value <= 999).ToList();


            if (effectiveOutputs.Count > 0)
            {
                var measureOut = new MeasureOutput()
                {
                    Validity = MeasureValidity.Valid,
                    Value = effectiveOutputs.Min(x => x.Value),
                };

                return measureOut;
            }
            else
            {
                var effectiveOutput = effectiveOutputs.FirstOrDefault();

                var measureOut = new MeasureOutput()
                {
                    Validity = MeasureValidity.Valid,
                    Value = effectiveOutput?.Value ?? 999.9999,
                };

                return measureOut;
            }
        }

        public Collection<ICalculateOperation> Operations { get; set; } = new Collection<ICalculateOperation>();


    }
}