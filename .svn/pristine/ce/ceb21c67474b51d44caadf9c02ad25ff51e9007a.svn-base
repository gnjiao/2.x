using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class QueueSchedulingWorkpieceInPositionCommandController : ICommandController
    {
        public void Initialize()
        {            
        }

        public void Command(MeasureEvent measureEvent)
        {
            QueueSchedulingInitializer.Singleton.WorkInPositionCommand(measureEvent);
        }
    }
}
