using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class QueueSchedulingStationCompletedEventController : IEventController
    {
        public void Initialize()
        {
        }

        public IObservable<MeasureEvent> Event => QueueSchedulingInitializer.Singleton.StationCompletedEvent;
    }
}
