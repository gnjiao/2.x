using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class SubImage2Filter : ImageFilterBase
    {
        public IImageFilter MinuendImageFilter { get; set; }

        public IImageFilter SubtrahendImageFilter { get; set; }

        public double Mult { get; set; }

        public double Add { get; set; }

        protected override HImage ProcessInner(HImage image)
        {
            var minuendImage = MinuendImageFilter.Process(image);
            var subtrahendImage = SubtrahendImageFilter.Process(image);

            HImage subImage = minuendImage.SubImage(subtrahendImage, Mult, Add);

            minuendImage.Dispose();
            subtrahendImage.Dispose();

            return subImage;
        }
    }
}