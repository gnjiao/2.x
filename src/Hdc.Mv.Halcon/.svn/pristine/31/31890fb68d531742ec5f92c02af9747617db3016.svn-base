using System;
using System.ComponentModel;
using HalconDotNet;
using Hdc.Mv.Halcon;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class MeanImageFilter : IImageFilter
    {
        private int counter;
        public HImage Process(HImage image)
        {
            var domain = image.GetDomain();


            var domainCount = domain.CountObj();

            int maskHeight;
            int maskWidth;
            int imageWidth;
            int imageHeight;

            if (domainCount == 0)
            {
                maskWidth = MaskWidth;
                maskHeight = MaskHeight;
            }
            else
            {
                imageWidth = image.GetWidth();
                imageHeight = image.GetHeight();

                maskWidth = MaskWidth == 0 ? imageWidth : MaskWidth;
                maskHeight = MaskHeight == 0 ? imageHeight : MaskHeight;

                if (maskWidth > (imageWidth * 2)) maskWidth = imageWidth * 2 - 1;
                if (maskHeight > (imageHeight * 2)) maskHeight = imageHeight * 2 - 1;
            }

            //            var swMeanImage = new NotifyStopwatch("MeanFilter.MeanImage");
            HImage enhancedImage = image.MeanImage(maskWidth, maskHeight);
            //            swMeanImage.Dispose();

            return enhancedImage;
        }

        [DefaultValue(9)]
        [Browsable(true)] //yx
        [Description("Width of filter mask")]
        public int MaskWidth { get; set; } = 9;

        [DefaultValue(9)]
        [Browsable(true)] //yx
        [Description("Width of filter mask")]
        public int MaskHeight { get; set; } = 9;

        public MeanImageFilter()
        {
        }

        public MeanImageFilter(int maskWidth, int maskHeight)
        {
            MaskWidth = maskWidth;
            MaskHeight = maskHeight;
        }
    }
}