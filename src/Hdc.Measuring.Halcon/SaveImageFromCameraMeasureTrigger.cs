using System;
using System.Diagnostics;
using Hdc.Mv.Halcon;

namespace Hdc.Measuring
{
    [Serializable]
    public class SaveImageFromCameraMeasureTrigger : IMeasureTrigger
    {
        public int TargetIndex { get; set; }
        public TriggerType TriggerType { get; set; }

        private bool _isSubscribed;

        private int _currentPointIndex;

        public void Action(IMeasureDevice measureDevice, int pointIndex, MeasureResult measureResult)
        {
            var cameraDevice = measureDevice as CameraHalconInspectorMeasureDevice;
            _currentPointIndex = pointIndex;

            if (!_isSubscribed)
            {
                _isSubscribed = true;

                cameraDevice.ImageAcquisitedEvent.Subscribe(
                    x =>
                    {
                        if (IsDisabled)
                            return;

                        var now = DateTime.Now;

                        var fileName =
                            $"{ImageDirectory}\\{now.ToString("yyyy-MM-dd_HH.mm.ss")}_{_currentPointIndex.ToString("D8")}.tif";
                        x.WriteImageOfTiffLzw(fileName);

                        Console.WriteLine("Image saved: " + fileName);
                        Debug.WriteLine("Image saved: " + fileName);
                    });
            }
        }

        public string ImageDirectory { get; set; }

        public bool IsDisabled { get; set; }
    }
}