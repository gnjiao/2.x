using System;
using System.Windows.Markup;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    [ContentProperty("RegionExtractor")]
    public class PaintRegionImageFilter : ImageFilterBase
    {
        protected override HImage ProcessInner(HImage image)
        {
            var region = RegionExtractor.Extract(image);
            if (region.Area <= 0.01)
            {
                return image;
            }
            var paintedImage = image.PaintRegion(region, (double) GrayValue, Type);
            return paintedImage;
        }

        public int GrayValue { get; set; } = 255;

        public string Type { get; set; } = "fill";

        public IRegionExtractor RegionExtractor { get; set; }
    }
}