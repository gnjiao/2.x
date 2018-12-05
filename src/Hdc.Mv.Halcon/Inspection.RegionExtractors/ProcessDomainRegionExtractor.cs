﻿using System;
using System.Windows.Markup;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    [ContentProperty("RegionProcessor")]
    public class ProcessDomainRegionExtractor : RegionExtractorBase, IRegionExtractor
    {
        protected override HRegion ExtractInner(HImage image)
        {
            var region = image.GetDomain();
            var modDomain = RegionProcessor.Process(region);
            region.Dispose();
            return modDomain;
        }

        public IRegionProcessor RegionProcessor { get; set; }
    }
}