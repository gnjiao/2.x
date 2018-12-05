using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive.Subjects;
using HalconDotNet;
using Hdc.Mv.ImageAcquisition;

namespace Hdc.Measuring
{
    [Serializable]
    public class LoadFromFilesMeasureService : IMeasureService
    {
        private readonly Subject<StationResult> _stationCompletedEvent = new Subject<StationResult>();
        private MeasureSchema _measureSchema;
        private LoadImagesFromDirectoryPlugin _savePlugin;

        public void Initialize(MeasureSchema measureSchema)
        {
            _measureSchema = measureSchema;

            foreach (var plugin in measureSchema.Plugins)
            {
                _savePlugin = plugin as LoadImagesFromDirectoryPlugin;
                if (_savePlugin != null)
                {
                    _savePlugin.Initialize(_measureSchema);
                    break;
                }
            }
        }

        public string ImageDirectoryName { get; set; }

        public IObservable<StationResult> StationCompletedEvent => _stationCompletedEvent;

        public void TriggerWorkpieceLocatingReadyAll()
        {
            var allFileNames = Directory.GetFiles(ImageDirectoryName);
            var allFileCount = allFileNames.Length;
            var cycleCount = allFileCount / _measureSchema.MeasureDefinitions.Count;

            for (int i = 0; i < cycleCount; i++)
            {
                Debug.WriteLine("TriggerWorkpieceLocatingReady " + i + ", begin");
                TriggerWorkpieceLocatingReady(i* _measureSchema.MeasureDefinitions.Count);
                Debug.WriteLine("TriggerWorkpieceLocatingReady " + i + ", end");
            };
        }

        public void TriggerWorkpieceLocatingReady(int fileIndexOffset = 0)
        {
            List<MeasureResult> MeasureResults = new List<MeasureResult>();

           

            for (int i = 0; i < _measureSchema.MeasureDefinitions.Count; i++)
            {
                var definition = _measureSchema.MeasureDefinitions[i];


//                provider.HFramegrabber

                string fileName;

                if (ImageDirectoryName.IsNullOrEmpty())
                {
                    fileName = _savePlugin.GetImageFileName(fileIndexOffset+i);
                }
                else
                {
                    var fileNames = Directory.GetFiles(ImageDirectoryName);
                    fileName = fileNames[fileIndexOffset+i];
                }

                var camera = new HalconCamera
                {
                    OpenFramegrabber_Name = "File",
                    OpenFramegrabber_CameraType = fileName
                };
                camera.Init();

                var cameraDef = definition.Device as CameraHalconInspectorMeasureDevice;
                cameraDef.Calibrator = null;
                cameraDef.HFramegrabberProvider = new HFramegrabberProvider() {HFramegrabber = camera.HFramegrabber };
                cameraDef.Initialize();

                var measureData = definition.Device.Measure(new MeasureEvent(){});
                measureData.Index = definition.Index;

                MeasureResults.Add(measureData);

                camera.Dispose();
            }

            List<CalculateResult> calculateResults = new List<CalculateResult>();

            for (int i = 0; i < _measureSchema.CalculateDefinitions.Count; i++)
            {
                var definition = _measureSchema.CalculateDefinitions[i];
                var measureOutput = definition.CalculateOperation.Calculate(MeasureResults, definition);

                if (definition.TuneEnabled)
                {
                    measureOutput.Value = MeasureOutputValueTuner.Tune(
                        measureOutput.Value,
                        definition.StandardValue,
                        definition.ToleranceUpper,
                        definition.ToleranceLower,
                        definition.SystemOffsetValue);
                    measureOutput.IsTuned = true;
                }

                var calculateResult = new CalculateResult
                {
                    Index = i,
                    Definition = definition,
                    Output = measureOutput
                };
                calculateResults.Add(calculateResult);
            }

            var timeStamp = DateTime.Now;

            var stationResult = new StationResult()
            {
                StationIndex = _measureSchema.StationIndex,
                StationName = _measureSchema.StationName,
                StationDescription = _measureSchema.StationDescription,
                CalculateResults = calculateResults,
                MeasureResults = MeasureResults,
                MeasureTag = 0,
                WorkpieceTag = 0,
                MeasureDateTime = timeStamp,
            };


            _stationCompletedEvent.OnNext(stationResult);
        }
    }
}