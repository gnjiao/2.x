using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class XyRcApiStationCompletedEventController : IEventController
    {
        public void Initialize()
        {
            
        }

        public IObservable<MeasureEvent> Event => XyRcApiInitializer.Singleton.StationCompletedEvent;
    }
}