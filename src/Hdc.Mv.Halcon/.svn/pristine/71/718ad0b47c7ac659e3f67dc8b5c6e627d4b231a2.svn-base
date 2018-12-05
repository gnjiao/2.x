using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class EdgesImageImageFilter: ImageFilterBase
    {
        protected override HImage ProcessInner(HImage image)
        {
            HImage imgDir;

            return image.EdgesImage(out imgDir, Filter, Alpha, NMS, Low, High);
        }

        /// <summary>
        /// List of values: 'canny', 'deriche1', 'deriche1_int4', 'deriche2', 'deriche2_int4', 'lanser1', 'lanser2', 'mshen', 'shen', 'sobel_fast'
        /// List of values (for compute devices): 'canny', 'sobel_fast'
        /// </summary>
        public string Filter { get; set; } = "canny";

        public double Alpha { get; set; } = 1.0;

        /// <summary>
        /// List of values: 'hvnms', 'inms', 'nms', 'none'
        /// </summary>
        public string NMS { get; set; } = "nms";

        /// <summary>
        /// Typical range of values: 1 ≤ Low ≤ 255
        /// </summary>
        public int Low { get; set; } = 20;

        /// <summary>
        /// Typical range of values: 1 ≤ High ≤ 255
        /// </summary>
        public int High { get; set; } = 40;
    }
}