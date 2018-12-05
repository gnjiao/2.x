using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class CircleRegionExtractor : RegionExtractorBase, IRegionExtractor
    {
        protected override HRegion ExtractInner(HImage image)
        {
            var rect = new HRegion();
            rect.GenCircle(Y, X, Radius);
            return rect;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Radius { get; set; }
    }
}