using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using HalconDotNet;
using Hdc.Mv.Inspection;

namespace Hdc.Mv.Halcon
{
    [Serializable]
    [Block("ImageFilter", BlockCatagory.ImageFilter)]
    public class ImageFilterBlock : Block
    {
        public override void Process()
        {
            if (InputImage == null)
            {
                Status = BlockStatus.Error;
                Message = "InputImage is null";
                Exception = new BlockException("InputImage is null");
                return;
            }

            var image = InputImage;

            Images.Clear();
            Images.Add(InputImage);

            foreach (var imageFilter in ImageFilters)
            {
                var hImage = imageFilter.Process(image);
                Images.Add(hImage);

                image = hImage;
            }

            OutputImage = image;

            Status = BlockStatus.Valid;
            Message = "Process OK";
        }

        [Browsable(false)]
        public Collection<HImage> Images { get; set; } = new BindingList<HImage>();

        [Browsable(true)]
        [Category(BlockPropertyCategories.Parameter)]
        public Collection<IImageFilter> ImageFilters { get; set; } = new BindingList<IImageFilter>();

        [InputPort]
        [Browsable(true)]
        [Category(BlockPropertyCategories.Input)]
        public HImage InputImage { get; set; }

        [OutputPort]
        [Browsable(true)]
        [Category(BlockPropertyCategories.Output)]
        public HImage OutputImage { get; set; }
    }
}