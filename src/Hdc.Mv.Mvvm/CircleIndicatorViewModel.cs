using System.Windows.Media;

namespace Hdc.Mv.Mvvm
{
    public class CircleIndicatorViewModel
    {
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Radius { get; set; }
        public bool DisplayEnabled { get; set; } = true;

        public CircleIndicatorViewModel()
        {
        }

        public CircleIndicatorViewModel(double centerX, double centerY, double radius)
        {
            CenterX = centerX;
            CenterY = centerY;
            Radius = radius;
        }
    }
}