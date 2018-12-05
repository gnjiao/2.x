using System;
using System.Collections.Generic;
using System.Windows;
using HalconDotNet;
using Hdc.Mv;
using Hdc.Mv.Halcon;

namespace Hdc.Measuring
{
    [Serializable]
    public class GetDistanceBetweenLineAndCircleCalculateOperation : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            var Line1X1 = Line1X1Operation.Calculate(measureResults, definition).Value;
            var Line1Y1 = Line1Y1Operation.Calculate(measureResults, definition).Value;
            var Line1X2 = Line1X2Operation.Calculate(measureResults, definition).Value;
            var Line1Y2 = Line1Y2Operation.Calculate(measureResults, definition).Value;
            var x1 = Circle1XOperation.Calculate(measureResults, definition).Value;
            var y1 = Circle1YOperation.Calculate(measureResults, definition).Value;

            var Length = HMisc.DistancePl(y1, x1, Line1Y1, Line1X1, Line1Y2, Line1X2);

            
            return new MeasureOutput() { Value = Length };
        }

        public ICalculateOperation Line1X1Operation { get; set; }
        public ICalculateOperation Line1Y1Operation { get; set; }
        public ICalculateOperation Line1X2Operation { get; set; }
        public ICalculateOperation Line1Y2Operation { get; set; }
        public ICalculateOperation Circle1XOperation { get; set; }
        public ICalculateOperation Circle1YOperation { get; set; }
    }
}