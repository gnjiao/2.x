using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Markup;

namespace Hdc.Measuring.GoDEX
{
    [Serializable]    
    [ContentProperty(nameof(FilterMeasureOutputs))]
    public class FilterPrintStationResultProcessor : IStationResultProcessor
    {
        public enum OperationType
        {
            Min = 0,
            Max = 1
        };
        public void Process(StationResult stationResult)
        {
            var tempOutput = new MeasureOutput();

            foreach (var measureOutput in FilterMeasureOutputs)
            {
                var measureResult = stationResult.MeasureResults
                        .Single(x => x.Index == measureOutput.MeasureResultIndex).Outputs[measureOutput.MeasureOutputIndex];
                                    
                switch (Operation)
                {
                    case OperationType.Min:

                        if (measureResult.Value < tempOutput.Value)
                        {
                            tempOutput.Validity = MeasureValidity.Error;

                            tempOutput = measureResult;
                        }

                        break;

                    case OperationType.Max:

                        if (measureResult.Value > tempOutput.Value)
                        {
                            tempOutput.Validity = MeasureValidity.Error;

                            tempOutput = measureResult;
                        }

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }                                    
        }

        public Collection<FilterMeasureOutput> FilterMeasureOutputs { get; set; } = new Collection<FilterMeasureOutput>();

        public OperationType Operation = OperationType.Min;
    }
}
