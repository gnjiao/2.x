using System;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class DistanceBetweenPointAndLineResult 
    {
        public DistanceBetweenPointAndLineDefinition Definition { get; set; }

        public bool HasError { get; set; }
        public int Index { get; set; }
        public double Distance { get; set; }

        public double PointX { get; set; }
        public double PointY { get; set; }
        public double LineX1 { get; set; }
        public double LineY1 { get; set; }
        public double LineX2 { get; set; }
        public double LineY2 { get; set; }

        public double PointXInWorld => PointX.ToMillimeterFromPixel(Definition.PixelCellSideLengthInMillimeter);
        public double PointYInWorld => PointY.ToMillimeterFromPixel(Definition.PixelCellSideLengthInMillimeter);
        public double LineX1InWorld => LineX1.ToMillimeterFromPixel(Definition.PixelCellSideLengthInMillimeter);
        public double LineY1InWorld => LineY1.ToMillimeterFromPixel(Definition.PixelCellSideLengthInMillimeter);
        public double LineX2InWorld => LineX2.ToMillimeterFromPixel(Definition.PixelCellSideLengthInMillimeter);
        public double LineY2InWorld => LineY2.ToMillimeterFromPixel(Definition.PixelCellSideLengthInMillimeter);
        public double DistanceInWorld
            => Distance.ToMillimeterFromPixel(Definition.PixelCellSideLengthInMillimeter);

    }
}