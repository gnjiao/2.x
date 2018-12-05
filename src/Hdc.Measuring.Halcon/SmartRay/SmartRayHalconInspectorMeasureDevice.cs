using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using System.Windows;
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
    public class SmartRayHalconInspectorMeasureDevice : IHalconInspectorMeasureDevice
    {
        private readonly Subject<HImage> _imageAcquisitedEvent = new Subject<HImage>();
        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;

            if (Inspector != null)
            {
                Console.WriteLine($"{nameof(SmartRayHalconInspectorMeasureDevice)}:: " +
                                  $"{nameof(Inspector)}.{nameof(HalconInspectionSchemaInspector.UpdateInspectionSchemaFromDir)}() " +
                                  $"begin at: " + DateTime.Now);
                ((HalconInspectionSchemaInspector)Inspector).UpdateInspectionSchemaFromDir();
                Console.WriteLine($"{nameof(SmartRayHalconInspectorMeasureDevice)}:: " +
                                  $"{nameof(Inspector)}.{nameof(HalconInspectionSchemaInspector.UpdateInspectionSchemaFromDir)}() " +
                                  $"end at: " + DateTime.Now);
            }

            for (int i = 0; i < Inspectors.Count; i++)
            {
                var inspector = Inspectors[i];
                Console.WriteLine($"{nameof(SmartRayHalconInspectorMeasureDevice)}:: " +
                                  $"{nameof(Inspectors)}[{i}].{nameof(HalconInspectionSchemaInspector.UpdateInspectionSchemaFromDir)}() " +
                                  $"begin at: " + DateTime.Now);
                ((HalconInspectionSchemaInspector)inspector).UpdateInspectionSchemaFromDir();
                Console.WriteLine($"{nameof(SmartRayHalconInspectorMeasureDevice)}:: " +
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
            HImage hImage8Bpp;

            if (HFramegrabberProvider == null)
            {
                Console.WriteLine($"{nameof(HFramegrabberProvider)} == null, the program has stopped.");
#if !DEBUG
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(255);
#endif
                throw new InvalidOperationException($"{nameof(HFramegrabberProvider)} == null");
            }

            hImage8Bpp = HFramegrabberProvider.GrabImage();
            //hImage8Bpp.WriteImage("tiff", 0, "b:\\GrabbdImage.tif");
            //hImage8Bpp = HFramegrabberProvider.HFramegrabber.GrabImageAsync(HFramegrabberProvider.GrabAsyncMaxDelay);
            
            Console.WriteLine("CameraHalconInspectorMeasureDevice::Measure() end at: " + DateTime.Now);
            sw2.Dispose();

            HImage calibrate8Bpp;
            if (Calibrator == null)
            {
                calibrate8Bpp = hImage8Bpp;
            }
            else
            {
                try
                {
                    if (IsFitPlane)
                    {
                        Console.WriteLine("CameraHalconInspectorMeasureDevice.Measure(): Calibrate8Bpp() begin at: " +
                                          DateTime.Now);
                        //hImage8Bpp = hImage8Bpp.ConvertImageType("byte");
                        //calibrate8Bpp = Calibrator.Calibrate(hImage8Bpp);
                        calibrate8Bpp = hImage8Bpp;
                        calibrate8Bpp.CropPart(0, 0, 2300, 8000);
                        HImage croppartHImage = new HImage();
                        croppartHImage = calibrate8Bpp.CropPart(0, 0, 2300, 8000);
                        // croppartHImage.WriteImage("tiff", 0, "b:\\CalibratedImage0000000.tif");

                        double minGray, maxGray, range;
                        //求取整幅图像的一个灰度范围，然后提取主要部分，将最高亮度的图像去掉
                        croppartHImage.MinMaxGray(croppartHImage, 0.0, out minGray, out maxGray, out range);
                        //wangtedGray这个值是最高灰度减去SubValue值，就是我们目标的图像最高的灰度
                        double wangtedGray = maxGray - SubValue;
                        HRegion regiondomain = croppartHImage.Threshold(0, wangtedGray);
                        calibrate8Bpp.Dispose();
                        HImage cropDomainImage = croppartHImage.ReduceDomain(regiondomain).CropDomain();
                        calibrate8Bpp = cropDomainImage;

                        var hRegion = new HRegion();
                        var plane = new HImage();
                        double alpha, beta, gamma;
                        int width, height;
                        hRegion.ReadRegion(RegionHobjFile);
                        calibrate8Bpp.GetImageSize(out width, out height);
                        alpha = hRegion.FitSurfaceFirstOrder(calibrate8Bpp, "tukey", 5, 2.0, out beta, out gamma);
                        plane.GenImageSurfaceFirstOrder("uint2", alpha, beta, gamma, 0, 0, width, height);
                        plane = plane.AddImage(plane, PlanMult, PlanAdd);
                        calibrate8Bpp = calibrate8Bpp.SubImage(plane, 1.0, 0.0);
                        //calibrate8Bpp.WriteImage("tiff", 0, "b:\\CalibratedImage11111111.tif");

                        Console.WriteLine("CameraHalconInspectorMeasureDevice.Measure(): Calibrate8Bpp() end at: " +
                                          DateTime.Now);
                    }
                    else
                    {
                        Console.WriteLine("CameraHalconInspectorMeasureDevice.Measure(): Calibrate8Bpp() begin at: " +
                                          DateTime.Now);
                        //                    hImage8Bpp = hImage8Bpp.ConvertImageType("byte");
                        calibrate8Bpp = Calibrator.Calibrate(hImage8Bpp);
                        //calibrate8Bpp.WriteImage("tiff", 0, "b:\\CalibratedImage.tif");
                        Console.WriteLine("CameraHalconInspectorMeasureDevice.Measure(): Calibrate8Bpp() end at: " +
                                          DateTime.Now);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine(
                        "CameraHalconInspectorMeasureDevice.Measure(): Calibrate8Bpp() !!! error !!!, at: " +
                        DateTime.Now);
                    return GetErroMeasureResult();
                }
            }

            _imageAcquisitedEvent.OnNext(calibrate8Bpp);

            // add to cache
            if (IsAddToCacheEnabled)
            {
                HImage existImage;
                var isExist = HalconCameraInitializer.Singleton.Images.TryGetValue(ImageIndex, out existImage);
                if (isExist) existImage.Dispose();
                HalconCameraInitializer.Singleton.Images.AddOrUpdate(
                    ImageIndex, calibrate8Bpp.CopyImage(), (i, image) => calibrate8Bpp.CopyImage());
            }

            //
            var inspectionResults = new List<InspectionResult>();

            try
            {
                if (Inspector != null)
                {
                    Console.WriteLine($"{nameof(Inspectors)}.Inspect() begin at: " + DateTime.Now);
                    var inspectionResult = Inspector.Inspect(calibrate8Bpp);
                    Console.WriteLine($"{nameof(Inspectors)}.Inspect() end at: " + DateTime.Now);
                    inspectionResults.Add(inspectionResult);
                }

                for (var i = 0; i < Inspectors.Count; i++)
                {
                    var inspector = Inspectors[i];
                    Console.WriteLine($"{nameof(Inspectors)}[{i}].Inspect() begin at: " + DateTime.Now);
                    var inspectionResult2 = inspector.Inspect(calibrate8Bpp);
                    Console.WriteLine($"{nameof(Inspectors)}[{i}].Inspect() end at: " + DateTime.Now);
                    inspectionResults.Add(inspectionResult2);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("CameraHalconInspectorMeasureDevice.Measure(): Inspect() !!! error !!!, at: " +
                                  DateTime.Now);
                return GetErroMeasureResult();
            }

            calibrate8Bpp.Dispose();

            var md = new MeasureResult();

            foreach (var mapEntry in MeasureDataMapEntries)
            {
                var inspectionResult = inspectionResults[mapEntry.InspectionResultIndex];
                var output = mapEntry.GetMeasureData(inspectionResult);
                md.Outputs.Add(output);
            }

            return md;
        }

        public IHalconInspector Inspector { get; set; }

        public Collection<IHalconInspector> Inspectors { get; set; } = new Collection<IHalconInspector>();

        public IHFramegrabberProvider HFramegrabberProvider { get; set; }

        public Collection<IMeasureDataMapEntry> MeasureDataMapEntries { get; set; } =
            new Collection<IMeasureDataMapEntry>();

        public IObservable<HImage> ImageAcquisitedEvent => _imageAcquisitedEvent;

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
        public bool IsFitPlane { get; set; } = true;
        public double PlanMult { set; get; } = 0.5;
        public double PlanAdd { set; get; } = -600.0;
        public string RegionHobjFile { set; get; }
        public double SubValue { set; get; } = 0;
    }
}