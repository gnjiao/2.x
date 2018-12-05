using System;
using Hdc.Generators;
using Hdc.Measuring;

namespace Hdc.Measuring
{
    public static class SampleGenerator
    {
        public static StationResult CreateMeasureResult(int stationIndex = 0)
        {
            var random = new Random();

            var mr = new StationResult();
            for (int i = 0; i < 5; i++)
            {
                var cr = new CalculateResult()
                {
                    Index = i,
                    Output = new MeasureOutput()
                    {
                        Value = i + random.Next(-55, 55)*0.001,
                    },
                    Definition = new CalculateDefinition()
                    {
                        StandardValue = i,
                        SipNo = "#" + (stationIndex *100 + i),
                        Name = "Measurement-" + i.ToString(),
                        ToleranceUpper = 0.050,
                        ToleranceLower = -0.050,
                    },
                };

                mr.CalculateResults.Add(cr);
            }
            return mr;
        }

        public static WorkpieceResult CreateWorkpieceResult(int wpcTag = 0, int stationCount = 5)
        {
            var wr = new WorkpieceResult {Tag = wpcTag };

            for (int i = 0; i < stationCount; i++)
            {
                var measureDateTime = DateTime.Now;

                var mr = CreateMeasureResult(i);
                mr.StationIndex = i;
                mr.StationName = "Station-" + i.ToString("D2");
                mr.MeasureDateTime = measureDateTime;
                mr.WorkpieceTag = wr.Tag;
                
                wr.StationResults.Add(mr);
            }
            return wr;
        }

        static ISequenceGenerator _generator = new SequentialIdentityGenerator();

        public static WorkpieceResult CreateWorkpieceResultWithTag(int stationCount = 5)
        {
            int tag = (int) _generator.Next;
            var workpieceResultWithTag = CreateWorkpieceResult(tag, stationCount);
            return workpieceResultWithTag;
        }
    }
}