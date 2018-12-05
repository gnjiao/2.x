using System;
using System.ComponentModel;
using HalconDotNet;
using Hdc.Mv.Halcon;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class MedianImageFilter : ImageFilterBase
    {
        protected override HImage ProcessInner(HImage image)
        {
            HImage enhancedImage = image.MedianImage(
                MaskType.ToHalconString(),
                Radius,
                Margin.ToHalconString());

            return enhancedImage;
        }

        [DefaultValue(MedianMaskType.Circle)]
        [Browsable(true)]
        [Description("Filter mask type, List of values: 'circle', 'square'")]
        public MedianMaskType MaskType { get; set; } = MedianMaskType.Circle;

        [DefaultValue(1)]
        [Browsable(true)]
        [Description("Radius of the filter mask, Suggested values: 1, 2")]
        public int Radius { get; set; } = 1;

        [DefaultValue(MedianMargin.Mirrored)]
        [Browsable(true)]
        [Description("Border treatment, Suggested values: 'mirrored', 'cyclic', 'continued', 0, 30, 60, 90, 120, 150, 180, 210, 240, 255")]
        public MedianMargin Margin { get; set; } = MedianMargin.Mirrored;
    }
}