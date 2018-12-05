using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using HalconDotNet;
using Hdc.Linq;
using Hdc.Mv.Halcon;
using Hdc.Mv.Inspection;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    [ContentProperty("RegionSearchingDefinition")]
    public class GetTwoPointsWithRegionsAndLinesCoordinateExtactor : ICoordinateExtactor
    {
        public string Name { get; set; }
        public string CoordinateName { get; set; }

        public IRelativeCoordinate Extact(HImage image, IRelativeCoordinate coordinate,
            double pixelCellSideLengthInMillimeter)
        {
            var inspector3 = InspectorFactory.CreateRegionSearchingInspector();

            // RegionSearchingDefinition.UpdateRelativeCoordinate(coordinate);
            foreach (var RegionSearchingDefinition in Definitions)
            {
                RegionSearchingDefinition.UpdateRelativeCoordinate(coordinate);
            }
            var CoordinateRegions = inspector3.SearchRegions(image, Definitions);

            int num = CoordinateRegions.Count;
            Point[] pt = new Point[num];
            Point ptError = new Point(0, 0);
            foreach (var region in CoordinateRegions)
            {

                try
                {
                    pt[region.Index] = region.Region.GetCenterPoint();
                }
                catch (Exception)
                {
                    pt[region.Index] = ptError;
                }

            }
            if (pt[0].X > pt[1].X)
            {
                Point ptt = pt[1];
                pt[1] = pt[0];
                pt[0] = ptt;
            }

            foreach (var Edge in DefinitionEdges)
            {
                if (Edge.Name.Contains("LP"))
                {
                    Edge.StartX = Edge.StartX + pt[0].X;
                    Edge.StartY = Edge.StartY + pt[0].Y;
                    Edge.EndX = Edge.EndX + pt[0].X;
                    Edge.EndY = Edge.EndY + pt[0].Y;
                }
                else
                {
                    Edge.StartX = Edge.StartX + pt[1].X;
                    Edge.StartY = Edge.StartY + pt[1].Y;
                    Edge.EndX = Edge.EndX + pt[1].X;
                    Edge.EndY = Edge.EndY + pt[1].Y;
                }

            }

            var edegesinspector3 = InspectorFactory.CreateEdgeInspector("Hal");
            var edges = edegesinspector3.SearchEdges(image, DefinitionEdges);

            Point originPoint = new Point();
            //Exception
            if (pt[0].X + pt[0].Y < 0.01 || pt[1].X + pt[1].Y < 0.01)
            {
                originPoint = new Point(0, 0);
                CoordAngle = AngleOffset;
            }
            else
            {
                originPoint = GetCoordPointWith2Points(edges);
            }

            var _coordinate = RelativeCoordinateFactory.CreateCoordinateUsingPointAndAngle(originPoint.X, originPoint.Y, CoordAngle);

            if (_coordinate == null) throw new ArgumentNullException(nameof(_coordinate));
            return _coordinate;
        }
        public Collection<RegionSearchingDefinition> Definitions { get; set; } = new Collection<RegionSearchingDefinition>();
        public Collection<EdgeSearchingDefinition> DefinitionEdges { get; set; } = new Collection<EdgeSearchingDefinition>();
        public int PointNumber { get; set; } = 2;
        public string PointType { get; set; } = "Center";   //Center or Cross, Center will have 4 lines and Cross will have 2.
        public string OriginPointXLocation { get; set; } = "Middle";  //Left, Right or Middle
        public string OriginPointYLocation { get; set; } = "Middle";  //Left, Right or Middle
        public double AngleOffset { get; set; } = 0;    //degree
        public double XOffset { get; set; } = 0;    //mm
        public double YOffset { get; set; } = 0;    //mm
        private double CoordAngle { get; set; }
        //默认两点连线为Y轴线，且从0到1为Y轴正向
        //AngleOffset若为正值则顺时针旋转

        private Point GetCoordPointWith2Points(IList<EdgeSearchingResult> CoordinateLines)
        {
            Point originPoint = new Point();
            List<Line> lpTB = new List<Line>();
            List<Line> lpLR = new List<Line>();
            List<Line> rpTB = new List<Line>();
            List<Line> rpLR = new List<Line>();
            foreach (var edgeline in CoordinateLines)
            {
                string lineName = edgeline.Definition.Name;
                if (lineName.Contains("LPTB"))
                {
                    Line _line = new Line();
                    _line = edgeline.EdgeLine.X1 < edgeline.EdgeLine.X2
                                 ? edgeline.EdgeLine
                                 : edgeline.EdgeLine.Reverse();
                    lpTB.Add(_line);
                }
                if (lineName.Contains("LPLR"))
                {
                    Line _line = new Line();
                    _line = edgeline.EdgeLine.Y1 < edgeline.EdgeLine.Y2
                                 ? edgeline.EdgeLine
                                 : edgeline.EdgeLine.Reverse();
                    lpLR.Add(_line);
                }
                if (lineName.Contains("RPTB"))
                {
                    Line _line = new Line();
                    _line = edgeline.EdgeLine.X1 < edgeline.EdgeLine.X2
                                 ? edgeline.EdgeLine
                                 : edgeline.EdgeLine.Reverse();
                    rpTB.Add(_line);
                }
                if (lineName.Contains("RPLR"))
                {
                    Line _line = new Line();
                    _line = edgeline.EdgeLine.Y1 < edgeline.EdgeLine.Y2
                                 ? edgeline.EdgeLine
                                 : edgeline.EdgeLine.Reverse();
                    rpLR.Add(_line);
                }
            }
            Point leftPoint = new Point();
            Point rightPoint = new Point();
            switch (PointType)
            {
                case "Center":
                    Line lphorizontalMiddleLines = new Line();
                    Line lpverticalMiddleLines = new Line();
                    Line rphorizontalMiddleLines = new Line();
                    Line rpverticalMiddleLines = new Line();

                    lphorizontalMiddleLines = lpTB[0].GetMiddleLineUsingAngle(lpTB[1]);
                    lpverticalMiddleLines = lpLR[0].GetMiddleLineUsingAngle(lpLR[1]);
                    rphorizontalMiddleLines = rpTB[0].GetMiddleLineUsingAngle(rpTB[1]); ;
                    rpverticalMiddleLines = rpLR[0].GetMiddleLineUsingAngle(rpLR[1]); ;

                    leftPoint = lphorizontalMiddleLines.IntersectionWith(lpverticalMiddleLines);
                    rightPoint = rphorizontalMiddleLines.IntersectionWith(rpverticalMiddleLines);
                    break;
                case "Cross":
                    leftPoint = lpTB[0].IntersectionWith(lpLR[0]);
                    rightPoint = rpTB[0].IntersectionWith(rpLR[0]);
                    break;
            }
            switch (OriginPointXLocation)
            {
                case "Middle":
                    originPoint.X = (leftPoint.X + rightPoint.X) / 2 + XOffset * 1000 / 12;
                    break;
                case "Left":
                    originPoint.X = leftPoint.X + XOffset * 1000 / 12;
                    break;
                case "Right":
                    originPoint.X = rightPoint.X + XOffset * 1000 / 12;
                    break;
            }
            switch (OriginPointYLocation)
            {
                case "Middle":
                    originPoint.Y = (leftPoint.Y + rightPoint.Y) / 2 + YOffset * 1000 / 12;
                    break;
                case "Left":
                    originPoint.Y = leftPoint.Y + YOffset * 1000 / 12;
                    break;
                case "Right":
                    originPoint.Y = rightPoint.Y + YOffset * 1000 / 12;
                    break;
            }
            Vector lineangle = new Vector(rightPoint.X - leftPoint.X, rightPoint.Y - leftPoint.Y);
            CoordAngle = lineangle.GetAngleToX();                                                       //改
            CoordAngle = -CoordAngle - 90 + AngleOffset;
            return originPoint;
        }
    }
}