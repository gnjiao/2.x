using System;
using System.Diagnostics;
using Hdc.Mv.Halcon;

namespace Hdc.Measuring
{
    [Serializable]
    public class SaveImageFromCameraPlugin : IMeasureSchemaPlugin
    {
        public void Initialize(MeasureSchema measureSchema)
        {
            for (int i = 0; i < measureSchema.MeasureDefinitions.Count; i++)
            {
                var md = measureSchema.MeasureDefinitions[i];
                var cameraDevice = md.Device as CameraHalconInspectorMeasureDevice;

                var i1 = i;
                cameraDevice?.ImageAcquisitedEvent.Subscribe(
                    x =>
                    {
                        if (IsDisabled)
                            return;

                        string fileName;

                        if (!IsOverwrite)
                        {
                            var now = DateTime.Now;
                            fileName = $"{ImageDirectory}\\{now.ToString("yyyy-MM-dd_HH.mm.ss")}_{i1.ToString("D8")}.tif";
                            x.WriteImageOfTiffLzw(fileName);
                        }
                        else
                        {
                            fileName = $"{ImageDirectory}\\{i1.ToString("D2")}.tif";
                            x.WriteImageOfTiffLzw(fileName);
                        }

                        Console.WriteLine("Image saved: " + fileName);
                        Debug.WriteLine("Image saved: " + fileName);
                    });
            }
        }

        public string ImageDirectory { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsOverwrite { get; set; }
    }
}