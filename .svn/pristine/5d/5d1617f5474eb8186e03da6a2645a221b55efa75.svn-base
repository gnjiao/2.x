using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class GrabImageStartTrigger: IMeasureTrigger
    {
        public TriggerType TriggerType { get; set; }

        public void Action(IMeasureDevice measureDevice, int pointIndex, MeasureResult measureResult)
        {
            HalconCameraInitializer.SingletonHalconCamera.HFramegrabber.GrabImageStart(-1.0);
        }
    }
}