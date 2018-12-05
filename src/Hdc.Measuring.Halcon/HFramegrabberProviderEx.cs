using System;
using Hdc.Mv.ImageAcquisition.Halcon;

namespace Hdc.Measuring
{
    public static class HFramegrabberProviderEx
    {
        public static void SetFramegrabberParam(this IHFramegrabberProvider framegrabber, string name, string value,
            FrameGrabberParamEntryDataType dataType)
        {
            switch (dataType)
            {
                case FrameGrabberParamEntryDataType.String:
                    framegrabber.SetFramegrabberParam(name, value);
                    break;
                case FrameGrabberParamEntryDataType.Int32:
                    var intValue = Int32.Parse(value);
                    framegrabber.SetFramegrabberParam(name, intValue);
                    break;
                case FrameGrabberParamEntryDataType.Double:
                    var doubleValue = Double.Parse(value);
                    framegrabber.SetFramegrabberParam(name, doubleValue);
                    break;
                default:
                    throw new InvalidOperationException(
                    $"{nameof(HFramegrabberProviderEx)}: DataType is unknown");
            }
        }
    }
}