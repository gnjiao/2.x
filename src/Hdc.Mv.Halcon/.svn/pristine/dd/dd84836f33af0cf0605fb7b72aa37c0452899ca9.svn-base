using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class RegiongrowingRegionExtractor : RegionExtractorBase, IRegionExtractor
    {
        protected override HRegion ExtractInner(HImage image)
        {
            var region = image.Regiongrowing(Row, Column, Tolerance, MinSize);
            return region;
        }

        public int Row { get; set; } = 3;
        public int Column { get; set; } = 3;
        public double Tolerance { get; set; } = 6.0;
        public int MinSize { get; set; } = 100;
    }
}