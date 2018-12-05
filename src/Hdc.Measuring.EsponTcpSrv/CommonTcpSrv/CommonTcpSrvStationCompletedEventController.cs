using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class CommonTcpSrvStationCompletedEventController : IEventController
    {
        public void Initialize()
        {
        }

        public IObservable<MeasureEvent> Event => CommonTcpSrvInitializer.Singleton.StationCompletedEvent;
    }
}
