using System;
using HalconDotNet;

namespace Hdc.Mv.ImageAcquisition.Halcon
{
    public static class HFramegrabberEx
    {
         public static void SetFramegrabberParam(this HFramegrabber framegrabber,  string name, string value, FrameGrabberParamEntryDataType dataType)
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
                    $"{nameof(HFramegrabberEx)}: DataType is unknown");
            }
        }
    }
}