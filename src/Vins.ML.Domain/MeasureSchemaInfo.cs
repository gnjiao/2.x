using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Vins.ML.Domain
{
    [Serializable]
    [ContentProperty(nameof(FileNameReferences))]
    public class MeasureSchemaInfo
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public string SourceMeasureSchemaFileName { get; set; }
        public string TargetMeasureSchemaFileName { get; set; }

        public Collection<FileNameReference> FileNameReferences { get; set; } = new Collection<FileNameReference>();
    }
}