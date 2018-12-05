using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class MultipleGrabImageStartTrigger : IMeasureTrigger
    {
        public TriggerType TriggerType { get; set; }

        public int Index { get; set; }

        public void Action(IMeasureDevice measureDevice, int pointIndex, MeasureResult measureResult)
        {
            MultipleHalconCameraInitializer.Singleton.HalconCamerals[Index].HFramegrabber.GrabImageStart(-1.0);
        }
    }
}