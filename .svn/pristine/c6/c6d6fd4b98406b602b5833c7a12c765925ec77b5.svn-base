using System;
using System.Reactive.Linq;

namespace Hdc.Measuring
{
    [Serializable]
    public class SynchronizeStationCompletedEventController : IEventController
    {
        public void Initialize()
        {
        }

        public IObservable<MeasureEvent> Event => MultipleQueueSchedulingInitializer.Singleton.StationCompletedEvent;
    }
}
