using System;
using System.Collections.Generic;

namespace Hdc.Measuring
{
    [Serializable]
    public class StepCalculateWithZOperation : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            var op1 = Operation1.Calculate(measureResults, definition);
            var op2 = Operation2.Calculate(measureResults, definition);

            //            if (op1.Validity == MeasureOutputValidity.Valid &&
            //                op2.Validity == MeasureOutputValidity.Valid)
            //            {
            var value = (op2.Value - op1.Value) + (Z2 - Z1);
            if (IsAbsolute)
            {
                value = Math.Abs(value);
            }
            var measureOut = new MeasureOutput()
            {
                Validity = MeasureValidity.Valid,
                Value = value,
            };
            return measureOut;
            //            }
            //            else
            //            {
            //                var measureOut = new MeasureOutput()
            //                {
            //                    Validity = MeasureOutputValidity.Alarm,
            //                    Value = 0.000,
            //                };
            //                return measureOut;
            //            }
        }

        public ICalculateOperation Operation1 { get; set; }
        public ICalculateOperation Operation2 { get; set; }
        public double Z1 { get; set; }
        public double Z2 { get; set; }
        public bool IsAbsolute { get; set; }
    }
}