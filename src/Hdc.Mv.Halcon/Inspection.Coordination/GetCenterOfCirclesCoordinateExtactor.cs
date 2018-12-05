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
    public class GetCenterOfCirclesCoordinateExtactor : ICoordinateExtactor
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
                    throw new NotImplementedException();
                    break;
                case 2:
                    throw new NotImplementedException();
                    break;
                case 3:
                    throw new NotImplementedException();
                    break;
                case 4:
                    //默认前两个圆的圆心连线为X轴线，后两个圆的圆心连线为Y轴线
                    //且从左到右为X轴正向，从上到下为Y轴正向
                    originPoint.X = (pt[0].X + pt[1].X) / 2;
                    originPoint.Y = (pt[2].Y + pt[3].Y) / 2;
                    if (pt[1].X >= pt[0].X)
                    {
                        Vector lineanglex = new Vector(pt[1].X - pt[0].X, pt[1].Y - pt[0].Y);
                        var coordAngleX1 = lineanglex.GetAngleToX();
                        if (pt[3].Y >= pt[2].Y)
                        {
                            Vector lineangley = new Vector(pt[3].X - pt[2].X, pt[3].Y - pt[2].Y);
                            var coordAngleX2 = lineangley.GetAngleToX() + 90;
                            CoordAngle = (coordAngleX1 + coordAngleX2) / 2;
                        }
                        else
                        {
                            Vector lineangley = new Vector(pt[2].X - pt[3].X, pt[2].Y - pt[3].Y);
                            var coordAngleX2 = lineangley.GetAngleToX() + 90;
                            CoordAngle = (coordAngleX1 + coordAngleX2) / 2;
                        }
                    }
                    else
                    {
                        Vector lineanglex = new Vector(pt[0].X - pt[1].X, pt[0].Y - pt[1].Y);
                        var coordAngleX1 = lineanglex.GetAngleToX();
                        if (pt[3].Y >= pt[2].Y)
                        {
                            Vector lineangley = new Vector(pt[3].X - pt[2].X, pt[3].Y - pt[2].Y);
                            var coordAngleX2 = lineangley.GetAngleToX() + 90;
                            CoordAngle = (coordAngleX1 + coordAngleX2) / 2;
                        }
                        else
                        {
                            Vector lineangley = new Vector(pt[2].X - pt[3].X, pt[2].Y - pt[3].Y);
                            var coordAngleX2 = lineangley.GetAngleToX() + 90;
                            CoordAngle = (coordAngleX1 + coordAngleX2) / 2;
                        }
                    }
                    break;
            }

            //AngleOffset若为正值则顺时针旋转
            CoordAngle = -CoordAngle + AngleOffset;
            var _coordinate = RelativeCoordinateFactory.CreateCoordinateUsingPointAndAngle(
                    originPoint.X, originPoint.Y, CoordAngle);
            if (_coordinate == null) throw new ArgumentNullException(nameof(_coordinate));
            return _coordinate;
        }
        public Collection<CircleSearchingDefinition> Definitions { get; set; } = new Collection<CircleSearchingDefinition>();
        public int PointNumber { get; set; } = 4;
        public double AngleOffset { get; set; } = 0;
        private double CoordAngle { get; set; }
    }
}

