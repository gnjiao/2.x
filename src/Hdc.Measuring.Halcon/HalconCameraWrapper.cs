using System;
using Hdc.Mv;
using Hdc.Mv.ImageAcquisition;

namespace Hdc.Measuring
{
    [Serializable]
    public class HalconCameraWrapper: ICamera
    {
        public void Dispose()
        {
            
        }

        public bool Init()
        {
            return true;
        }

        public ImageInfo Acquisition()
        {
            return HalconCameraInitializer.SingletonHalconCamera.Acquisition();
        }
    }
}