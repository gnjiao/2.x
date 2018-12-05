using HalconDotNet;

namespace Hdc.Measuring
{
    public interface IHFramegrabberProvider
    {
        HImage GrabImage();
        void SetFramegrabberParam(string param, string value);
        void SetFramegrabberParam(HalconDotNet.HTuple param, HalconDotNet.HTuple value);

        /// <summary>
        /// in ms
        /// </summary>
        double GrabAsyncMaxDelay { get; set; }
    }
}