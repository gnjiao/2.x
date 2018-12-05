using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Hdc.Measuring
{
    [Serializable]
    public class GeneralMqEventController : IEventController
    {
        private readonly Subject<MeasureEvent> _event = new Subject<MeasureEvent>();

        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            MqInitializer.Bus.SubscribeAsync<GeneralMqEvent>(SubscriptionId, x =>
            {
                return Task.Run(() =>
                {
                    if (x.EventName == EventName)
                        _event.OnNext(new MeasureEvent());
                });
            });
        }

        public IObservable<MeasureEvent> Event => _event;

        public string EventName { get; set; }

        public string SubscriptionId { get; set; }
    }
}