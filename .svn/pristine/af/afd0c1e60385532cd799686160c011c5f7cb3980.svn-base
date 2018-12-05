using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty(nameof(PointPositions))]
    public class AggregationCalculationOperationPlus : ICalculationOperationPlus
    {
        public IEnumerable<MeasureOutput> Calculate(IList<MeasureResult> measureResults)
        {
            double[] x = PointPositions.Select(p =>
            {
                if (Math.Abs(p.X) > 0.000001)
                    return p.X;

                return measureResults.Single(m => m.Index == p.MeasureResultIndex)
                    .MeasureEvent.X;
            }).ToArray();

            double[] y = PointPositions.Select(p =>
            {
                if (Math.Abs(p.Y) > 0.000001)
                    return p.Y;

                return measureResults.Single(m => m.Index == p.MeasureResultIndex)
                    .MeasureEvent.Y;
            }).ToArray();

            double[] z = PointPositions.Select(p => measureResults.Single(m => m.Index == p.MeasureResultIndex)
                .Outputs[p.MeasureOutputIndex].Value).ToArray();

            var maxZ = z.Max();
            var minZ = z.Min();
            var avgZ = z.Average();
            var stdDev = z.CalculateStdDev();
            var stepZ = maxZ - minZ;
            var step5to6 = z[8]-z[3];

            var outputs = new List<MeasureOutput>()
            {
                new MeasureOutput()
                {
                    Name = "MaxZ",
                    Value = maxZ,
                },
                new MeasureOutput()
                {
                    Name = "MinZ",
                    Value = minZ,
                },
                new MeasureOutput()
                {
                    Name = "AvgZ",
                    Value = avgZ,
                },
                new MeasureOutput()
                {
                    Name = "STDDev",
                    Value = stdDev,
                },
                new MeasureOutput()
                {
                    Name="FlatNessError",
                    Value = stepZ,
                },
                new MeasureOutput()
                {
                    Name="Step5to6",
                    Value = stepZ,
                },
            };

            return outputs;
        }


        public Collection<PointPosition> PointPositions { get; set; } = new Collection<PointPosition>();
   }
}