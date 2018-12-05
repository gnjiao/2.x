using System;

namespace Hdc.Measuring
{
    public interface IMeasureDevice
    {
        void Initialize();

        bool IsInitialized { get; }

        MeasureResult Measure(MeasureEvent measureEvent);
    }
}