using System.Collections.Generic;
using System.Linq;

namespace Hdc.Measuring
{
    public static class Ex
    {
        public static void ClearCalculationOperations(this StationResult stationResult)
        {
            foreach (var calculateResult in stationResult.CalculateResults)
            {
                calculateResult.Definition.CalculateOperation = null;
            }
        }

        public static List<IMeasureTrigger> GetBeforeTriggers(this MeasureDefinition stationResult)
        {
            return stationResult.Triggers?.Where(x => x.TriggerType == TriggerType.Before).ToList() ?? new List<IMeasureTrigger>();
        }

        public static List<IMeasureTrigger> GetAfterTriggers(this MeasureDefinition stationResult)
        {
            return stationResult.Triggers?.Where(x => x.TriggerType == TriggerType.After).ToList() ?? new List<IMeasureTrigger>();
        }

        public static List<WorkpieceResult> MergeWorkpieceResults(this IEnumerable<StationResult> stationResults)
        {
            var stationResultsGroupByWorkpiece = stationResults
                .OrderBy(x => x.WorkpieceTag)
                .GroupBy(x => x.WorkpieceTag);

            var workpieceResults = new List<WorkpieceResult>();
            foreach (var group in stationResultsGroupByWorkpiece)
            {
                if (!group.Any())
                    continue;

                var tag = group.First().WorkpieceTag;
                var wp = new WorkpieceResult() { Tag = tag, };

                for (int i = 0; i < 32; i++)
                {
                    var i1 = i;
                    var srs = group.Where(x => x.StationIndex == i1).ToList();
                    if (!srs.Any())
                    {
                        continue;
                        var sr = new StationResult()
                        {
                            StationIndex = i,
                        };
                        wp.StationResults.Add(sr);
                    }
                    else
                    {
                        var sr = srs.OrderByDescending(x => x.MeasureDateTime).First();
                        wp.StationResults.Add(sr);

                        if (string.IsNullOrEmpty(wp.WorkpieceDataCode) && (sr.WorkpieceDataCode != null))
                        {
                            wp.WorkpieceDataCode = sr.WorkpieceDataCode;
                        }

                        if (string.IsNullOrEmpty(wp.FixtureDataCode) && (sr.FixtureDataCode != null))
                        {
                            wp.FixtureDataCode = sr.FixtureDataCode;
                        }
                    }
                }

                if (wp.FixtureDataCode != null)
                {
                    foreach (var stationResult in wp.StationResults)
                    {
                        foreach (var calculateResult in stationResult.CalculateResults)
                        {
                            var selected = calculateResult.Definition.FixtureCorrectionDefinitions
                                .FirstOrDefault(x => x.FixtureDataCode == wp.FixtureDataCode);

                            if (selected != null)
                            {
                                calculateResult.FixtureCorrectionValue = selected.CorrectionValue;
                            }

                            calculateResult.Definition.FixtureCorrectionDefinitions.Clear();
                        }
                    }
                }


                workpieceResults.Add(wp);
            }

            return workpieceResults;
        }
    }
}