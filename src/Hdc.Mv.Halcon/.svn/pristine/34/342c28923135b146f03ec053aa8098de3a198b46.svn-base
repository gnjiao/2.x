using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class ThresholdRegionExtractor : RegionExtractorBase, IRegionExtractor
    {
        protected override HRegion ExtractInner(HImage image)
        {
            var region = image.Threshold(MinGray, MaxGray);
            return region;
        }

        public double MinGray { get; set; }

        public double MaxGray { get; set; } = 255;
    }
}