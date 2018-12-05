using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class HysteresisThresholdRegionExtractor : RegionExtractorBase, IRegionExtractor
    {
        protected override HRegion ExtractInner(HImage image)
        {
            var region = image.HysteresisThreshold(Low, High, MaxLength);
            return region;
        }

        public int Low { get; set; } = 30;

        public int High { get; set; } = 60;

        public int MaxLength { get; set; } = 10;
    }
}