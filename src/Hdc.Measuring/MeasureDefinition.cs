using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("Device")]
    public class MeasureDefinition
    {
        public int Index { get; set; }

        public string DeviceName { get; set; }

        public int PointIndex { get; set; }

        public IMeasureDevice Device { get; set; }

        public Collection<IMeasureTrigger> Triggers { get; set; } = new Collection<IMeasureTrigger>();
    }
}