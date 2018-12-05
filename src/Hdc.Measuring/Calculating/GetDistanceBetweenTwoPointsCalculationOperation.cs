using System;
using System.Collections.Generic;
using System.Windows;

namespace Hdc.Measuring
{
    [Serializable]
    public class GetDistanceBetweenTwoPointsCalculationOperation : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            var X1 = GetX1Operation.Calculate(measureResults, definition).Value;
            var X2 = GetX2Operation.Calculate(measureResults, definition).Value;
            var Y1 = GetY1Operation.Calculate(measureResults, definition).Value;
            var Y2 = GetY2Operation.Calculate(measureResults, definition).Value;

            var vector1 = new Vector(X1,Y1);
            var vector2 = new Vector(X2,Y2);
            var link = vector1 - vector2;
            var distance = link.Length;

            var measureOut = new MeasureOutput()
            {
                Validity = MeasureValidity.Valid,
                Value = distance,
            };
            return measureOut;
        }

        public ICalculateOperation GetX1Operation { get; set; }
        public ICalculateOperation GetX2Operation { get; set; }
        public ICalculateOperation GetY1Operation { get; set; }
        public ICalculateOperation GetY2Operation { get; set; }
        public bool IsAbsolute { get; set; }
    }
}