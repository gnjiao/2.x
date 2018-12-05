using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Hdc.Mercury;

namespace Hdc.Measuring
{
    [Serializable]
    public class OpcBooleanEventController : IEventController
    {
        private readonly Subject<MeasureEvent> _subject = new Subject<MeasureEvent>();

        public OpcBooleanEventController()
        {
        }

        public OpcBooleanEventController(string deviceName)
        {
            DeviceName = deviceName;
        }

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _device = OpcInitializer.DeviceGroupProvider.RootDeviceGroup
                .GetDevice<bool>(DeviceName);

            _device
                .Delay(TimeSpan.FromMilliseconds(Delay))
                .Subscribe(x =>
            {
                if (!x)
                    return;

                Console.WriteLine("OpcEventSignalController.Event Raised: " + DeviceName + ", begin at " + DateTime.Now);
                _subject.OnNext(new MeasureEvent());
                Console.WriteLine("OpcEventSignalController.Event Raised: " + DeviceName + ", end at " + DateTime.Now);
            });
        }

        private IDevice<bool> _device;

        private bool _isInitialized;

        public string DeviceName { get; set; }

        public int Delay { get; set; } // In Milliseconds

        public IObservable<MeasureEvent> Event => _subject;

        public void Acknowledge()
        {
            Console.WriteLine("OpcEventSignalController.Acknowledge(): " + DeviceName + ", begin at " + DateTime.Now);
            _device.WriteFalse();
            Console.WriteLine("OpcEventSignalController.Acknowledge(): " + DeviceName + ", end at " + DateTime.Now);
        }
    }
}