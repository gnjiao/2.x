using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Hdc.Measuring
{
    [Serializable]
    public class WorkpieceInPositionMqEventController : IEventController
    {
        private readonly Subject<MeasureEvent> _event = new Subject<MeasureEvent>();

        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            MqInitializer.Bus.SubscribeAsync<WorkpieceInPositionEvent>(SubscriptionId, x =>
            {
                return Task.Run(() =>
                {
                    if (StationIndex >= 0)
                    {
                        if (x.StationIndex != StationIndex) return;
                    }

                    var me = new MeasureEvent()
                    {
                        StationIndex = x.StationIndex,
                        DateTime = x.DateTime,
                        WorkpieceTag = x.WorkpieceTag,
                    };
                    _event.OnNext(me);
                });
            });
        }

        public IObservable<MeasureEvent> Event => _event;

        public int StationIndex { get; set; }

        public string SubscriptionId { get; set; }
    }
}