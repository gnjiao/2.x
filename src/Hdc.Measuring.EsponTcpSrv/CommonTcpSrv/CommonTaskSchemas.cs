using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("CommonTaskSchema")]
    public class CommonTaskSchemas
    {
        public int StationIndex { get; set; }
        public Collection<CommonTaskSchema> CommonTaskSchema { get; set; } = new Collection<CommonTaskSchema>();

    }
}