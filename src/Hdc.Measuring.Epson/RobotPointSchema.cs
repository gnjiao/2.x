using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using Hdc.Mv.RobotVision;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("RobotPoints")]
    public class RobotPointSchema
    {
        public int Index { get; set; }

        public int StationIndex { get; set; }

        public Collection<RobotPoint> RobotPoints { get; set; } = new Collection<RobotPoint>();
    }
}