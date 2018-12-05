using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
//    [ContentProperty("ConstantOperations")]
    public class GetSmallestDeviationOfStepCalculationOperation : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            IList<TempValue> tempValues = new List<TempValue>();

            foreach (var op1 in Operations1)
            {
                var v1 = op1.Calculate(measureResults, definition).Value;
                foreach (var op2 in Operations2)
                {
                    var v2 = op2.Calculate(measureResults, definition).Value;

                    var value = v1 + v2 + AddConstantValue;
                    var finalValue = value + SystemOffsetValue;

                    double devOverTolerance;

                    if (finalValue > LimitUpper)
                        devOverTolerance = finalValue - LimitUpper;
                    else if (finalValue < LimitLower)
                        devOverTolerance = finalValue - LimitLower;
                    else
                        devOverTolerance = 0;

                    var tv = new TempValue()
                    {
                        Value = value,
                        Deviation = value - StandardValue,
                        DeviationOverTolerance = devOverTolerance,
                        DeviationOverToleranceAbs = Math.Abs(devOverTolerance)
                    };

                    tempValues.Add(tv);
                }
            }

            var orderBy = tempValues.OrderBy(x=>x.DeviationOverToleranceAbs);
            var bestOutputs = orderBy.Where(x => x.DeviationOverToleranceAbs <= 0.021).ToList();

            if (bestOutputs.Count > 0)
            {
                var measureOut = new MeasureOutput()
                {
                    Validity = MeasureValidity.Valid,
                    Value = bestOutputs.Average(x => x.Value),
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

        public CalculateOperationCollection Operations1 { get; set; } = new CalculateOperationCollection();

        public CalculateOperationCollection Operations2 { get; set; } = new CalculateOperationCollection();

//        [Obsolete]
//        public CalculateOperationCollection ConstantOperations { get; set; } = new CalculateOperationCollection();

        public double AddConstantValue { get; set; }

        public double StandardValue { get; set; }
        public double ToleranceUpper { get; set; }
        public double ToleranceLower { get; set; }
        public double SystemOffsetValue { get; set; }
        public double LimitUpper => StandardValue + ToleranceUpper;
        public double LimitLower => StandardValue + ToleranceLower;


        private class TempValue
        {
            public double Value { get; set; }
            public double Deviation { get; set; }
            public double DeviationOverTolerance { get; set; }
            public double DeviationOverToleranceAbs { get; set; }
        }
    }
}