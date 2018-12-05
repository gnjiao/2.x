using System;
using HalconDotNet;
using System.ComponentModel;//yx

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class GrayRangeRectImageFilter : ImageFilterBase
    {
        protected override HImage ProcessInner(HImage image)
        {
            return image.GrayRangeRect(MaskHeight, MaskWidth);
        }

        [DefaultValue(11)]
        [Browsable(true)] //yx
        [Description("Height of the filter mask, Suggested values: 3, 5, 7, 9, 11, 13, 15")]
        public int MaskHeight { get; set; } = 11;

        [DefaultValue(11)]
        [Browsable(true)] //yx
        [Description("Height of the filter mask, Suggested values: 3, 5, 7, 9, 11, 13, 15")]
        public int MaskWidth { get; set; } = 11;
    }
}