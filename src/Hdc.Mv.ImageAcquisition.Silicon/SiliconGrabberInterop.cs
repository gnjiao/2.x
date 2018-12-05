using System.Runtime.InteropServices;

namespace Hdc.Mv.ImageAcquisition
{
    public static class SiliconGrabberInterop
    {
        [DllImport(@"SiliconGrabber.dll")]
        public static extern int Initialize(int boardCount, GrabberInfo[] grabberInfos);

        [DllImport(@"SiliconGrabber.dll")]
        public static extern int RegisterCallback(int boardIndex, int camPortIndex, AcquisitionCompletedCallback callback);

        [DllImport(@"SiliconGrabber.dll")]
        public static extern int SendSoftwareTrigger(int boardIndex);

        [DllImport(@"SiliconGrabber.dll")]
        public static extern int Uninitialize();

        [DllImport(@"SiliconGrabber.dll")]
        public static extern int RegisterGrabStartCallBack(int boardIndex, int camPortIndex, AcquisitionStartedCallback callback);
    }
}