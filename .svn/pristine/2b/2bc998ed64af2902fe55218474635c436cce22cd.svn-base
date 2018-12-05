using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using HalconDotNet;
using Hdc.Mv.Halcon;
using Hdc.Mv.ImageAcquisition;
using Hdc.Mv.Inspection;

namespace Hdc.Measuring
{
    [Serializable]
    public class SimCameraHalconInspectorMeasureDevice : IHalconInspectorMeasureDevice
    {
        private readonly List<string> _fileNames = new List<string>();
        private int _index = 0;
        private int _tag;
        private bool _isInitialized;

        public void Initialize()
        {
            var fileNames = Directory.GetFiles(SampleImageDirectory);
            var imageFileNames = fileNames.Where(x =>
                x.EndsWith(".tif") ||
                x.EndsWith(".bmp") ||
                x.EndsWith(".jpg") ||
                x.EndsWith(".png"));
            _fileNames.AddRange(imageFileNames);

            _isInitialized = true;
        }

        public bool IsInitialized => _isInitialized;

        public MeasureResult Measure(MeasureEvent measureEvent)
        {
            var fn = _fileNames[_index];
            _index++;
            _tag++;
            if (_index == _fileNames.Count)
                _index = 0;

            var hi = new HImage();
            hi.ReadImage(fn);

            var inspectionResult = Inspector.Inspect(hi);
            hi.Dispose();

            MeasureResult md = new MeasureResult()
            {
                Tag = _tag,
                MeasureDateTime = DateTime.Now,
            };

            foreach (var mapEntry in MeasureDataMap.MeasureDataMapEntries)
            {
                var output = mapEntry.GetMeasureData(inspectionResult);
                md.Outputs.Add(output);
            }

            return md;
        }

        public string SampleImageDirectory { get; set; }

        public IHalconInspector Inspector { get; set; }
        public Collection<IMeasureDataMapEntry> MeasureDataMapEntries { get; set; }
        public MeasureDataMap MeasureDataMap { get; set; }

        public int Index { get; set; }
    }
}