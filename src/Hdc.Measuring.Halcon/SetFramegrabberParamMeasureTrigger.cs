using System;
using HalconDotNet;
using Hdc.Mv.ImageAcquisition;
using Hdc.Mv.ImageAcquisition.Halcon;

namespace Hdc.Measuring
{
    [Serializable]
    public class SetFramegrabberParamMeasureTrigger : IMeasureTrigger
    {
        public TriggerType TriggerType { get; set; }

        public void Action(IMeasureDevice measureDevice, int pointIndex, MeasureResult measureResult)
        {
            var halconDevice = measureDevice as CameraHalconInspectorMeasureDevice;
            if (halconDevice == null)
                throw new InvalidOperationException(
                    $"{nameof(SetFramegrabberParamMeasureTrigger)}: " +
                    $"measureDevice is not CameraHalconInspectorMeasureDevice");
          
            if (halconDevice.HFramegrabberProvider == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(SetFramegrabberParamMeasureTrigger)}: " +
                    $"measureDevice.HFramegrabberProvider is null");
            }

            halconDevice.HFramegrabberProvider.SetFramegrabberParam(Name, Value, DataType);
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public FrameGrabberParamEntryDataType DataType { get; set; } = FrameGrabberParamEntryDataType.String;
    }
}