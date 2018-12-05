using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class AutoThresholdRegionExtractor : RegionExtractorBase, IRegionExtractor
    {
        protected override HRegion ExtractInner(HImage image)
        {
            var region = image.AutoThreshold(Sigma);
            return region;
        }

        public double Sigma { get; set; }
    }
}