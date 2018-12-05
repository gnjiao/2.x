using System;
using System.Reactive.Subjects;
using System.Threading;
using Hdc.Mv.Production;

namespace Hdc.Mv.ImageAcquisition
{
    [Serializable]
    public class SiliconFrameGrabber : IFrameGrabber
    {
        private AcquisitionCompletedCallback _acquisitionCompletedCallback;
        private AcquisitionStartedCallback _acquisitionStartedCallback;

        private readonly Subject<GrabInfo> _grabStateChangedEvent = new Subject<GrabInfo>();

        private void OnAcquisitionCompletedCallback(long msg, ImageInfo imageInfo)
        {
            Tag = (int) msg;

            var acquisitionResult = new GrabInfo()
                                    {
                                        FrameTag = (int) msg,
                                        FrameGrabberName = Name,
                                        ImageInfo = imageInfo,
                                        State = GrabState.Completed,
                                    };
            acquisitionResult.State = GrabState.Started;
            _grabStateChangedEvent.OnNext(acquisitionResult);
//            Thread.Sleep(200);
            acquisitionResult.State = GrabState.Completed;
            _grabStateChangedEvent.OnNext(acquisitionResult);
        }

        private void OnAcquisitionStartedCallback(long msg)
        {
            Tag = (int) msg;

            var acquisitionResult = new GrabInfo()
                                    {
                                        FrameTag = (int) msg,
                                        FrameGrabberName = Name,
                                        State = GrabState.Started,
                                    };
            acquisitionResult.State = GrabState.Started;
            _grabStateChangedEvent.OnNext(acquisitionResult);
//            Thread.Sleep(200);
//            acquisitionResult.State = GrabState.Completed;
//            _grabStateChangedEvent.OnNext(acquisitionResult);
        }

        public int Index { get; set; }

        public string Name { get; set; }

        public void Initialize()
        {
            _acquisitionCompletedCallback = OnAcquisitionCompletedCallback;
            _acquisitionStartedCallback = OnAcquisitionStartedCallback;

            int ret;

            ret = SiliconGrabberInterop.RegisterGrabStartCallBack(BoardIndex, CamPortIndex, _acquisitionStartedCallback);
            if (ret != 0)
                throw new Exception("SiliconGrabberInterop.RegisterGrabStartCallBack");

            ret = SiliconGrabberInterop.RegisterCallback(BoardIndex, CamPortIndex, _acquisitionCompletedCallback);
            if(ret!=0)
                throw new Exception("SiliconGrabberInterop.RegisterCallback");
        }

        public IObservable<GrabInfo> GrabStateChangedEvent
        {
            get { return _grabStateChangedEvent; }
        }

        public int BoardIndex { get; set; }

        public int CamPortIndex { get; set; }

        public void Trigger()
        {
            SiliconGrabberInterop.SendSoftwareTrigger(BoardIndex);
        }

        public void Trigger(Action beforeTriggerAction)
        {
            SiliconGrabberInterop.SendSoftwareTrigger(BoardIndex);
        }

        public int Tag { get; private set; }

        public void Trigger(ImageInfo imageInfo)
        {
            var grabInfo = new GrabInfo()
                                    {
                                        FrameGrabberName = Name,
                                        ImageInfo = imageInfo,
                                    };
            grabInfo.State = GrabState.Started;
            _grabStateChangedEvent.OnNext(grabInfo);
            grabInfo.State = GrabState.Completed;
            _grabStateChangedEvent.OnNext(grabInfo);
        }
    }
}