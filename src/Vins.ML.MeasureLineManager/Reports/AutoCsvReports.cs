using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Hdc.Measuring;
using Aspose.Cells;
using Hdc.Reflection;
using Hdc.Serialization;

namespace Vins.ML.MeasureLineManager.Reports
{
    [Serializable]
    class AutoCsvReports
    {
        public static string AssemblyDirectoryPath = typeof(AutoCsvReports).Assembly.GetAssemblyDirectoryPath();

        public static void AutoReportCsv(WorkpieceResult wr, int channelno)
        {
            var printstationindex = 1;
            var originalPrintColumn = 1;
            var totalCalculateResultesCount = 0;
            var fixtureDataCode = wr.FixtureDataCode;
            var totalStationResultesCount = wr.StationResults.Count;

            var csvDir = GetReportsDir();
            //var result = GetWorkpieceRank(wr);
            var passName = wr.IsNG2 ? "FAIL" : "PASS";
            var testTime = $"{wr.MeasureCompletedDateTime:yyyyMMddHHmmss}";
            //var testTime = $"{ GetWorkpieceFinishTime(wr):yyyyMMddHHmmss}";
            var stationReportList = new List<StationReport>();


            foreach (var sr in wr.StationResults)
            {
                var calculationReportList = new List<CaculationReport>();
                foreach (var cr in sr.CalculateResults)
                {
                    var cm = new CaculationReport
                    {
                        Sip = cr.Definition.SipNo,
                        StandardValue = cr.Definition.StandardValue.ToString("f4"),
                        ToleranceUpper = cr.Definition.ToleranceUpper.ToString("f4"),
                        ToleranceLower = cr.Definition.ToleranceLower.ToString("f4"),
                        FinalValue = cr.Output.Value.ToString("f4")
                    };
                    calculationReportList.Add(cm);
                }

                var stationReport = new StationReport
                {
                    TestTime = sr.MeasureDateTime.ToString(),
                    StationSn = sr.StationIndex.ToString(),
                    Result = sr.IsNG2 ? "FAIL" : "PASS",
                    CalculationReport = calculationReportList
                };

                calculationReportList.Clear();
                stationReportList.Add(stationReport);
                totalCalculateResultesCount += sr.CalculateResults.Count;
            }

            Workbook wb = new Workbook();
            wb.Worksheets[0].Cells[0, 0].Value = "BarCode";
            wb.Worksheets[0].Cells[1, 0].Value = "SIP#";
            wb.Worksheets[0].Cells[2, 0].Value = "StandardValue";
            wb.Worksheets[0].Cells[3, 0].Value = "ToleranceUpper";
            wb.Worksheets[0].Cells[4, 0].Value = "ToleranceLower";
            wb.Worksheets[0].Cells[0, 1].Value = "Test_Time";
            wb.Worksheets[0].Cells[0, 2].Value = "Station_SN";
            wb.Worksheets[0].Cells[0, 3].Value = "result";


            for (int i = 0; i < totalStationResultesCount; i++)
            {
                for (int j = 0; j < totalCalculateResultesCount; j++)
                {
                    wb.Worksheets[0].Cells[i + 5, j + 3].Value = "NA";
                }
                wb.Worksheets[0].Cells[i + 5, 0].Value = wr.FixtureDataCode;
                wb.Worksheets[0].Cells[i + 5, 1].Value = stationReportList[i].TestTime;
                wb.Worksheets[0].Cells[i + 5, 2].Value = stationReportList[i].StationSn;
                wb.Worksheets[0].Cells[i + 5, 3].Value = stationReportList[i].Result;
            }

            foreach (var sr in stationReportList)
            {
                foreach (var cl in sr.CalculationReport)
                {
                    wb.Worksheets[0].Cells[0, originalPrintColumn + 3].Value = originalPrintColumn;
                    wb.Worksheets[0].Cells[1, originalPrintColumn + 3].Value = cl.Sip;
                    wb.Worksheets[0].Cells[2, originalPrintColumn + 3].Value = cl.StandardValue;
                    wb.Worksheets[0].Cells[3, originalPrintColumn + 3].Value = cl.ToleranceUpper;
                    wb.Worksheets[0].Cells[4, originalPrintColumn + 3].Value = cl.ToleranceLower;
                    wb.Worksheets[0].Cells[4 + printstationindex, originalPrintColumn + 3].Value = cl.FinalValue;
                    originalPrintColumn++;
                }
                printstationindex++;
            }

            var filename = Path.Combine(csvDir,
                $"{fixtureDataCode}_{passName}_{testTime}_{channelno}.csv");
            wb.Save(filename);
        }

