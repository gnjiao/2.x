using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hdc.Measuring
{
    [Serializable]
    public class SynchronizeWorkpieceInPositionCommandController : ICommandController
    {
        public void Initialize()
        {
        }

        public void Command(MeasureEvent measureEvent)
        {
            MultipleQueueSchedulingInitializer.Singleton.WorkInPositionCommand(measureEvent);
        }
    }
}
