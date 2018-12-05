using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Hdc.Collections.Generic;
using HalconDotNet;
using Hdc.Mv.Halcon;

namespace Hdc.Mv.Inspection
{
    public class DistanceBetweenPointAndLineInspector
    {
        public DistanceBetweenPointAndLineResult CalculateDistanceBetweenPointAndLine(HImage image,
            DistanceBetweenPointAndLineDefinition definition,
            InspectionResult inspectionResult, 
            IDictionary<string, IRelativeCoordinate> coordinates)
        {
            var result = new DistanceBetweenPointAndLineResult()
            {
                Definition = definition.DeepClone()
            };

            var pointResult = inspectionResult.PointOfEdgeAndRadialLineResults.Where(x => x.Definition != null)
                .SingleOrDefault(x => x.Definition.Name == definition.PointName);
            
            var fitEdgeLine = inspectionResult.EdgeSearchingOfRegionPointsResults
                .Where(x => x.Definition != null)
                .SingleOrDefault(x => x.Definition.Name == definition.LineName);

            var edgeLine = inspectionResult.Edges.Where(x => x.Definition != null)
                .SingleOrDefault(x => x.Definition.Name == definition.LineName);
            
            HTuple distanceLength = new HTuple();

            result.PointX = pointResult.X;
            result.PointY = pointResult.Y;

            if (definition.IsUsingFitLine)
            {
                result.LineX1 = fitEdgeLine.X1;
                result.LineY1 = fitEdgeLine.Y1;
                result.LineX2 = fitEdgeLine.X2;
                result.LineY2 = fitEdgeLine.Y2;
            }
            else
            {
                result.LineX1 = edgeLine.X1;
                result.LineY1 = edgeLine.Y1;
                result.LineX2 = edgeLine.X2;
                result.LineY2 = edgeLine.Y2;
            }

            HOperatorSet.DistancePl(result.PointY, result.PointX, result.LineY1, result.LineX1, result.LineY2, result.LineX2, out distanceLength);

            var vector1 = new Vector(result.LineX2 - result.LineX1, result.LineY2 - result.LineY1);
            var vector2 = new Vector(result.PointX - result.LineX1, result.PointY - result.LineY1);

            if (definition.IsUsingAbsolute)
            {
                result.Distance = distanceLength;
            }
            else
            {
                result.Distance = vector1.GetAngleToX() < vector2.GetAngleToX() ? distanceLength : -distanceLength;
            }

            return result;
        }
    }
}