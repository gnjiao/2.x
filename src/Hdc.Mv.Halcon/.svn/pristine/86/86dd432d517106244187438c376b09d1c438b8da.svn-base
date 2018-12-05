using System;
using HalconDotNet;
using Hdc.Mv.Halcon;
using System.ComponentModel;//yx

//Creat by YanXin

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class DomainToBinImageFilter : ImageFilterBase
    {
        protected override HImage ProcessInner(HImage image)
        {
            var width = image.GetWidth();
            var height = image.GetWidth();
            HRegion domain = image.GetDomain();
            var image2 = domain.RegionToBin(ForegroundGray, BackgroundGray, width, height);
            return image2;
        }

        //public IRegionExtractor RegionExtractor { get; set; }

        [DefaultValue(255)]
        [Browsable(true)] //yx
        [Description("Gray value in which the regions are displayed, Suggested values: 0, 1, 50, 100, 128, 150, 200, 254, 255")]
        public int ForegroundGray { get; set; } = 255;

        [DefaultValue(0)]
        [Browsable(true)] //yx
        [Description("Gray value in which the background is displayed, Suggested values: 0, 1, 50, 100, 128, 150, 200, 254, 255")]
        public int BackgroundGray { get; set; } = 0;
    }
}