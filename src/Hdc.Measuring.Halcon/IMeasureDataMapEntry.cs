using Hdc.Mv.Inspection;

namespace Hdc.Measuring
{
    public interface IMeasureDataMapEntry
    {
        MeasureOutput GetMeasureData(InspectionResult result);

        int InspectionResultIndex { get; }
    }
}