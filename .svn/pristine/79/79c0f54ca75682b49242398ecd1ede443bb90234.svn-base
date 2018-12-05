using System;
using System.IO;
using System.Linq;
using Hdc.Linq;
using Hdc.Reflection;
using Hdc.Serialization;

namespace Hdc.Measuring.Reporting
{
    [Serializable]
    public class XamlReportStationResultProcessor : IStationResultProcessor
    {
        private readonly string _assemblyDirectoryPath;
//        private string _reportsDir;
        private DateTime _bootDateTime = DateTime.Now;
        private int _reportCounter;

        public XamlReportStationResultProcessor()
        {
            _assemblyDirectoryPath = typeof (XamlReportStationResultProcessor).Assembly.GetAssemblyDirectoryPath();
        }

        public string ReportDirectoryPath { get; set; } = "_Reports_XAML";

        public bool IsAbsolutePath { get; set; } = false;

        public void Process(StationResult stationResult2)
        {
            var now = DateTime.Now;
            var reportsDir = GetReportsDir();

            var clone = stationResult2.DeepClone();
            clone.ClearCalculationOperations();
            clone.CalculateResults.ForEach(x => x.Definition.CalculateOperation = null);

            // serialize xaml of zip
            var fileName = $"{reportsDir}\\{clone.WorkpieceTag:D6}_{now.ToString("yyyy-MM-dd_HH.mm.ss")}_{_reportCounter.ToString("D8")}.xaml.zip";
            clone.SerializeToXamlFileOfZip(fileName);
            _reportCounter++;
        }

        public string GetReportsDir()
        {
            if (string.IsNullOrEmpty(ReportDirectoryPath))
            {
                ReportDirectoryPath = "_Reports_XAML";
                IsAbsolutePath = false;
            }

            string reportsDir;
            if (IsAbsolutePath)
            {
                reportsDir = ReportDirectoryPath;
            }
            else
            {
                reportsDir = Path.Combine(_assemblyDirectoryPath, ReportDirectoryPath);
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
    }
}