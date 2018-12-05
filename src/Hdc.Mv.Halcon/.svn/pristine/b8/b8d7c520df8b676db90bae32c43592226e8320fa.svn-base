using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class FindHoleHalfCircleRegionExtractor : IRegionExtractor
    {
        public HRegion Extract(HImage image)
        {
            var foundRegion = image.FindHoleHalfCircleRegion(LeftOrRight, HasPre,
                HoughCircleThresholdGrayMin, HoughCircleThresholdGrayMax, HoughCircleSelectAreaMin,
                HoughCircleSelectAreaMax
                , HoughCircleRadius, HoughCirclePercent, MaxLineWidth);
            return foundRegion;
        }

        public bool LeftOrRight { get; set; }
        public bool HasPre { get; set; }
        public int HoughCircleThresholdGrayMin { get; set; }
        public int HoughCircleThresholdGrayMax { get; set; }
        public int HoughCircleSelectAreaMin { get; set; }
        public int HoughCircleSelectAreaMax { get; set; }
        public int HoughCircleRadius { get; set; }
        public int HoughCirclePercent { get; set; }
        public double MaxLineWidth { get; set; }
    }
}