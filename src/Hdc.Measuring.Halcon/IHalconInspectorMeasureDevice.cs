using System.Collections.ObjectModel;
using Hdc.Mv.Inspection;

namespace Hdc.Measuring
{
    public interface IHalconInspectorMeasureDevice : IMeasureDevice
    {
        IHalconInspector Inspector { get; set; }

//        MeasureDataMap MeasureDataMap { get; set; }

        Collection<IMeasureDataMapEntry> MeasureDataMapEntries { get; set; }
    }
}