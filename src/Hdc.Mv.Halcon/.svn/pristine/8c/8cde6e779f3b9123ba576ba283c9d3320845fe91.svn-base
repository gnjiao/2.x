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
    public class EdgeSearchingOfRegionPointsInspector
    {
        public EdgeSearchingOfRegionPointsResult CalculateEdgeSearchingOfRegionPoints(
                    HImage image,
                    EdgeSearchingOfRegionPointsDefinition definition,
                    InspectionResult inspectionResult)
        {
            var result = new EdgeSearchingOfRegionPointsResult()
            {
                Definition = definition.DeepClone()
            };

            result.ResultLine = new LineDefinition();
            List<PointOfEdgeAndRadialLineResult> pointResult = new List<PointOfEdgeAndRadialLineResult>();

            foreach (var point in inspectionResult.PointOfEdgeAndRadialLineResults)
            {
                if (point.Definition.Name.Contains(definition.PointName))
                    pointResult.Add(point);
            }
            
            if (pointResult.Count < 2)
            {
                result.HasError = true;
                return result;
            }

            HTuple pointX = new HTuple();
            HTuple pointY = new HTuple();
            HObject contour = new HObject();
            HTuple rowBegin, colBegin, rowEnd, colEnd, nr, nc, dist;
            for (int i = 0; i < pointResult.Count; i++)
            {
                pointX[i] = pointResult[i].X;
                pointY[i] = pointResult[i].Y;
            }

            HOperatorSet.GenContourPolygonXld(out contour, pointY, pointX);
            HOperatorSet.FitLineContourXld(contour, definition.Algorithm, definition.MaxNumPoints, definition.ClippingEndPoints,
                definition.Iterations, definition.ClippingFactor, out rowBegin, out colBegin, out rowEnd, out colEnd, out nr, out nc, out dist);

            result.X1 = colBegin;
            result.Y1 = rowBegin;
            result.X2 = colEnd;
            result.Y2 = rowEnd;

            Line finaLine = new Line(colBegin, rowBegin, colEnd, rowEnd);

            result.ResultLine.Name = definition.Name;
            result.ResultLine.ActualLine = finaLine;

            return result;
        }
    }
}