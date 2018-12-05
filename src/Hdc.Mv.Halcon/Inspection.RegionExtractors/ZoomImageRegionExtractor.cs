using System;
using System.Windows.Markup;
using HalconDotNet;
using Hdc.Mv.Halcon;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    [ContentProperty("RegionExtractor")]
    public class ZoomImageRegionExtractor : RegionExtractorBase, IRegionExtractor
    {
        protected override HRegion ExtractInner(HImage image)
        {
            HImage zoomedImage = image.ZoomImageFactor(ScaleWidth, ScaleHeight, Interpolation.ToHalconString());
            HRegion zoomedRegion = RegionExtractor.Extract(zoomedImage);

            var oriRegion = zoomedRegion.ZoomRegion(1 / ScaleWidth, 1 / ScaleHeight);

            zoomedImage.Dispose();
            zoomedRegion.Dispose();

            return oriRegion;
        }

        public double ScaleWidth { get; set; } = 0.5;

        public double ScaleHeight { get; set; } = 0.5;

        public Interpolation Interpolation { get; set; } = Interpolation.Constant;

        public IRegionExtractor RegionExtractor { get; set; }
    }
}