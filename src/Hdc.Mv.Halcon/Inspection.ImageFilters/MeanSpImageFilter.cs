using System;
using HalconDotNet;
using Hdc.Mv.Halcon;
using System.ComponentModel;//yx

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class MeanSpImageFilter : IImageFilter
    {
        public HImage Process(HImage image)
        {
            var maskWidth = MaskWidth;
            var maskHeight = MaskHeight;

            HImage enhancedImage = image.MeanSp(maskHeight, maskWidth, MinThresh, MaxThresh);

            return enhancedImage;
        }

        [DefaultValue(3)]
        [Browsable(true)]
        [Description("Width of filter mask")]
        public int MaskWidth { get; set; } = 3;//yx

        [DefaultValue(3)]
        [Browsable(true)]
        [Description("Height of filter mask")]
        public int MaskHeight { get; set; } = 3;//yx

        [DefaultValue(1)]
        [Browsable(true)]
        [Description("Minimum gray value")]
        public int MinThresh { get; set; } = 1;//yx

        [DefaultValue(254)]
        [Browsable(true)]
        [Description("Maximum gray value")]
        public int MaxThresh { get; set; } = 254;//yx
    }
}