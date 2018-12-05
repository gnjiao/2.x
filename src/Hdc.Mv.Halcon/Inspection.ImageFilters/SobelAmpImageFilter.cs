using System;
using HalconDotNet;
using Hdc.Mv.Halcon;
using System.ComponentModel;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class SobelAmpImageFilter : ImageFilterBase
    {
        protected override HImage ProcessInner(HImage image)
        {
            return image.SobelAmp(FilterType.ToHalconString(), Size);
        }

        [DefaultValue(3)]
        [Browsable(true)]
        [Description(
            "Size of filter mask, List of values: 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27, 29, 31, 33, 35, 37, 39"
            )]
        public int Size { get; set; } = 3;

        [DefaultValue("sum_abs")]
        [Browsable(true)]
        [Description("Filter type, List of values: 'sum_abs', 'sum_abs_binomial', 'sum_sqrt', 'sum_sqrt_binomial', 'thin_max_abs', 'thin_max_abs_binomial', 'thin_sum_abs', 'thin_sum_abs_binomial', 'x', 'x_binomial', 'y', 'y_binomial'")]
        public SobelAmpFilterType FilterType { get; set; }=SobelAmpFilterType.SumAbs;
    }
}