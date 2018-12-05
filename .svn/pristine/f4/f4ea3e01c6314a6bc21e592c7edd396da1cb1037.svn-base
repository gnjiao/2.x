using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hdc.Measuring
{
    [Serializable]
    public class QueueSchedulingSensorInPositionEventController : IEventController
    {
        public void Initialize()
        {            
        }

        public IObservable<MeasureEvent> Event => QueueSchedulingInitializer.Singleton.SensorInPositionEvent;
    }
}
