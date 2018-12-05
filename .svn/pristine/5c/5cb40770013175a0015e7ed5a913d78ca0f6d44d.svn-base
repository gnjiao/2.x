using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class CommonTcpSrvSensorInPositonCommandController : ICommandController
    {
        public void Initialize()
        {
        }

        public void Command(MeasureEvent measureEvent)
        {
            CommonTcpSrvInitializer.Singleton.SensorInPositionCommand();
        }
    }
}
