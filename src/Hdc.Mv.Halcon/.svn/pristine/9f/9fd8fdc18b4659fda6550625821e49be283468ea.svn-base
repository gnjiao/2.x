using System;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class EdgeSearchingOfRegionPointsResult
    {
        public EdgeSearchingOfRegionPointsDefinition Definition { get; set; }

        public string Name
        {
            get { return Definition.Name; }
            set { Definition.Name = value; }
        }
        public bool HasError { get; set; }
        public int Index { get; set; }
        public bool IsNotFound { get; set; }
        public LineDefinition ResultLine { get; set; }

        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }

        public double X1InWorld => X1.ToMillimeterFromPixel(Definition.PixelCellSideLengthInMillimeter);
        public double X2InWorld => X2.ToMillimeterFromPixel(Definition.PixelCellSideLengthInMillimeter);
        public double Y1InWorld => Y1.ToMillimeterFromPixel(Definition.PixelCellSideLengthInMillimeter);
        public double Y2InWorld => Y2.ToMillimeterFromPixel(Definition.PixelCellSideLengthInMillimeter);

        public double XMiddleInWorld => ((X1 + X2) / 2).ToMillimeterFromPixel(Definition.PixelCellSideLengthInMillimeter);
        public double YMiddleInWorld => ((Y1 + Y2) / 2).ToMillimeterFromPixel(Definition.PixelCellSideLengthInMillimeter);
    }
}