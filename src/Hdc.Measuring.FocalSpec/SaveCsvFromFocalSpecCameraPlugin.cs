using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using FocalSpec.FsApiNet.Model;

namespace Hdc.Measuring
{
    [Serializable]
    public class SaveCsvFromFocalSpecCameraPlugin : IMeasureSchemaPlugin
    {
        public void Initialize(MeasureSchema measureSchema)
        {
            for (var i = 0; i < measureSchema.MeasureDefinitions.Count; i++)
            {
                var md = measureSchema.MeasureDefinitions[i];
                var focalSpecDevice = md.Device as FocalSpecInspectorMeasureDevice;

                var i1 = i;
                focalSpecDevice?.CsvAcquisitedEvent.Subscribe(
                    x =>
                    {
                        if (IsDisabled)
                            return;

                        string fileName = null;

                        if (!IsOverwrite)
                        {
                            for (int index = 0; index < x.Count; index++)
                            {
                                var latestProfile = x[index];
                                var now = DateTime.Now;
                                fileName =
                                    $"{CsvDirectory}\\{now.ToString("yyyy-MM-dd_HH.mm.ss")}_{i1.ToString("D8")}_{index}.csv";
                                ExportCsv.Export(fileName, latestProfile);
                            }
                        }
                        else
                        {
                            for (int index = 0; index < x.Count; index++)
                            {
                                var latestProfile = x[index];
                                fileName = $"{CsvDirectory}\\{i1.ToString("D2")}_{index}.csv";
                                ExportCsv.Export(fileName, latestProfile);
                            }
                        }

                        Console.WriteLine("Csv saved: " + fileName);
                        Debug.WriteLine("Csv saved: " + fileName);
                    });

                focalSpecDevice?.CsvTempEvent.Subscribe(
                    x =>
                    {
                        if (IsDisabled)
                            return;

                        string fileName = null;
                        
                        var now = DateTime.Now;
                        fileName = $"{CsvDirectory}\\{now.ToString("yyyy-MM-dd_HH.mm.ss")}.csv";

                        var str = new StringBuilder();

                        for (var index = 0; index < x[0].Count; index++)
                        {
                            str.AppendLine($"{x[0][index]:0.000000};{x[1][index]:0.000000};");
                        }

                        File.WriteAllText(fileName, str.ToString());

                        Console.WriteLine("csv saved: " + fileName);
                        Debug.WriteLine("csv saved: " + fileName);
                    });
            }
        }

        public string CsvDirectory { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsOverwrite { get; set; }
    }
}
