using System;
using System.Collections.Generic;
using System.Windows;
using Hdc.Mv;
using Hdc.Mv.Halcon;

namespace Hdc.Measuring
{
    [Serializable]
    public class GetDistanceBetweenLinesCalculationOperation : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            var Line1X1 = Line1X1Operation.Calculate(measureResults, definition).Value;
            var Line1Y1 = Line1Y1Operation.Calculate(measureResults, definition).Value;
            var Line1X2 = Line1X2Operation.Calculate(measureResults, definition).Value;
            var Line1Y2 = Line1Y2Operation.Calculate(measureResults, definition).Value;
            var Line2X1 = Line2X1Operation.Calculate(measureResults, definition).Value;
            var Line2Y1 = Line2Y1Operation.Calculate(measureResults, definition).Value;
            var Line2X2 = Line2X2Operation.Calculate(measureResults, definition).Value;
            var Line2Y2 = Line2Y2Operation.Calculate(measureResults, definition).Value;


            var line1 = new Line(Line1X1, Line1Y1, Line1X2, Line1Y2);
            var line2 = new Line(Line2X1, Line2Y1, Line2X2, Line2Y2);


            var line1Angle = line1.GetVectorFrom1To2().GetAngleToX();
            var line2Angle = line2.GetVectorFrom1To2().GetAngleToX();

            if (Math.Abs(line1Angle - line2Angle) > 90)
            {
                line2 = new Line(Line2X2, Line2Y2 , Line2X1, Line2Y1);
                line2Angle = line2.GetVectorFrom1To2().GetAngleToX();
            }


            var midLineX1 = (Line1X1 + Line2X1) / 2.0;
            var midLineX2 = (Line1X2 + Line2X2) / 2.0;
            var midLineY1 = (Line1Y1 + Line2Y1) / 2.0;
            var midLineY2 = (Line1Y2 + Line2Y2) / 2.0;
            var midLine = new Line(midLineX1, midLineY1, midLineX2, midLineY2);
            var midLineCenter = midLine.GetCenterPoint().ToVector();

            
            var midLineAngle = (line1Angle + line2Angle) / 2.0;

            var vectorX = new Vector(100000,0).Rotate(-midLineAngle);
            var linkLineStartPoint=vectorX.Rotate(90.0);
            var linkLineEndPoint=vectorX.Rotate(-90.0);
            var linkLineStartVector = linkLineStartPoint+midLineCenter;
            var linkLineEndVector = linkLineEndPoint + midLineCenter;
            var linkLine = new Line(linkLineStartVector.ToPoint(), linkLineEndVector.ToPoint());

            var intersectPointToLine1 = HalconExtensions.IntersectionWith(line1, linkLine);
            var intersectPointToLine2 = HalconExtensions.IntersectionWith(line2, linkLine);
            var intersectLine = new Line(intersectPointToLine1, intersectPointToLine2);
            var length = intersectLine.GetLength();

            return new MeasureOutput() {Value = length};



        }

        public ICalculateOperation Line1X1Operation { get; set; }
        public ICalculateOperation Line1Y1Operation { get; set; }
        public ICalculateOperation Line1X2Operation { get; set; }
        public ICalculateOperation Line1Y2Operation { get; set; }
        public ICalculateOperation Line2X1Operation { get; set; }
        public ICalculateOperation Line2Y1Operation { get; set; }
        public ICalculateOperation Line2X2Operation { get; set; }
        public ICalculateOperation Line2Y2Operation { get; set; }
    }
}