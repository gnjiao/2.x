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
    public class GetSmallestDeviationOfTwoPointsCalculationOperation : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {

            var p1x=Points1X[0].Calculate(measureResults, definition).Value;
            var p1y=Points1Y[0].Calculate(measureResults, definition).Value;
            var p2x=Points2X[0].Calculate(measureResults, definition).Value;
            var p2y=Points2Y[0].Calculate(measureResults, definition).Value;

            var valueX = p1x + p2x + AddConstantXValue;
            var valueY = p1y + p2y + AddConstantYValue;

            var distance = Math.Sqrt(valueY*valueY + valueX*valueX);
            var measureOut = new MeasureOutput()
            {
                Validity = MeasureValidity.Valid,
                Value = distance,
            };

            return measureOut;

        }

        public CalculateOperationCollection Points1X { get; set; } = new CalculateOperationCollection(); 

        public CalculateOperationCollection Points1Y { get; set; } = new CalculateOperationCollection();

        public CalculateOperationCollection Points2X { get; set; } = new CalculateOperationCollection();

        public CalculateOperationCollection Points2Y { get; set; } = new CalculateOperationCollection();

        //        [Obsolete]
        //        public CalculateOperationCollection ConstantOperations { get; set; } = new CalculateOperationCollection();

        public double AddConstantXValue { get; set; }
        public double AddConstantYValue { get; set; }

        public double StandardValue { get; set; }
        public double ToleranceUpper { get; set; }
        public double ToleranceLower { get; set; }
        public double SystemOffsetValue { get; set; }
        public double LimitUpper => StandardValue + ToleranceUpper;
        public double LimitLower => StandardValue + ToleranceLower;
    }
}
