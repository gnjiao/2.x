using System;
using System.Collections.Generic;
using System.Windows;

namespace Hdc.Measuring
{
    [Serializable]
    public class GetDistanceBetweenTwoCircleCalculationOperation : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            var x1 = Circle1XOperation.Calculate(measureResults, definition).Value;
            var y1 = Circle1YOperation.Calculate(measureResults, definition).Value;
            var x2 = Circle2XOperation.Calculate(measureResults, definition).Value;
            var y2 = Circle2YOperation.Calculate(measureResults, definition).Value;

            var vector1 = new Vector() { X = x1, Y = y1 };
            var vector2 = new Vector() { X = x2, Y = y2 };
            var link = vector2 - vector1;
            var linkLength = link.Length;
            return new MeasureOutput() { Value = linkLength };
        }

        public ICalculateOperation Circle1XOperation { get; set; }
        public ICalculateOperation Circle1YOperation { get; set; }
        public ICalculateOperation Circle2XOperation { get; set; }
        public ICalculateOperation Circle2YOperation { get; set; }
    }
}