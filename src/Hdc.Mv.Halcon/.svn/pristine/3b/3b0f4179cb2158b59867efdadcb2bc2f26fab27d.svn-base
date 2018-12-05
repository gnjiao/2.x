using System;
using System.ComponentModel;
using HalconDotNet;
using Hdc.Mv.Halcon;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class ConvertImageTypeImageFilter : ImageFilterBase
    {
        protected override HImage ProcessInner(HImage image)
        {
            return image.ConvertImageType(NewType.ToHalconString());
        }

        [DefaultValue(ImageType.Byte)]
        [Browsable(true)]
        [Description("Desired image type (i.e., type of the gray values).\n" +
                     "List of values: 'byte', 'complex', 'cyclic', 'direction', 'int1', 'int2', 'int4', 'int8', 'real', 'uint2'")]
        public ImageType NewType { get; set; } = ImageType.Byte;
    }
}