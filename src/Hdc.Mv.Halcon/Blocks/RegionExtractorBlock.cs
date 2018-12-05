using System;
using System.Windows.Markup;
using HalconDotNet;
using Hdc.Mv.Inspection;

namespace Hdc.Mv.Halcon
{
    [Serializable]
    [Block("RegionExtractor", BlockCatagory.RegionExtractor)]
    [ContentProperty("RegionExtractor")]
    public class RegionExtractorBlock: Block
    {
        public override void Process()
        {
            if (Image == null)
            {
                Status = BlockStatus.Error;
                Message = "Image is null.";
                Exception = new BlockException("Image is null.");
                return;
            }

            if (RegionExtractor == null)
            {
                Status = BlockStatus.Error;
                Message = "RegionExtractor is null.";
                Exception = new BlockException("RegionExtractor is null.");
                return;
            }

            try
            {
                Region = RegionExtractor.Extract(Image);
                Status = BlockStatus.Valid;
            }
            catch (Exception ex)
            {
                Status = BlockStatus.Error;
                Message = "RegionExtractorBlock Error! RegionExtractor.Extract() throw exception.";
                Exception = ex;
            }
        }

        [InputPort]
        public HImage Image { get; set; }

        [OutputPort]
        public HRegion Region { get; set; }

        public IRegionExtractor RegionExtractor { get; set; }
    }
}