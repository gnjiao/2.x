using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class QueueSchedulingSensorInPositonCommandController : ICommandController
    {
        public void Initialize()
        {
        }

        public void Command(MeasureEvent measureEvent)
        {            
            QueueSchedulingInitializer.Singleton.SensorInPositionCommand(measureEvent);
        }
    }
}
