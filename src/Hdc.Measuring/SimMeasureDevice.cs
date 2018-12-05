using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class SimMeasureDevice: IMeasureDevice
    {
        public void Initialize()
        {
            IsInitialized = true;
        }

        public bool IsInitialized { get; set; }

        public MeasureResult Measure(MeasureEvent measureEvent)
        {
            return new MeasureResult();
        }
    }
}