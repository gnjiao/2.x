using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hdc.Measuring
{
    public class SetRobotIoTrigger : IMeasureTrigger
    {
        public TriggerType TriggerType { get; set; }
        public string Label { get; set; }
        public int BitNumber { get; set; } = 0;
        public bool OnOff { get; set; }

        public void Action(IMeasureDevice measureDevice, int pointIndex, MeasureResult measureResult = null)
        {
            if(!string.IsNullOrEmpty(Label))
                XyRcApiInitializer.Singleton.SetIoOn(OnOff, Label);
            else
                XyRcApiInitializer.Singleton.SetIoOn(OnOff, BitNumber);
        }
    }
}
