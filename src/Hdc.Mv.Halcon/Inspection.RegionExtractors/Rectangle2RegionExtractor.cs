﻿using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class Rectangle2RegionExtractor : RegionExtractorBase, IRegionExtractor, IRectangle2
    {
        protected override HRegion ExtractInner(HImage image)
        {
            var region = this.GenRegion();

            return region;
        }

//        public Rectangle2 Rectangle2 { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }
        public double HalfWidth { get; set; }
        public double HalfHeight { get; set; }
    }
}