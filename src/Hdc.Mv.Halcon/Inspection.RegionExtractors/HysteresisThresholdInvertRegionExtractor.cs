using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public  class HysteresisThresholdInvertRegionExtractor : RegionExtractorBase, IRegionExtractor
    {
        protected override HRegion ExtractInner(HImage image)
        {
            var invertImage = image.InvertImage();
            var region = invertImage.HysteresisThreshold(255- Low, 255 - High, MaxLength);
            invertImage.Dispose();
            return region;
        }

        public int Low { get; set; } = 60;

        public int High { get; set; } = 30;

        public int MaxLength { get; set; } = 10;
    }
}