using System;
using HalconDotNet;
using Hdc.Mv.Halcon;
using System.ComponentModel;//yx

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class EmphasizeImageFilter : ImageFilterBase
    {
        [DefaultValue(7)]
        [Browsable(true)] //yx
        [Description("Width of low pass mask, Suggested values: 3, 5, 7, 9, 11, 15, 21, 25, 31, 39")]
        public int MaskWidth { get; set; } = 7;

        [DefaultValue(7)]
        [Browsable(true)] //yx
        [Description("Height of low pass mask, Suggested values: 3, 5, 7, 9, 11, 15, 21, 25, 31, 39")]
        public int MaskHeight { get; set; } = 7;

        [DefaultValue(1)]
        [Browsable(true)] //yx
        [Description("Intensity of contrast emphasis, Suggested values: 3, 5, 7, 9, 11, 15, 21, 25, 31, 39")]
        public double Factor { get; set; } = 1;

        protected override HImage ProcessInner(HImage image)
        {
            var domain = image.GetDomain();
            var domainWidth = domain.GetWidth();
            var domainHeight = domain.GetHeight();

            var maskWidth = MaskWidth == 0 ? domainWidth : MaskWidth;
            var maskHeight = MaskHeight == 0 ? domainHeight : MaskHeight;

            HImage enhancedImage = image.Emphasize(maskWidth, maskHeight, Factor);

            return enhancedImage;
        }
    }
}