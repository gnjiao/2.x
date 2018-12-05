using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class BinaryThresholdRegionExtractor : RegionExtractorBase,IRegionExtractor
    {
        protected override HRegion ExtractInner(HImage image)
        {
            int usedThreshold;
            var region = image.BinaryThreshold("max_separability", LightDark.ToHalconString(), out usedThreshold);
            return region;
        }

        public LightDark LightDark { get; set; } = LightDark.Dark;
        public string Name { get; set; }
    }
}