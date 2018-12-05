using System.Collections.Generic;

namespace Hdc.Measuring
{
    public interface ICalculationOperationPlus
    {
        IEnumerable<MeasureOutput> Calculate(IList<MeasureResult> measureResults);
    }
}