using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class JaiCameraWrapperInitializer : IInitializer
    {
        public void Initialize()
        {
            Console.WriteLine("JaiCameraWrapperInitializer.Initialize() begin at: " + DateTime.Now);
            JaiCameraWrapper.JaiCamera.Init();
            Console.WriteLine("JaiCameraWrapperInitializer.Initialize() end at: " + DateTime.Now);
        }
    }
}