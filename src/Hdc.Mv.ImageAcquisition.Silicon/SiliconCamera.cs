using System;
using System.Threading;

namespace Hdc.Mv.ImageAcquisition
{
    [Serializable]
    public class SiliconCamera : ICamera
    {
        private AcquisitionCompletedCallback _acquisitionCompletedCallback;

        private ImageInfo _imageInfo;
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        public void Dispose()
        {
        }

        public bool Init()
        {
            _acquisitionCompletedCallback = OnAcquisitionCompletedCallback;

            var ret = SiliconGrabberInterop.RegisterCallback(BoardIndex, CamPortIndex, _acquisitionCompletedCallback);
            return ret == 0;
        }

        private void OnAcquisitionCompletedCallback(long msg, ImageInfo imageInfo)
        {
            acquisitionCallbackCounter++;
            Console.WriteLine($"SiliconCamera.Callback(): Tag={Tag}, CallbackCounter={acquisitionCallbackCounter}, begin at " + DateTime.Now);

            Tag = (int) msg;


            if (!AcquisitionWorking)
            {
                Console.WriteLine($"SiliconCamera.Callback(): !!!Error: AcquisitionWorking=false, Tag={Tag}, CallbackCounter={acquisitionCallbackCounter}, begin at " + DateTime.Now);
                return;
            }

            _imageInfo = imageInfo;
            _autoResetEvent.Set();

            Console.WriteLine("SiliconCamera.Callback(): end at " + DateTime.Now);
        }

        private int acquisitionCounter;
        private int acquisitionCallbackCounter;

        private bool AcquisitionWorking = false;

        public ImageInfo Acquisition()
        {
            AcquisitionWorking = true;

            acquisitionCounter++;
            Console.WriteLine($"SiliconCamera.Acquisition(): WaitOne, acquisitionCounter={acquisitionCounter}, begin at " + DateTime.Now);
            var ret = _autoResetEvent.WaitOne(10000);

            if (!ret)
            {
                Console.WriteLine($"SiliconCamera.Acquisition(): !!! WaitOne timeout, at " + DateTime.Now);
                return new ImageInfo();
            }

            AcquisitionWorking = false;
            Console.WriteLine($"SiliconCamera.Acquisition(): end at " + DateTime.Now);
            return _imageInfo;
        }

        public int BoardIndex { get; set; }

        public int CamPortIndex { get; set; }

        public int Tag { get; private set; }
    }
}