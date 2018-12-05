using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class EsponTcpSrvStationCompletedEventController : IEventController
    {
        public void Initialize()
        {
        }

        public IObservable<MeasureEvent> Event => EsponTcpSrvInitializer.Singleton.StationCompletedEvent;
    }
}
