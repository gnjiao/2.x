using System;
using System.Collections.Generic;

namespace Hdc.Measuring
{
    public interface IMeasureService
    {
        void Initialize(MeasureSchema measureSchema);

        IObservable<StationResult> StationCompletedEvent { get; }
    }
}