using System;
using System.Threading;

namespace Hdc.Measuring
{
    [Serializable]
    public class DelayMeasureTrigger : IMeasureTrigger
    {
        public int TargetIndex { get; set; }
        public TriggerType TriggerType { get; set; }

        public void Action(IMeasureDevice measureDevice, int pointIndex, MeasureResult measureResult)
        {
            Thread.Sleep(Delay);
        }

        public int Delay { get; set; }
    }
}