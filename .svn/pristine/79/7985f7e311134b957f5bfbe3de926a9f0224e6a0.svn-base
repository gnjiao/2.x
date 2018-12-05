using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hdc.Mv.Halcon;

namespace Hdc.Measuring
{
    [Serializable]
    public class MultipleSaveImageFromCameraPlugin : IMeasureSchemaPlugin
    {
        public void Initialize(MeasureSchema measureSchema)
        {
            for (int i = 0; i < measureSchema.MeasureDefinitions.Count; i++)
            {
                var md = measureSchema.MeasureDefinitions[i];
                var cameraDevice = md.Device as MultipleCameraHalconInspectorMeasureDevice;

                var i1 = i;
                cameraDevice?.ImageAcquisitedEvent.Subscribe(
                    xImgs =>
                    {
                        if (IsDisabled)
                            return;

                        for (var index = 0; index < xImgs.Length; index++)
                        {
                            var x = xImgs[index];

                            string fileName;
                            if (!IsOverwrite)
                            {
                                var now = DateTime.Now;
                                fileName =
                                    $"{ImageDirectory}\\Index{index}_{now:yyyy-MM-dd_HH.mm.ss}_{i1:D8}.tif";
                                x.WriteImageOfTiffLzw(fileName);
                            }
                            else
                            {
                                fileName = $"{ImageDirectory}\\Index{index}_{i1:D2}.tif";
                                x.WriteImageOfTiffLzw(fileName);
                            }
                            Console.WriteLine("Image saved: " + fileName);
                            Debug.WriteLine("Image saved: " + fileName);
                        }
                    });
            }
        }

        public string ImageDirectory { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsOverwrite { get; set; }
    }
}
