using System;
using HalconDotNet;

namespace Hdc.Measuring
{
    [Serializable]
    public class HFramegrabberProvider :IHFramegrabberProvider
    {
        public double GrabAsyncMaxDelay { get; set; } = -1.0;

        public HFramegrabber HFramegrabber { get; set; }

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