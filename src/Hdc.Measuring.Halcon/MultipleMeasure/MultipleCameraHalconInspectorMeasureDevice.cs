using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Markup;
using HalconDotNet;
using Hdc.Diagnostics;
using Hdc.Mv;
using Hdc.Mv.Halcon;
using Hdc.Mv.ImageAcquisition;
using Hdc.Mv.ImageAcquisition.Halcon;
using Hdc.Mv.Inspection;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("MeasureDataMapEntries")]
    public class MultipleCameraHalconInspectorMeasureDevice : IHalconInspectorMeasureDevice
    {
        private readonly Subject<HImage[]> _imageAcquisitedEvent = new Subject<HImage[]>();
        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;

            if (Inspector != null)
            {
                Console.WriteLine($"{nameof(CameraHalconInspectorMeasureDevice)}:: " +
                                  $"{nameof(Inspector)}.{nameof(HalconInspectionSchemaInspector.UpdateInspectionSchemaFromDir)}() " +
                                  $"begin at: " + DateTime.Now);
                ((HalconInspectionSchemaInspector)Inspector).UpdateInspectionSchemaFromDir();
                Console.WriteLine($"{nameof(CameraHalconInspectorMeasureDevice)}:: " +
                                  $"{nameof(Inspector)}.{nameof(HalconInspectionSchemaInspector.UpdateInspectionSchemaFromDir)}() " +
                                  $"end at: " + DateTime.Now);
            }

            for (var i = 0; i < Inspectors.Count; i++)
            {
                var inspector = Inspectors[i];
                Console.WriteLine($"{nameof(CameraHalconInspectorMeasureDevice)}:: " +
                                  $"{nameof(Inspectors)}[{i}].{nameof(HalconInspectionSchemaInspector.UpdateInspectionSchemaFromDir)}() " +
                                  $"begin at: " + DateTime.Now);
                ((HalconInspectionSchemaInspector) inspector).UpdateInspectionSchemaFromDir();
                Console.WriteLine($"{nameof(CameraHalconInspectorMeasureDevice)}:: " +
                                  $"{nameof(Inspectors)}[{i}].{nameof(HalconInspectionSchemaInspector.UpdateInspectionSchemaFromDir)}() " +
                                  $"end at: " + DateTime.Now);
            }

            _isInitialized = true;
        }

        public bool IsInitialized => _isInitialized;

        public MeasureResult Measure(MeasureEvent measureEvent)
        {
            var sw2 = new NotifyStopwatch("Camera.Acquisition()");
            Console.WriteLine("CameraHalconInspectorMeasureDevice::Measure() begin at: " + DateTime.Now);
                               
            if (HFramegrabberProviders == null)
            {
                Console.WriteLine($"{nameof(HFramegrabberProvider)} == null, the program has stopped.");                
#if !DEBUG
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(255);
#endif
                throw new InvalidOperationException($"{nameof(HFramegrabberProvider)} == null");
            }

            var hImage8Bpps = new HImage[HFramegrabberProviders.Count];

            for (var index = 0; index < HFramegrabberProviders.Count; index++)
            {
                hImage8Bpps[index] = HFramegrabberProviders[index].GrabImage();
            }

            //hImage8Bpp = HFramegrabberProvider.GrabImage();
            //                hImage8Bpp.WriteImage("tiff", 0, "b:\\GrabbdImage.tif");
            //                hImage8Bpp = HFramegrabberProvider.HFramegrabber.GrabImageAsync(HFramegrabberProvider.GrabAsyncMaxDelay);
            
            Console.WriteLine("CameraHalconInspectorMeasureDevice::Measure() end at: " + DateTime.Now);
            sw2.Dispose();


            var inspectionResults = new List<InspectionResult>();

            InspectionResultImage(hImage8Bpps, inspectionResults);
            
            var md = new MeasureResult();

            foreach (var mapEntry in MeasureDataMapEntries)
            {
                var inspectionResult = inspectionResults[mapEntry.InspectionResultIndex];
                var output = mapEntry.GetMeasureData(inspectionResult);
                md.Outputs.Add(output);
            }

            return md;
        }

        private void InspectionResultImage(IReadOnlyList<HImage> hImage8BppImags, ICollection<InspectionResult> inspectionResultOutput)
        {
            var calibrate8Bpp = new HImage[hImage8BppImags.Count];

            for (var index = 0; index < hImage8BppImags.Count; index++)
            {
                var hImage8Bpp = hImage8BppImags[index];
                if (Calibrator == null)
                {
                    calibrate8Bpp[index] = hImage8Bpp;
                }
                else
                {
                    try
                    {
                        Console.WriteLine("CameraHalconInspectorMeasureDevice.Measure(): Calibrate8Bpp() begin at: " +
                                          DateTime.Now);

                        calibrate8Bpp[index] = Calibrator.Calibrate(hImage8Bpp);

                        Console.WriteLine("CameraHalconInspectorMeasureDevice.Measure(): Calibrate8Bpp() end at: " +
                                          DateTime.Now);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(
                            "CameraHalconInspectorMeasureDevice.Measure(): Calibrate8Bpp() !!! error !!!, at: " +
                            DateTime.Now);
                        calibrate8Bpp[index] = hImage8Bpp;
                        //return GetErroMeasureResult();
                    }
                }

                // add to cache
                if (IsAddToCacheEnabled)
                {
                    HImage existImage;
                    var isExist = HalconCameraInitializer.Singleton.Images.TryGetValue(ImageIndex, out existImage);
                    if (isExist) existImage.Dispose();
                    HalconCameraInitializer.Singleton.Images.AddOrUpdate(
                        ImageIndex, calibrate8Bpp[index].CopyImage(), (i, image) => calibrate8Bpp[index].CopyImage());
                }
            }

            _imageAcquisitedEvent.OnNext(calibrate8Bpp);

            try
            {
                if (Inspector != null)
                {
                    foreach (var hImage in calibrate8Bpp)
                    {
                        Console.WriteLine($"{nameof(Inspectors)}.Inspect() begin at: " + DateTime.Now);
                        var inspectionResult = Inspector.Inspect(hImage);
                        Console.WriteLine($"{nameof(Inspectors)}.Inspect() end at: " + DateTime.Now);
                        inspectionResultOutput.Add(inspectionResult);
                    }
                }

                for (var index = 0; index < Inspectors.Count; index++)
                {
                    var inspector = Inspectors[index];
                    Console.WriteLine($"{nameof(Inspectors)}[{index}].Inspect() begin at: " + DateTime.Now);
                    var inspectionResult2 = inspector.Inspect(calibrate8Bpp[index]);
                    Console.WriteLine($"{nameof(Inspectors)}[{index}].Inspect() end at: " + DateTime.Now);
                    inspectionResultOutput.Add(inspectionResult2);
                }

//                var tasks = Inspectors.Select((inspector, index) => Task.Run(() =>
//                    {
//                        Console.WriteLine($"{nameof(Inspectors)}[{index}].Inspect() begin at: " + DateTime.Now);
//                        var inspectionResult2 = inspector.Inspect(calibrate8Bpp[index]);
//                        Console.WriteLine($"{nameof(Inspectors)}[{index}].Inspect() end at: " + DateTime.Now);
//                        inspectionResultOutput.Add(inspectionResult2);
//                    }))
//                    .ToList();
//
//                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception)
            {
                Console.WriteLine("CameraHalconInspectorMeasureDevice.Measure(): Inspect() !!! error !!!, at: " +
                                  DateTime.Now);
                //return GetErroMeasureResult();
            }

            foreach (var hImage in calibrate8Bpp)
            {
                hImage.Dispose();
            }
            
        }

        public IHalconInspector Inspector { get; set; }

        public Collection<IHalconInspector> Inspectors { get; set; } = new Collection<IHalconInspector>();

        public Collection<IHFramegrabberProvider> HFramegrabberProviders { get; set; } = new Collection<IHFramegrabberProvider>();

        public Collection<IMeasureDataMapEntry> MeasureDataMapEntries { get; set; } =
            new Collection<IMeasureDataMapEntry>();

        public IObservable<HImage[]> ImageAcquisitedEvent => _imageAcquisitedEvent;

        public IHalconImageCalibrator Calibrator { get; set; }

        public MeasureResult GetErroMeasureResult()
        {
            MeasureResult mr = new MeasureResult();
            foreach (var mapEntry in MeasureDataMapEntries)
            {
                var output = new MeasureOutput() { Value = 999.999, Validity = MeasureValidity.Error };
                mr.Outputs.Add(output);
            }
            return mr;
        }

        public bool IsAddToCacheEnabled { get; set; }
        public int ImageIndex { get; set; }
    }
}