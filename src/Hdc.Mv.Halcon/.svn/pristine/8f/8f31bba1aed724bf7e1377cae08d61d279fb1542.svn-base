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
    public class GetTwoPointsWithRegionsCoordinateExtactor : ICoordinateExtactor
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
            foreach (var region in CoordinateRegions)
            {
                pt[region.Index] = region.Region.GetCenterPoint();
            }
            Point originPoint = new Point();
            switch (PointNumber)
            {
                case 1:
                    originPoint.X = pt[0].X;
                    originPoint.Y = pt[0].Y;
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

            //默认两点连线为Y轴线，且从0到1为Y轴正向
            //AngleOffset若为正值则顺时针旋转
            CoordAngle = -CoordAngle - 90 + AngleOffset;
            //var unionregion = CoordinateRegions[0].Region.Union2(CoordinateRegions[1].Region);
            //double xx, yy, phi, length1, length2;
            //unionregion.SmallestRectangle2(out yy, out xx, out phi, out length1, out length2);
            //var phiangle = -phi / 3.1415926 * 180 - 90 + AngleOffset;
            //var _coordinate = RelativeCoordinateFactory.CreateCoordinateUsingPointAndAngle(xx, yy, phiangle);
            var _coordinate = RelativeCoordinateFactory.CreateCoordinateUsingPointAndAngle(
                    originPoint.X, originPoint.Y, CoordAngle);
            //var _coordinate = RelativeCoordinateFactory.CreateCoordinateUsingPointAndAngle(
            //        originPoint.X, originPoint.Y, coordinate.GetCoordinateAngle());
            if (_coordinate == null) throw new ArgumentNullException(nameof(_coordinate));
            return _coordinate;
        }
        public Collection<RegionSearchingDefinition> Definitions { get; set; } = new Collection<RegionSearchingDefinition>();
        public int PointNumber { get; set; } = 2;
        public bool UsingHorizontal { get; set; } = true;   //Way to definite point 0 and point 1. If false, Left means Up and Right means Down
        public string OriginPointXLocation { get; set; } = "Middle";  //Left, Right or Middle
        public string OriginPointYLocation { get; set; } = "Middle";  //Left, Right or Middle
        public double AngleOffset { get; set; } = 0;    //degree
        public double XOffset { get; set; } = 0;    //mm
        public double YOffset { get; set; } = 0;    //mm
        private double CoordAngle { get; set; }
    }
}

