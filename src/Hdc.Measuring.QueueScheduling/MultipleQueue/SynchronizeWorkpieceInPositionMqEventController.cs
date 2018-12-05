using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
    public class SynchronizeWorkpieceInPositionMqEventController : IEventController
    {
        private readonly Subject<MeasureEvent> _event = new Subject<MeasureEvent>();        

        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;

            MqInitializer.Bus.Subscribe<WorkpieceInPositionEvent>(SubscriptionId, x =>
            {
                Task.Run(() =>
                {
                    if (x.StationIndex != 1) return;

                    // var stationInfo = StationInfos.FirstOrDefault(p => p.StationIndex == 3/*x.StationIndex*/);

                    // if (stationInfo != null) stationInfo.WorkInpieceFlag = true;

                    // if (StationInfos.Any(si => si.WorkInpieceFlag == false))
                    //     return;

                    var me = new MeasureEvent
                    {
                        StationIndex = 0,
                        DateTime = DateTime.Now,
                        WorkpieceTag = x.WorkpieceTag
                    };

                    _event.OnNext(me);

                    foreach (var si in StationInfos)
                    {
                        si.WorkInpieceFlag = false;
                    }

                });
            });

            _isInitialized = true;
        }

        public IObservable<MeasureEvent> Event
        {
            get
            {
                if (_event != null)
                {
                    lock (StationInfos)
                    {
                        return _event;
                    }
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
