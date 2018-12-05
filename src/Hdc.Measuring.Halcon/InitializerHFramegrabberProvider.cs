using System;
using System.Collections.Generic;
using HalconDotNet;
using Hdc.Mv.ImageAcquisition.Halcon;

namespace Hdc.Measuring
{
    [Serializable]
    public class InitializerHFramegrabberProvider :IHFramegrabberProvider
    {
        public double GrabAsyncMaxDelay { get; set; } = -1.0;

        public HFramegrabber HFramegrabber => HalconCameraInitializer.SingletonHalconCamera.HFramegrabber;

        public HImage GrabImage()
        {
            return HFramegrabber.GrabImage();
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