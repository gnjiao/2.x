using System;
using Hdc.Mv;
using Hdc.Mv.ImageAcquisition;

namespace Hdc.Measuring
{
    [Serializable]
    public class JaiCameraWrapper : ICamera
    {
        public static JaiCamera JaiCamera = new JaiCamera();

        public void Dispose()
        {
            JaiCamera.Dispose();
        }

        private static bool _isInitialized;

        public bool Init()
        {
            if (_isInitialized)
                return false;

            _isInitialized = true;

            JaiCamera.Init();

            return true;
        }

        public ImageInfo Acquisition()
        {
            return JaiCamera.Acquisition();
        }
    }
}