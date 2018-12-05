using System;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using EasyNetQ;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("StationIndexs")]
    public class MultiWorkpieceInPositionMqEventController : IEventController
    {
        private readonly Subject<MeasureEvent> _event = new Subject<MeasureEvent>();

        private bool _isInitialized;
        private readonly Guid _clientGuid = Guid.NewGuid();
        private readonly AutoResetEvent _stationCompleteEvent = new AutoResetEvent(false);        

        public void Initialize()
        {
            if (_isInitialized) return;

            _isInitialized = true;

            MqInitializer.Bus.SubscribeAsync<GeneralMqCommand>(SubscriptionId, request =>
            {
                return Task.Run(() =>
                {
                    _stationCompleteEvent.Set();
                });
            });

            MqInitializer.Bus.Subscribe<WorkpieceInPositionEvent>(SubscriptionId, x =>
            {
                Task.Run(() =>
                {
                    if (x.StationIndex != 0) return;

                    for (var index = 0; index < StationIndexs.Count; index++)
                    {
                        var stationIndex = StationIndexs[index];
                        var me = new MeasureEvent
                        {
                            StationIndex = stationIndex,
                            DateTime = DateTime.Now,
                            WorkpieceTag = x.WorkpieceTag
                        };

                        if (index == StationIndexs.Count - 1)
                            me.Message = "END";

                        _event.OnNext(me);

                        Thread.Sleep(100);

                        _stationCompleteEvent.WaitOne();
                    }
                });                                                     
            });
        }

        public IObservable<MeasureEvent> Event => _event;

        public string SubscriptionId { get; set; } = nameof(MultiWorkpieceInPositionMqEventController);

        public Collection<int> StationIndexs { get; set; } = new Collection<int>();
    }
}