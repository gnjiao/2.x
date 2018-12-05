using System;
using System.Collections.Generic;
using HalconDotNet;
using Hdc.Mv.ImageAcquisition.Halcon;

namespace Hdc.Measuring
{
    [Serializable]
    public class MultipleInitializerHFramegrabberProvider : IHFramegrabberProvider
    {
        public double GrabAsyncMaxDelay { get; set; } = -1.0;

        public int Index { get; set; }

        public HFramegrabber HFramegrabber => MultipleHalconCameraInitializer.Singleton.HalconCamerals[Index].HFramegrabber;

        public HImage GrabImage()
        {
            //HFramegrabber.GrabImageStart(GrabAsyncMaxDelay);
            return HFramegrabber.GrabImageAsync(GrabAsyncMaxDelay);
        }

        public void SetFramegrabberParam(string param, string value)
        {
            HFramegrabber.SetFramegrabberParam(param, value);
        }

        public void SetFramegrabberParam(HTuple param, HTuple value)
        {
            HFramegrabber.SetFramegrabberParam(param, value);
        }
    }
}