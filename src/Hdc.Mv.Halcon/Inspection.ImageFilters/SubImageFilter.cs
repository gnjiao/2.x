using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class SubImageFilter : ImageFilterBase
    {
        public IImageFilter SubtrahendImageFilter { get; set; }

        public double Mult { get; set; }

        public double Add { get; set; }

        protected override HImage ProcessInner(HImage image)
        {
            var subtrahendImage = SubtrahendImageFilter.Process(image);

            HImage subImage = image.SubImage(subtrahendImage, Mult, Add);

            subtrahendImage.Dispose();

            return subImage;
        }
    }
}