using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Reactive.Subjects;

namespace Hdc.Measuring
{
    [Serializable]
    public class SynchronizeSensorInPositionEventController : IEventController
    {
        private bool _isInitialized;

        private readonly Subject<MeasureEvent> _event = new Subject<MeasureEvent>();
        public void Initialize()
        {
            if (_isInitialized) return;

            MqInitializer.Bus.Subscribe<SensorInPositionOpcEvent>(SubscriptionId, x =>
            {
                Task.Run(() =>
                {
                    lock (StationInfos)
                    {
                        if (x.StationIndex == 0) return;

                        var stationInfo = StationInfos.FirstOrDefault(p => p.StationIndex == x.StationIndex);

                        if (stationInfo != null) stationInfo.SensorInPositionFlag = true;

                        if (StationInfos.Any(si => si.SensorInPositionFlag == false))
                            return;

                        _event.OnNext(new MeasureEvent
                        {
                            PointIndex = x.PointIndex,
                            StationIndex = x.StationIndex,
                            PositionIndex = x.PointIndex,
                            Message = x.Message,
                            WorkpieceTag = x.WorkpieceTag
                        });                       

                        foreach (var si in StationInfos)
                        {
                            si.SensorInPositionFlag = false;
                        }
                    }
                });
            });

            _isInitialized = true;
        }

        public IObservable<MeasureEvent> Event
        {
            get
            {
                if (_event != null) lock (StationInfos)
                {
                    return _event;
                }
                else
                {
                    return null;
                }
            }
        }

        private static string SubscriptionId => MultipleQueueSchedulingInitializer.Singleton.SubscriptionId;
        private static Collection<StationWorkInPieceInfo> StationInfos => MultipleQueueSchedulingInitializer.Singleton.StationInfos;
    }
}
