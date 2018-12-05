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
    [ContentProperty("CircleSearchingDefinition")]
    public class GetTwoPointsWithCircleCoordinateExtactor : ICoordinateExtactor
    {
        public string Name { get; set; }
        public string CoordinateName { get; set; }

        public IRelativeCoordinate Extact(HImage image, IRelativeCoordinate coordinate,
            double pixelCellSideLengthInMillimeter)
        {
            var inspector4 = InspectorFactory.CreateCircleInspector("Hal");

            // RegionSearchingDefinition.UpdateRelativeCoordinate(coordinate);
            foreach (var circleSearchingDefinition in Definitions)
            {
                circleSearchingDefinition.UpdateRelativeCoordinate(coordinate);
            }
            var coordinateCircles = inspector4.SearchCircles(image, Definitions);

            int num = coordinateCircles.Count;
            Point[] pt = new Point[num];
            foreach (var cirlce in coordinateCircles)
            {
                pt[cirlce.Index].X = cirlce.CenterX;
                pt[cirlce.Index].Y = cirlce.CenterY;
            }
            Point originPoint = new Point();
            switch (PointNumber)
            {
                case 1:
                    originPoint.X = pt[0].X + XOffset * 1000 / 12;
                    originPoint.Y = pt[0].Y + YOffset * 1000 / 12;
                    break;
                case 2:
                    Point leftPoint = new Point();
                    Point rightPoint = new Point();
                    if (UsingHorizontal)
                    {
                        if (pt[1].X > pt[0].X)
                        {
                            leftPoint = pt[0];
                            rightPoint = pt[1];
                        }
                        else
                        {
                            leftPoint = pt[1];
                            rightPoint = pt[0];
                        }
                    }
                    else
                    {
                        if (pt[1].Y > pt[0].Y)
                        {
                            leftPoint = pt[0];
                            rightPoint = pt[1];
                        }
                        else
                        {
                            leftPoint = pt[1];
                            rightPoint = pt[0];
                        }
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
                    CoordAngle = lineangle.GetAngleToX();

                    break;
                case 3:
                    throw new NotImplementedException();
                    break;
                case 4:
                    throw new NotImplementedException();
                    //                                    originPoint = line.Y1 > line.Y2 ? line.GetPoint1() : line.GetPoint2();
                    break;
                case 5:
                    throw new NotImplementedException();
                    //                                    originPoint = line.Y1 < line.Y2 ? line.GetPoint1() : line.GetPoint2();
                    break;
            }

            //coordinate.GetCoordinateAngle();
            //默认两点连线为Y轴线，且从0到1为Y轴正向
            //AngleOffset若为正值则顺时针旋转
            CoordAngle = -90 - CoordAngle + AngleOffset;
            var _coordinate = RelativeCoordinateFactory.CreateCoordinateUsingPointAndAngle(
                    originPoint.X, originPoint.Y, CoordAngle);
            if (_coordinate == null) throw new ArgumentNullException(nameof(_coordinate));
            return _coordinate;
        }
        public Collection<CircleSearchingDefinition> Definitions { get; set; } = new Collection<CircleSearchingDefinition>();
        public int PointNumber { get; set; } = 2;   //If PointNumber = 1, UsingHorizontal, OriginPointXLocation and OriginPointYLocation are useless.
        public bool UsingHorizontal { get; set; } = true;   //Way to definite point 0 and point 1. If false, Left means Up and Right means Down
        public string OriginPointXLocation { get; set; } = "Middle";  //Left, Right or Middle
        public string OriginPointYLocation { get; set; } = "Middle";  //Left, Right or Middle
        public double AngleOffset { get; set; } = 0;    //degree
        public double XOffset { get; set; } = 0;    //mm
        public double YOffset { get; set; } = 0;    //mm
        private double CoordAngle { get; set; }
    }
}

