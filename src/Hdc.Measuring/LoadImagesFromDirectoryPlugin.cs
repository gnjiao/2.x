using System;
using System.IO;

namespace Hdc.Measuring
{
    [Serializable]
    public class LoadImagesFromDirectoryPlugin : IMeasureSchemaPlugin
    {
        private string[] _fileNames;

        public void Initialize(MeasureSchema measureSchema)
        {
            if (Directory.Exists(ImageDirectory))
            {
                _fileNames = Directory.GetFiles(ImageDirectory);
            }
        }

        public string ImageDirectory { get; set; }

        public string GetImageFileName(int index)
        {
            return _fileNames[index];
        }
    }
}