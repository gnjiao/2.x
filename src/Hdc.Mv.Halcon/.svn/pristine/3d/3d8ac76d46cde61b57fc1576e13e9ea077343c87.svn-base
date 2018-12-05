using System;
using HalconDotNet;
using System.ComponentModel;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class MinMaxGrayScaleFilter : ImageFilterBase
    {
        protected override HImage ProcessInner(HImage image)
        {
            double min;
            double max;
            double range;
            var domain = image.GetDomain();
            image.MinMaxGray(domain, Percent, out min, out max, out range);

            if (Math.Abs(max - min) < 0.000001)
                return image.ScaleImage(1.0, 0.0);

            var mult = 255 / (max - min);
            var add = -mult * min;
            var scaledImage = image.ScaleImage(mult, add);

            return scaledImage;
        }

        [DefaultValue(0)]
        [Browsable(true)]
        [Description("Percentage below (above) the absolute maximum (minimum), " +
            "Suggested values: 0, 1, 2, 5, 7, 10, 15, 20, 30, 40, 50")]
        public double Percent { get; set; } = 0;
    }
}