using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class EsponTcpSrvSensorInPositonCommandController : ICommandController
    {
        public void Initialize()
        {
        }

        public void Command(MeasureEvent measureEvent)
        {
            EsponTcpSrvInitializer.Singleton.SensorInPositionCommand();
        }
    }
}
