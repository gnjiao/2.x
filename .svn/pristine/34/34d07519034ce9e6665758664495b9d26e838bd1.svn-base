using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("Operations")]
    public class GetSmallestDeviationMeasureOutputCalculationOperation : ICalculateOperation
    {
//        private CalculateDefinition _calculateDefinition;
//        public void Initialize(CalculateDefinition calculateDefinition)
//        {
//            _calculateDefinition = calculateDefinition;
//        }

        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            var orderBy = Operations
                .Select(operation => operation.Calculate(measureResults, definition))
                .Select(output =>
                {
                    if (double.IsNaN(output.Value) || double.IsInfinity(output.Value))
                    {
                        return new
                        {
                            Value = output.Value,
                            Deviation = -999.999,
                            DeviationOverTolerance = -999.999,
                            DeviationOverToleranceAbs = -999.999
                        };
                    }

                    var finalValue = output.Value + SystemOffsetValue;
                    double devOverTolerance;

                    if (finalValue > LimitUpper)
                        devOverTolerance = finalValue - LimitUpper;
                    else if (finalValue < LimitLower)
                        devOverTolerance = finalValue - LimitLower;
                    else
                        devOverTolerance = 0;

                    return new
                    {
                        Value = output.Value,
                        Deviation = output.Value - StandardValue,
                        DeviationOverTolerance = devOverTolerance,
                        DeviationOverToleranceAbs = Math.Abs(devOverTolerance)
                    };
                })
                .OrderBy(x => x.DeviationOverToleranceAbs).ToList();

            var bestOutputs = orderBy.Where(x => x.DeviationOverToleranceAbs <= DeviationOverToleranceAbsLimited).ToList();
            if (bestOutputs.Count > 0)
            {
                var measureOut = new MeasureOutput()
                {
                    Validity = MeasureValidity.Valid,
                    Value = bestOutputs.Average(x=>x.Value),
                };

                return measureOut;
            }
            else
            {
                var bestOutput = orderBy.FirstOrDefault();

                var measureOut = new MeasureOutput()
                {
                    Validity = MeasureValidity.Valid,
                    Value = bestOutput?.Value ?? 999.9999,
                };

                return measureOut;
            }
        }

        public double StandardValue { get; set; }
        public double ToleranceUpper { get; set; }
        public double ToleranceLower { get; set; }
        public double SystemOffsetValue { get; set; }
        public double LimitUpper => StandardValue + ToleranceUpper;
        public double LimitLower => StandardValue + ToleranceLower;
        public Collection<ICalculateOperation> Operations { get; set; } = new Collection<ICalculateOperation>();

        /// <summary>
        /// etc: 0.0mm, 0.02mm
        /// </summary>
        public double DeviationOverToleranceAbsLimited { get; set; } = 0;
    }
}