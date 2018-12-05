using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Hdc.Mercury;

namespace Hdc.Measuring
{
    [Serializable]
    public class OpcWorkpieceInPositionEventController : IEventController
    {
        private readonly Subject<MeasureEvent> _subject = new Subject<MeasureEvent>();

        public OpcWorkpieceInPositionEventController()
        {
        }

        public OpcWorkpieceInPositionEventController(string deviceName)
        {
            DeviceName = deviceName;
        }

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _device = OpcInitializer.DeviceGroupProvider.RootDeviceGroup
                .GetDevice<UInt32>(DeviceName);

            _device
                .Delay(TimeSpan.FromMilliseconds(Delay))
                .Subscribe(x =>
                {
                    if (x == 0) return;

                    var wpcTag = x & Convert.ToUInt32("01111111111111111111111111111111", 2);
                    var measureRequestInfo = new MeasureEvent
                    {
                        WorkpieceTag = Convert.ToInt32(wpcTag),
                        Message = "typeof(Data) is MeasureRequestInfo, WorkpieceTag:" + wpcTag,
                        DateTime = DateTime.Now,
                    };

                    Console.WriteLine($"OpcUInt32EventController.Event Raised: {DeviceName}, begin at {DateTime.Now}");
                    _subject.OnNext(measureRequestInfo);
                    Console.WriteLine($"OpcUInt32EventController.Event Raised: {DeviceName}, end at {DateTime.Now}");
                });
        }

        private IDevice<UInt32> _device;

        private bool _isInitialized;

        public string DeviceName { get; set; }

        public int Delay { get; set; } // In Milliseconds

        public IObservable<MeasureEvent> Event => _subject;
    }
}