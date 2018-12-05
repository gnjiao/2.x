using System.Runtime.InteropServices;

namespace Hdc.Mv.ImageAcquisition
{
    public static class SiliconInteropApi
    {
        public class GrabberInfo
        {
            
        }

        [DllImport(@"SiliconGrabber.dll")]
        public static extern int InitGrabDevice();

        [DllImport(@"SiliconGrabber.dll")]
        public static extern ImageInfo GrabSingleFrame();

        [DllImport(@"SiliconGrabber.dll")]
        public static extern ImageInfo GrabSingleFrameFromFile([MarshalAs(UnmanagedType.LPTStr)] string fileName);

        [DllImport(@"SiliconGrabber.dll")]
        public static extern void FreeDevice();
    }
}