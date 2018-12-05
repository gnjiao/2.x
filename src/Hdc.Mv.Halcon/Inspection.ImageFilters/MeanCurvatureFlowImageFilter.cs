using System;
using System.ComponentModel;
using HalconDotNet;
using Hdc.Mv.Halcon;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class MeanCurvatureFlowImageFilter : ImageFilterBase
    {
        protected override HImage ProcessInner(HImage image)
        {
            HImage enhancedImage = image.MeanCurvatureFlow(Sigma, Theta, Iterations);

            return enhancedImage;
        }

        [DefaultValue(0.5)]
        [Browsable(true)]
        [Description("Smoothing parameter for derivative operator, Suggested values: 0.0, 0.1, 0.5, 1.0, Restriction: Sigma >= 0")]
        public double Sigma { get; set; } = 0.5;

        [DefaultValue(0.5)]
        [Browsable(true)]
        [Description("Time step, Suggested values: 0.1, 0.2, 0.3, 0.4, 0.5, Restriction: (0 < Theta) <= 0.5")]
        public double Theta { get; set; } = 0.5;

        [DefaultValue(10)]
        [Browsable(true)]
        [Description("Number of iterations, Suggested values: 1, 5, 10, 20, 50, 100, 500, Restriction: Iterations >= 1")]
        public int Iterations { get; set; } = 10;
    }
}