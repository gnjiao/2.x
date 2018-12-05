using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class EsponTcpSrvSensorInPositionEventController : IEventController
    {
        
        public void Initialize()
        {
        }

        public IObservable<MeasureEvent> Event => EsponTcpSrvInitializer.Singleton.SensorInPositionEvent;
    }
}