        public static void AutoReportCsv(WorkpieceResult wr, string dir, int channelno)
        {
            var printstationindex = 1;
            var originalPrintColumn = 1;
            var totalCalculateResultesCount = 0;
            var fixtureDataCode = wr.FixtureDataCode;
            var totalStationResultesCount = wr.StationResults.Count;

            //var result = GetWorkpieceRank(wr);
            var passName = wr.IsNG2 ? "FAIL" : "PASS";
            var testTime = $"{wr.MeasureCompletedDateTime:yyyyMMddHHmmss}";
            var stationReportList = new List<StationReport>();


            foreach (var sr in wr.StationResults)
            {
                var calculationReportList = new List<CaculationReport>();
                foreach (var cr in sr.CalculateResults)
                {
                    var cm = new CaculationReport
                    {
                        Sip = cr.Definition.SipNo,
                        StandardValue = cr.Definition.StandardValue.ToString("f4"),
                        ToleranceUpper = cr.Definition.ToleranceUpper.ToString("f4"),
                        ToleranceLower = cr.Definition.ToleranceLower.ToString("f4"),
                        FinalValue = cr.Output.Value.ToString("f4")
                    };
                    calculationReportList.Add(cm);
                }

                var stationReport = new StationReport
                {
                    TestTime = sr.MeasureDateTime.ToString(),
                    StationSn = sr.StationIndex.ToString(),
                    Result = sr.IsNG2 ? "FAIL" : "PASS",
                    CalculationReport = calculationReportList
                };

                //calculationReportList.Clear();
                stationReportList.Add(stationReport);
                totalCalculateResultesCount += sr.CalculateResults.Count;
            }

            Workbook wb = new Workbook();
            wb.Worksheets[0].Cells[0, 0].Value = "BarCode";
            wb.Worksheets[0].Cells[1, 0].Value = "SIP#";
            wb.Worksheets[0].Cells[2, 0].Value = "StandardValue";
            wb.Worksheets[0].Cells[3, 0].Value = "ToleranceUpper";
            wb.Worksheets[0].Cells[4, 0].Value = "ToleranceLower";
            wb.Worksheets[0].Cells[0, 1].Value = "Test_Time";
            wb.Worksheets[0].Cells[0, 2].Value = "Station_SN";
            wb.Worksheets[0].Cells[0, 3].Value = "result";


            for (int i = 0; i < totalStationResultesCount; i++)
            {
                for (int j = 0; j < totalCalculateResultesCount; j++)
                {
                    wb.Worksheets[0].Cells[i + 5, j + 4].Value = "NA";
                }
                wb.Worksheets[0].Cells[i + 5, 0].Value = wr.FixtureDataCode;
                wb.Worksheets[0].Cells[i + 5, 1].Value = stationReportList[i].TestTime;
                wb.Worksheets[0].Cells[i + 5, 2].Value = stationReportList[i].StationSn;
                wb.Worksheets[0].Cells[i + 5, 3].Value = stationReportList[i].Result;
            }

            foreach (var sr in stationReportList)
            {
                foreach (var cl in sr.CalculationReport)
                {
                    wb.Worksheets[0].Cells[0, originalPrintColumn + 3].Value = originalPrintColumn;
                    wb.Worksheets[0].Cells[1, originalPrintColumn + 3].Value = cl.Sip;
                    wb.Worksheets[0].Cells[2, originalPrintColumn + 3].Value = cl.StandardValue;
                    wb.Worksheets[0].Cells[3, originalPrintColumn + 3].Value = cl.ToleranceUpper;
                    wb.Worksheets[0].Cells[4, originalPrintColumn + 3].Value = cl.ToleranceLower;
                    wb.Worksheets[0].Cells[4 + printstationindex, originalPrintColumn + 3].Value = cl.FinalValue;
                    originalPrintColumn++;
                }
                printstationindex++;
            }

            var filename = Path.Combine(dir,
                $"{fixtureDataCode}_{passName}_{testTime}_{channelno}.csv");
            wb.Save(filename);
        }


        public static bool GetWorkpieceRank(WorkpieceResult wp)
        {
            return wp.StationResults.Any() && wp.StationResults.Any(x => x.IsNG2);
        }

        public static DateTime? GetWorkpieceFinishTime(WorkpieceResult wp)
        {
            return !wp.StationResults.Any() ? null : wp.StationResults.Select(x => x.MeasureDateTime).Max();
        }

        public static string GetReportsDir()
        {

            if (string.IsNullOrEmpty(ReportCsvDirectoryPath))
            {
                ReportCsvDirectoryPath = "_Reports_Csv";
                IsAbsolutePath = false;
            }

            string reportsDir;
            if (IsAbsolutePath)
            {
                reportsDir = ReportCsvDirectoryPath;
            }
            else
            {
                reportsDir = Path.Combine(AssemblyDirectoryPath, ReportCsvDirectoryPath);
            }

            if (!Directory.Exists(reportsDir))
                Directory.CreateDirectory(reportsDir);

            // create dir names today
            var now = DateTime.Now;
            var today = now.ToString("yyyy-MM-dd");
            var todayDir = Path.Combine(reportsDir, today);
            if (!Directory.Exists(todayDir))
                Directory.CreateDirectory(todayDir);

            return todayDir;
        }

        public class StationReport
        {
            public string FixtureDataCode;
            public string TestTime;
            public string StationSn;
            public string Result;
            public List<CaculationReport> CalculationReport;
        }

        public class CaculationReport
        {
            public string Sip;
            public string StandardValue;
            public string ToleranceUpper;
            public string ToleranceLower;
            public string FinalValue;
        }

        public static string ReportCsvDirectoryPath { get; set; } = "_Reports_Csv";
        public static bool IsAbsolutePath { get; set; }
    }
}
