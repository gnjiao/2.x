using System.Collections.Generic;

namespace Hdc.Measuring
{
    public interface ICalculateOperation
    {
        MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition);
    }
}