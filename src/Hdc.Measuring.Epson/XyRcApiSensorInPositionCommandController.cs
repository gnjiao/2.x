using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class XyRcApiSensorInPositionCommandController : ICommandController
    {
        public void Initialize()
        {
        }

        public void Command(MeasureEvent measureEvent)
        {
            XyRcApiInitializer.Singleton.SensorInPositionCommand();
        }
    }
}