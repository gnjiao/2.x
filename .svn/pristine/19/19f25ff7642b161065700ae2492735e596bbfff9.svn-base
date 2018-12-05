using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Hdc.Measuring.Minitab
{
    public static class MinitabOperations
    {
        public static string GetCommand(MinitabCommandConfig config)
        {
            var cmd = GetWOpen(config.WOPEN_FilePath) +
                      GetSheets(config.SHEET_Count, config.SHEET_VNAMES, config.SHEET_FIRST, config.SHEET_NROWS) +
                      $"END.\n" +
                      GetHistograms(config.OutputMtbDirectory, config.HistogramEntries) +
                      $"END.\n";
            return cmd;
        }

        public static string GetWOpen(string fileName)
        {
            var cmd = $"WOPEN \"{fileName}\";\n" +
                      $"    FTYPE;\n" +
                      $"        EXCEL;\n" +
                      $"    DATA;\n" +
                      $"        IGNOREBLANKROWS;\n" +
                      $"        EQUALCOLUMNS;\n";
            return cmd;
        }

        public static string GetSheets(int count, int VNAMES, int FIRST, int NROWS)
        {
            var s = "";
            for (int i = 1; i <= count; i++)
            {
                s += GetSheet(i, VNAMES, FIRST, NROWS);
            }
            return s;
        }

        public static string GetSheet(int index, int VNAMES, int FIRST, int NROWS)
        {
            var s = $"    SHEET {index};\n" +
                    $"        VNAMES {VNAMES};\n" +
                    $"        FIRST {FIRST};\n" +
                    $"        NROWS {NROWS};\n";
            return s;
        }

        public static string GetHistograms(string outputDir, IEnumerable<MinitabHistogramEntry> entries)
        {
            var s = "";
            foreach (var entry in entries)
            {
                s += GetHistogram(outputDir, entry.StationName, entry.ColumnName, entry.Format);
            }

            return s;
        }


        public static string GetHistogram(string outputDir, string stationName, string columnName, string format)
        {
            string ext = ".unknown";
            switch (format)
            {
                case null:
                    ext = ".mgf";
                    break;
                case "":
                    ext = ".mgf";
                    break;
                case "MGF":
                    ext = ".mgf";
                    break;
                case "JPEG":
                    ext = ".jpg";
                    break;
                case "TIF":
                    ext = ".tif";
                    break;
            }

            var formatSytax = format;
            if (formatSytax == "MGF") formatSytax = "";

            var outputFileName = Path.Combine(outputDir, stationName + "_" + columnName) + ext;

            var cmd = $"Worksheet '{stationName}';\n" +
                      $"Current.\n" +
                      $"Histogram '{columnName}';\n" +
                      $"    Bar;\n" +
                      $"    Distribution;\n" +
                      $"        Normal;\n" +
                      $"    GSave \"{outputFileName}\";\n" +
                      $"        REPLACE;\n" +
                      $"        {formatSytax};\n" +
                      $"END.\n";
            return cmd;
        }
    }
}