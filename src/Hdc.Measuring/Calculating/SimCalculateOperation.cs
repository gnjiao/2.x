using System;
using System.Collections.Generic;

namespace Hdc.Measuring
{
    [Serializable]
    public class SimCalculateOperation: ICalculateOperation
    {
        Random _random = new Random(); 

        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            var pct = _random.Next(-100, 100);
            var pctValue = pct/Percent ;
            var range = TolerancePlus - ToleranceMinus;
            var unit = range/200.0;
            var offset = pctValue*unit;
            var mid = ExpectValue + (TolerancePlus + ToleranceMinus)/2.0;
            var finalVlaue = mid + offset;
            return new MeasureOutput() {Value =  finalVlaue};
        }

        public double ExpectValue { get; set; }

        public double TolerancePlus { get; set; }

        public double ToleranceMinus { get; set; }

        public double Percent { get; set; }
    }
}