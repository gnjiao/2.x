using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class ChangeExposureTimeForJaiCameraMeasureTrigger: IMeasureTrigger
    {
        public TriggerType TriggerType { get; set; }
        public void Action(IMeasureDevice measureDevice, int pointIndex, MeasureResult measureResult)
        {
//            var cameraDevice = measureDevice as CameraHalconInspectorMeasureDevice;
            var jai = JaiCameraWrapper.JaiCamera;
            jai.ChangeExposureTime(ExposureTime);
        }

        public double ExposureTime { get; set; }
    }
}