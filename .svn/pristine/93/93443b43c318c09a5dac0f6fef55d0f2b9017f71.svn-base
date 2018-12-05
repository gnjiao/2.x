using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty(nameof(MeasureOutputReferences))]
    public class FlatnessErrorCalculateOperation2 : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            List<MeasureOutput> measureOutputs = new List<MeasureOutput>();

            List<double> xs = new List<double>();
            List<double> ys = new List<double>();
            List<double> zs = new List<double>();

            foreach (var measureOutputReference in MeasureOutputReferences)
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

            double[] distances;
            double f;
            FlatnessErrorOfA.FlatnessErrorOfACalculate(xValues, yValues, zValues, out distances, out f);

            MeasureOutput measureOutput = new MeasureOutput()
            {
                Value = f,
            };
            return measureOutput;
        }

        public Collection<MeasureOutputReference> MeasureOutputReferences { get; set; } =
            new Collection<MeasureOutputReference>();
    }
}