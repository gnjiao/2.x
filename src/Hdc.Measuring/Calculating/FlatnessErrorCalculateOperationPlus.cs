using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty(nameof(FlatnessMeasureOutputReferences))]
    public class FlatnessErrorCalculateOperationPlus : ICalculationOperationPlus
    {
        public IEnumerable<MeasureOutput> Calculate(IList<MeasureResult> measureResults)
        {
            List<MeasureOutput> measureOutputs = new List<MeasureOutput>();

            List<double> xs = new List<double>();
            List<double> ys = new List<double>();
            List<double> zs = new List<double>();

            foreach (var measureOutputReference in FlatnessMeasureOutputReferences)
            {
                var mo =
                    measureResults[measureOutputReference.MeasureResultIndex]
                    .Outputs[measureOutputReference.MeasureOutputIndex];
                measureOutputs.Add(mo);

                xs.Add(measureOutputReference.UseMeasureEventXY ? mo.MeasureEvent.X : measureOutputReference.X);
                ys.Add(measureOutputReference.UseMeasureEventXY ? mo.MeasureEvent.Y : measureOutputReference.Y);
                zs.Add(mo.Value + measureOutputReference.Offset);
            }

            var xValues = xs.ToArray();
            var yValues = ys.ToArray();
            var zValues = zs.ToArray();

            var flat = new FlatFunction();
            flat.CalculateFlatFunction(xValues, yValues, zValues);
            //            double f = flat.GetFlatnessError();

            //

            var results = new List<MeasureOutput>();
            foreach (var measureOutputReference in DistanceMeasureOutputReferences)
            {
                var mo = measureResults[measureOutputReference.MeasureResultIndex]
                    .Outputs[measureOutputReference.MeasureOutputIndex];

               var x = (measureOutputReference.UseMeasureEventXY) ? mo.MeasureEvent.X : measureOutputReference.X;
               var y = (measureOutputReference.UseMeasureEventXY) ? mo.MeasureEvent.Y : measureOutputReference.Y;
               var z = mo.Value + measureOutputReference.Offset;

                var distance = flat.GetDistance(x, y, z);

                MeasureOutput distanceMeasureOutput = new MeasureOutput()
                {
                    Name = measureOutputReference.Name,
                    Value = distance,
                };
                results.Add(distanceMeasureOutput);
            }


            MeasureOutput measureOutput = new MeasureOutput()
            {
                Name = FlatnessErrorMeasureOutputName,
                Value = flat.FlatnessError,
            };

            results.Add(measureOutput);

            return results;
        }

        public Collection<MeasureOutputReference> FlatnessMeasureOutputReferences { get; set; } =
            new Collection<MeasureOutputReference>();

        public Collection<MeasureOutputReference> DistanceMeasureOutputReferences { get; set; } =
            new Collection<MeasureOutputReference>();

        public string FlatnessErrorMeasureOutputName { get; set; } = "FlatnessError";
    }
}