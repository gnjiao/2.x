using System.Collections.Generic;
using FocalSpec.FsApiNet.Model;

namespace Hdc.Measuring
{
    using System.Linq;

    public class Profile
    {
        public IList<FsApi.Point> Points { get; set; }

        public FsApi.Header Header { get; set; }

        public float AverageIntensity { get; set; }

        public Profile(FsApi.Point[] points,int pointCount, FsApi.Header header)
        {
            this.Points = new List<FsApi.Point>();
            for (int i = 0; i < pointCount; i++)
            {
                FsApi.Point p = new FsApi.Point();
                p = points[i];
                this.Points.Add(p);
            }

            Header = header;
            AverageIntensity = 0.0f;
        }

        public Profile(IList<FsApi.Point> points, FsApi.Header header)
        {
            Points = points;
            Header = header;
            AverageIntensity = 0.0f;
        }

        public Profile(Profile profile)
        {
            Points = profile.Points;
            Header = profile.Header;
            AverageIntensity = profile.AverageIntensity;
        }

        public Profile(float[] zValues, float[] intensityValues, int lineLength,double xStep, FsApi.Header header)
        {
            this.Points = new List<FsApi.Point>();
            float no_meas = FsApi.NoMeasurement - 1;
            for (int i = 0; i < lineLength; i++)
            {
                if (zValues[i] > no_meas)
                {
                    continue;
                }

                FsApi.Point p = new FsApi.Point()
                {
                    Y = zValues[i],
                    X = (float)(i * xStep),
                    Intensity = intensityValues[i]
                };
                this.Points.Add(p);
            }

            this.Header = header;
            if (this.Points.Count!=0)
                this.AverageIntensity = this.Points.Average(p => p.Intensity);

        }
    }
}
