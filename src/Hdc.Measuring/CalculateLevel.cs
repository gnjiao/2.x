using System;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
    public class CalculateLevel
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public bool Enabled { get; set; } = true;
        public string Name { get; set; }
    }
}