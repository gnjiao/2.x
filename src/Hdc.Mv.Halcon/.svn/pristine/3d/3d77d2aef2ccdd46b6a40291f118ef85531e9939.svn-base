using System;
using System.Windows.Markup;
using HalconDotNet;
using Hdc.Mv.Halcon;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    [ContentProperty("RegionExtractor")]
    public class RegionToBinImageFilter : ImageFilterBase
    {
        protected override HImage ProcessInner(HImage image)
        {
            var width = image.GetWidth();
            var height = image.GetHeight();
            var region = RegionExtractor.Extract(image);
            if (region.Area < 0.01)
            {
                return image;
            }
            var image2 = region.RegionToBin(ForegroundGray, BackgroundGray, width, height);
            return image2;
        }

        public IRegionExtractor RegionExtractor { get; set; }

        public int ForegroundGray { get; set; } = 255;

        public int BackgroundGray { get; set; } = 0;
    }
}