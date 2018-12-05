using System;
using HalconDotNet;
using Hdc.Mv.ImageAcquisition;
using Hdc.Mv.ImageAcquisition.Halcon;

namespace Hdc.Measuring
{
    [Serializable]
    public class MultipleSetFramegrabberParamMeasureTrigger : IMeasureTrigger
    {
        public TriggerType TriggerType { get; set; }

        public void Action(IMeasureDevice measureDevice, int pointIndex, MeasureResult measureResult)
        {
            var halconDevice = measureDevice as MultipleCameraHalconInspectorMeasureDevice;
            if (halconDevice == null)
                throw new InvalidOperationException(
                    $"{nameof(SetFramegrabberParamMeasureTrigger)}: " +
                    $"measureDevice is not CameraHalconInspectorMeasureDevice");
          
            if (halconDevice.HFramegrabberProviders[Index] == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(SetFramegrabberParamMeasureTrigger)}: " +
                    $"measureDevice.HFramegrabberProvider is null");
            }

            halconDevice.HFramegrabberProviders[Index].SetFramegrabberParam(Name, Value, DataType);
        }

        public int Index { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public FrameGrabberParamEntryDataType DataType { get; set; } = FrameGrabberParamEntryDataType.String;
    }
}