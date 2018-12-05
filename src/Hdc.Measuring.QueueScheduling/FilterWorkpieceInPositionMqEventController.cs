using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using EasyNetQ;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("StationIndexs")]
    public class FilterWorkpieceInPositionMqEventController : IEventController
    {
        private readonly Subject<MeasureEvent> _event = new Subject<MeasureEvent>();

        private bool _isInitialized;
        private readonly Guid _clientGuid = Guid.NewGuid();
        public void Initialize()
        {
            if (_isInitialized) return;

            _isInitialized = true;

            MqInitializer.Bus.Subscribe<WorkpieceInPositionEvent>(SubscriptionId, x =>
            {
                Task.Run(() =>
                {
                    if (x.StationIndex == 0) return;

                    for (var i = 0; i < StationIndexs.Count; i++)
                    {
                        if (StationIndexs[i] == x.StationIndex)
                        {
                            var me = new MeasureEvent
                            {
                                StationIndex = x.StationIndex,
                                DateTime = DateTime.Now,
                                WorkpieceTag = x.WorkpieceTag,
                                PointIndex = i,
                                PositionIndex = i
                            };

                            _event.OnNext(me);

                            return;
                        }
                    }
                });
            });
        }
        
        public IObservable<MeasureEvent> Event => _event;
        public Collection<int> StationIndexs { get; set; } = new Collection<int>();

        public string SubscriptionId { get; set; }
    }
}
