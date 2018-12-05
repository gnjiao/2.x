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
    public class MaxCalculateOperationMinusMinCalculateOperation : ICalculateOperation
    {
        //        private CalculateDefinition _calculateDefinition;
        //        public void Initialize(CalculateDefinition calculateDefinition)
        //        {
        //            _calculateDefinition = calculateDefinition;
        //        }

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
                .Where(x => x.Value < 999).ToList();


            if (effectiveOutputs.Count > 0)
            {
                var measureOut = new MeasureOutput()
                {
                    Validity = MeasureValidity.Valid,
                    Value = effectiveOutputs.Max(x => x.Value)- effectiveOutputs.Min(x=>x.Value),
                };

                return measureOut;
            }
            else
            {
                var measureOut = new MeasureOutput()
                {
                    Validity = MeasureValidity.Valid,
                    Value = effectiveOutputs.Max(x => x.Value) - effectiveOutputs.Min(x => x.Value),
                };

                return measureOut;
            }
        }

        public Collection<ICalculateOperation> Operations { get; set; } = new Collection<ICalculateOperation>();

    }
}