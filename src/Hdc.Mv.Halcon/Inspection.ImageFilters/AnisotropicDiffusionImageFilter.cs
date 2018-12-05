using System;
using HalconDotNet;
using System.ComponentModel;//yx

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class AnisotropicDiffusionImageFilter: ImageFilterBase
    {
        protected override HImage ProcessInner(HImage image)
        {
            string modeString = null;
            switch (Mode)
            {
                    case AnisotropicDiffusionMode.Weickert:
                    modeString = "weickert";
                    break;
                    case AnisotropicDiffusionMode.Parabolic:
                    modeString = "parabolic";
                    break;
                    case AnisotropicDiffusionMode.PeronaMalik:
                    modeString = "perona-malik";
                    break;
            }

            HImage enhancedImage = image.AnisotropicDiffusion(modeString, Contrast,Theta, Iterations);
            return enhancedImage;
        }

        [DefaultValue("weickert")]
        [Browsable(true)] //yx
        [Description("Diffusion coefficient as a function of the edge amplitude")]
        public AnisotropicDiffusionMode Mode { get; set; } = AnisotropicDiffusionMode.Weickert;//

        [DefaultValue(5)]
        [Browsable(true)] //yx
        [Description("Contrast parameter")]
        public double Contrast { get; set; } = 5.0;

        [DefaultValue(1)]
        [Browsable(true)] //yx
        [Description("Time step")]
        public double Theta { get; set; } = 1.0;

        [DefaultValue(10)]
        [Browsable(true)] //yx
        [Description("Number of iterations")]
        public int Iterations { get; set; } = 10;
    }
}