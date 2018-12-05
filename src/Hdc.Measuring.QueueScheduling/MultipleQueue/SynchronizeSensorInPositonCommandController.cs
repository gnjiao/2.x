using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class SynchronizeSensorInPositonCommandController : ICommandController
    {
        public void Initialize()
        {
        }

        public void Command(MeasureEvent measureEvent)
        {
            MultipleQueueSchedulingInitializer.Singleton.SensorInPositionCommand(measureEvent);
        }
    }
}
