using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Hdc.Measuring;

namespace Vins.ML.MeasureLineManager
{
    [Serializable]
    public static class M1787Report
    {
        public class WorkPieceReport
        {
            public string Sip= "NA";
            public string StandardValue = "NA";
            public string ToleranceUpper = "NA";
            public string ToleranceLower = "NA";
            public string FinalValue = "NA";            
        }

        public class StationReport
        {
            public string FixtureDataCode;
            public string TestTime;
            public string StationSn;
            public string Result;
        }


        static readonly string[] Sips = new string[14]
        {
            "AX1", "AX2", "AX3", "AX4", "AY1", "AY2", "AY3",
            "BX1", "BX2", "BX3", "BX4", "BY1", "BY2", "BY3"
        };

        public static bool ExportM1787Csv(WorkpieceResult wr, string dir)
        {
            
            try
            {
                var str = new StringBuilder();
                str.AppendLine("BarCode,Test_Time,Station_SN,result,1,2,3,4,5,6,7,8,9,10,11,12,13,14");

                var listWpr = Sips.Select(sip => new WorkPieceReport
                {
                    Sip = sip, StandardValue = "NA", ToleranceUpper = "NA", ToleranceLower = "NA", FinalValue = "NA"
                }).ToList();

                var fixtureDataCode = wr.FixtureDataCode;
                var passName = "PASS";
                var testTime = $"{wr.MeasureCompletedDateTime:yyyyMMddHHmmss}";

                var stationReportList = new List<StationReport>();

                foreach (var sr in wr.StationResults)
                {
                    var stationReport = new StationReport
                    {
                        FixtureDataCode = fixtureDataCode,
                        TestTime = sr.MeasureDateTime.ToString(),
                        StationSn = sr.StationIndex.ToString(),
                        Result = sr.IsNG2 ? "PASS" : "FAIL"
                    };

                    if (!sr.IsNG2)
                        passName = "FAIL";

                    stationReportList.Add(stationReport);

                    for (var j = 0; j < sr.CalculateResults.Count; j++)
                    {
                        var cr = sr.CalculateResults[j];

                        var wpr = listWpr.Find(p => p.Sip == cr.Definition.SipNo);
                        if (wpr == null) continue;
                        wpr.StandardValue = cr.Definition.StandardValue.ToString("f4");
                        wpr.ToleranceUpper = cr.Definition.ToleranceUpper.ToString("f4");
                        wpr.ToleranceLower = cr.Definition.ToleranceLower.ToString("f4");
                        wpr.FinalValue = cr.Output.Value.ToString("f4");
                    }
                }

                str.AppendLine($"SIP#,,,,{Sips[0]},{Sips[1]},{Sips[2]},{Sips[3]},{Sips[4]},{Sips[5]},{Sips[6]},{Sips[7]},{Sips[8]},{Sips[9]},{Sips[10]},{Sips[11]},{Sips[12]},{Sips[12]}");
                str.AppendLine($"StandardValue,,,,{listWpr[0].StandardValue},{listWpr[1].StandardValue},{listWpr[2].StandardValue},{listWpr[3].StandardValue},{listWpr[4].StandardValue},{listWpr[5].StandardValue}," +
                               $"{listWpr[6].StandardValue},{listWpr[7].StandardValue},{listWpr[8].StandardValue},{listWpr[9].StandardValue},{listWpr[10].StandardValue},{listWpr[11].StandardValue},{listWpr[12].StandardValue},{listWpr[13].StandardValue}");
                str.AppendLine($"ToleranceUpper,,,,{listWpr[0].ToleranceUpper},{listWpr[1].ToleranceUpper},{listWpr[2].ToleranceUpper},{listWpr[3].ToleranceUpper},{listWpr[4].ToleranceUpper},{listWpr[5].ToleranceUpper}," +
                               $"{listWpr[6].ToleranceUpper},{listWpr[7].ToleranceUpper},{listWpr[8].ToleranceUpper},{listWpr[9].ToleranceUpper},{listWpr[10].ToleranceUpper},{listWpr[11].ToleranceUpper},{listWpr[12].ToleranceUpper},{listWpr[13].ToleranceUpper}");
                str.AppendLine($"ToleranceLower,,,,{listWpr[0].ToleranceLower},{listWpr[1].ToleranceLower},{listWpr[2].ToleranceLower},{listWpr[3].ToleranceLower},{listWpr[4].ToleranceLower},{listWpr[5].ToleranceLower}," +
                               $"{listWpr[6].ToleranceLower},{listWpr[7].ToleranceLower},{listWpr[8].ToleranceLower},{listWpr[9].ToleranceLower},{listWpr[10].ToleranceLower},{listWpr[11].ToleranceLower},{listWpr[12].ToleranceLower},{listWpr[13].ToleranceLower}");

                str.AppendLine($"{stationReportList[0].FixtureDataCode}, {stationReportList[0].TestTime},{stationReportList[0].StationSn},{stationReportList[0].Result},{listWpr[0].FinalValue},{listWpr[1].FinalValue},{listWpr[2].FinalValue},{listWpr[3].FinalValue},{listWpr[4].FinalValue},{listWpr[5].FinalValue}," +
                               $"{listWpr[6].FinalValue},NA,NA,NA,NA,NA,NA,NA");

                str.AppendLine($"{stationReportList[1].FixtureDataCode}, {stationReportList[1].TestTime},{stationReportList[2].StationSn},{stationReportList[1].Result},NA,NA,NA,NA,NA,NA," +
                            $"NA,{listWpr[7].FinalValue},{listWpr[8].FinalValue},{listWpr[9].FinalValue},{listWpr[10].FinalValue},{listWpr[11].FinalValue},{listWpr[12].FinalValue},{listWpr[13].FinalValue}");


                var filename = Path.Combine(dir, $"{fixtureDataCode}_{passName}_{testTime}_{ConfigProvider.Config.ChannelNo}.csv");

                File.WriteAllText(filename, str.ToString());

                return true;
            }
            catch (Exception)
            {
                //                                
            }

            return false;
        }
    }
}
