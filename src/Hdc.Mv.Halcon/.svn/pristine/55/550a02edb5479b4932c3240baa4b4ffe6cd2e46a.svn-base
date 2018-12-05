using System;
using System.ComponentModel;
using System.IO;
using HalconDotNet;

namespace Hdc.Mv.Halcon
{
    [Serializable]
    [Block("ReadImage", BlockCatagory.Image)]
    public class ReadImageBlock : Block
    {
        public override void Initialize()
        {
            base.Initialize();

            Image?.Dispose();
            Image = null;
        }

        public override void Process()
        {
            if (Image != null && Image.Key != IntPtr.Zero)
            {
                Status = BlockStatus.Valid;
                return;
            }

            var fn = File.Exists(FileName);

            if (!fn)
            {
                Status = BlockStatus.Error;
                Message = "FileName is not exist! " + FileName;
                Exception = new FileNotFoundException(FileName);
                return;
            }

            try
            {
                Image = new HImage();
                Image.ReadImage(FileName);
                Status = BlockStatus.Valid;
            }
            catch (Exception ex)
            {
                Status = BlockStatus.Error;
                Message = "ReadImage is not exist! " + FileName;
                Exception = ex;
                return;
            }
        }

        [InputPort]
        [Browsable(true)]
        [Category(BlockPropertyCategories.Input)]
        public string FileName { get; set; }

        [OutputPort]
        [Browsable(true)]
        [ReadOnly(true)]
        [Category(BlockPropertyCategories.Output)]
        public HImage Image { get; set; }
    }
}