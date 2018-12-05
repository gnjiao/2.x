using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class HighpassImageFilter : ImageFilterBase
    {
        protected override HImage ProcessInner(HImage image)
        {
            var enhancedImage = image.HighpassImage(Width, Height);

            return enhancedImage;
        }

        public int Width { get; set; } = 9;

        public int Height { get; set; } = 9;
    }
}