using System;
using System.Reactive.Subjects;
using Hdc.Mv;
using Hdc.Mv.ImageAcquisition;

namespace Hdc.Mv.ImageAcquisition
{
    public class SinglePortSiliconFrameGrabberWorker
    {
        private static AcquisitionCompletedCallback _acquisitionCompletedCallback;
        private readonly Subject<Payload> _acquisitionCompletedEvent = new Subject<Payload>();

        public SinglePortSiliconFrameGrabberWorker()
        {
        }

        public void Initialize()
        {
            _acquisitionCompletedCallback = OnAcquisitionCompletedCallback;
            int ret = SiliconGrabberInterop.RegisterCallback(0, 0, _acquisitionCompletedCallback);
            if (ret != 0)
                throw new Exception("SiliconGrabberInterop.RegisterCallback");
        }

        public Subject<Payload> AcquisitionCompletedEvent
        {
            get { return _acquisitionCompletedEvent; }
        }

        private void OnAcquisitionCompletedCallback(long msg, ImageInfo imageInfo)
        {
            _acquisitionCompletedEvent.OnNext(new Payload()
            {
                Msg = msg,
                ImageInfo = imageInfo,
            });
        }
    }

    public class Payload
    {
        public long Msg { get; set; }
        public ImageInfo ImageInfo { get; set; }
    }

    [Serializable]
    public class SinglePortSiliconFrameGrabber : IFrameGrabber
    {
        private static SinglePortSiliconFrameGrabberWorker _worker;

        private static SinglePortSiliconFrameGrabberWorker Worker
        {
            get
            {
                if (_worker != null) return _worker;

                _worker = new SinglePortSiliconFrameGrabberWorker();
                _worker.Initialize();
                return _worker;
            }
        }

        private readonly Subject<GrabInfo> _grabStateChangedEvent = new Subject<GrabInfo>();


        public int Index { get; set; }

        public string Name { get; set; }

        public int Phase { get; set; }

        public int PhaseCount { get; set; }

        public void Initialize()
        {
            Worker.AcquisitionCompletedEvent.Subscribe(
                x =>
                {
                    Tag = (int)x.Msg;
                    var frameTag = Tag;

                    if (PhaseCount > 0)
                    {
                        var mod = Tag % PhaseCount;
                        if (mod != Phase)
                            return;

                        frameTag = Tag / PhaseCount;
                    }

                    var acquisitionResult = new GrabInfo()
                    {
                        FrameTag = frameTag,
                        FrameGrabberName = Name,
                        ImageInfo = x.ImageInfo,
                        State = GrabState.Completed,
                    };
                    acquisitionResult.State = GrabState.Started;
                    _grabStateChangedEvent.OnNext(acquisitionResult);
                    //            Thread.Sleep(200);
                    acquisitionResult.State = GrabState.Completed;
                    _grabStateChangedEvent.OnNext(acquisitionResult);
                });

        }

        public IObservable<GrabInfo> GrabStateChangedEvent
        {
            get { return _grabStateChangedEvent; }
        }

        public void Trigger()
        {
            SiliconGrabberInterop.SendSoftwareTrigger(0);
        }

        public void Trigger(Action beforeTriggerAction)
        {
            beforeTriggerAction();
            SiliconGrabberInterop.SendSoftwareTrigger(0);
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