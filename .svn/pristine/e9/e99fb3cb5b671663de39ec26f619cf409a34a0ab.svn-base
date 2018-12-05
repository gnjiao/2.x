

using FocalSpec.FsApiNet.Model;

namespace Hdc.Measuring
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    class CoverGlassCalculator
    {
        private List<Profile> _profiles = new List<Profile>();

        public double MeasuredAngle = 0;

        class LineFormula
        {
            public double A;
            public double B;
            public double C;

            public double Distance(FsApi.Point point)
            {
                double dist = (point.X * this.A) + (point.Y * this.B) + this.C;

                if (dist < 0)
                {
                    return -1.0 * dist;
                }

                return dist;
            }
        }

        private bool FitAngle(List<FsApi.Point> edgePoints, double distanceBetweenLines)
        {
            double A, B;  
            double X = 0, Y = 0, XY = 0, X2 = 0, Y2 = 0;

            double points = 0;
            foreach (var point in edgePoints)
            {
                var y = distanceBetweenLines * points;
                X += point.X;
                Y += y;
                XY += point.X * y;
                X2 += point.X * point.X;
                Y2 += y * y;
                points += 1.0;
            }

            X /= points;
            Y /= points;
            XY /= points;
            X2 /= points;
            Y2 /= points;

            A = -(XY - (X * Y)); 

            var Bx = X2 - (X * X);
            var By = Y2 - (Y * Y);

            if (Math.Abs(Bx) < Math.Abs(By)) 
            {
                // vertical
                B = A;
                A = By;
            }
            else
            {
                // horizontal
                B = Bx;
            }

            this.MeasuredAngle = Math.Atan2(B, A);
            return true;
        }

        private bool FitLine(List<FsApi.Point> inputPoints, out LineFormula lineFormula)
        {
            lineFormula = new LineFormula();

            if (inputPoints.Count < 2)
            {
                lineFormula.A = 0;
                lineFormula.B = 0;
                lineFormula.C = 0;
                return false;
            }

            double X = 0, Y = 0, XY = 0, X2 = 0, Y2 = 0;

            double points = 0;
            foreach (var point in inputPoints)
            {
                var y = point.Y;
                X += point.X;
                Y += y;
                XY += point.X * y;
                X2 += point.X * point.X;
                Y2 += y * y;
                points += 1.0;
            }

            X /= points;
            Y /= points;
            XY /= points;
            X2 /= points;
            Y2 /= points;

            lineFormula.A = -(XY - (X * Y));

            var Bx = X2 - (X * X);
            var By = Y2 - (Y * Y);

            if (Math.Abs(Bx) < Math.Abs(By))
            {
                // vertical
                lineFormula.B = lineFormula.A;
                lineFormula.A = By;
            }
            else
            {
                // horizontal
                lineFormula.B = Bx;
            }
            lineFormula.C = -(lineFormula.A * X + lineFormula.B * Y);

            double D = Math.Sqrt(lineFormula.A * lineFormula.A + lineFormula.B * lineFormula.B);

            lineFormula.A /= D;
            lineFormula.B /= D;
            lineFormula.C /= D;
            return true;
        }

        private FsApi.Point FindEndPoint(Profile profile, float maxGap)
        {
            var prevPoint = profile.Points[0];
            foreach (var point in profile.Points)
            {
                if ((point.X - prevPoint.X) > maxGap)
                {
                    return prevPoint;
                }

                prevPoint = point;
            }

            return prevPoint;
        }

        private FsApi.Point FindInflectionPoint(Profile profile,float straightLineLength, float maxDistance)
        {
            List<FsApi.Point> inputPoints = new List<FsApi.Point>();

            // fit line for first points
            LineFormula lineFormula = null;
            foreach (var point in profile.Points)
            {
                if (point.X > straightLineLength)
                {
                    if (!this.FitLine(inputPoints, out lineFormula))
                    {
                        return new FsApi.Point { X = 0, Y = 0 };
                    }

                    break;
                }

                inputPoints.Add(point);
            }

            if (lineFormula != null)
            {
                List<double> distances = new List<double>();

                foreach (var point in profile.Points)
                {
                    if (point.X < straightLineLength)
                    {
                        continue;
                    }

                    var distance = lineFormula.Distance(point);
                    if (distance < 1)
                    {
                        continue;
                    }

                    distances.Add(distance);

                    if (distances.Average() > maxDistance)
                    {
                        return point;
                    }
                    if (distances.Count() > 10)
                    {
                        distances.RemoveAt(0); 
                    }
                }
            }

            return new FsApi.Point { X = 0, Y = 0 };
        }

        public bool MeasureAngle(Profile profile, double distanceBetweenLines,bool useInflectionPoint)
        {
            if (profile.Points.Count == 0)
            {
                return false;
            }

            this._profiles.Add(profile);

            if (this._profiles.Count > 1)
            {
                float maxGap = 20; // 20 um gap is allowed

                List<FsApi.Point> edgePoints = new List<FsApi.Point>();
                foreach (var p in this._profiles)
                {
                    if (useInflectionPoint)
                    {
                        // 500 micrometers are used for straight line fitting at the beginning
                        const float StraightLineLength = 500;

                        // Inflection point is found if averaged distance from the fitted line is more than 5 micrometers
                        const float MaxDistance = 5;

                        edgePoints.Add(this.FindInflectionPoint(p, StraightLineLength, MaxDistance));
                    }
                    else
                    {
                        edgePoints.Add(this.FindEndPoint(p, maxGap));
                    }
                }

                if (edgePoints.Count > 1)
                {
                    double y = distanceBetweenLines * (double)(edgePoints.Count - 1);
                    double x = (double)(edgePoints[0].X - edgePoints[edgePoints.Count - 1].X);
                    this.MeasuredAngle = Math.Atan2(x, y);
                    if (!this.FitAngle(edgePoints, distanceBetweenLines))
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }

                var profileIndex = 0;
                foreach (var p in this._profiles)
                {
                    var adjustedPoints = new List<FsApi.Point>();
                    var edge = (float)((double)edgePoints[profileIndex].X * Math.Cos(this.MeasuredAngle));

                    foreach (var point in p.Points)
                    {
                        if (!useInflectionPoint && (edge + 0.1) < point.X)
                        {
                            continue;
                        }

                        adjustedPoints.Add(new FsApi.Point()
                        {
                            X = -1 * (edge -
                                (float)((double)point.X * Math.Cos(
                                            this.MeasuredAngle))),
                            Y = point.Y,
                            Intensity = point.Intensity
                        });
                    }

                    this.ExportProfile(profileIndex, adjustedPoints);
                    profileIndex++;
                }

                this._profiles.Clear();

                return true;
            }
            return false;
        }

        private void ExportProfile(int profileIndex, List<FsApi.Point> points)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("X [mm];Z [mm]");

            foreach (var point in points)
            {
                str.AppendLine($"{point.X};{point.Y}");
            }

            File.WriteAllText("Export" + profileIndex + ".csv", str.ToString());
        }

        public bool TestFunction()
        {
            bool useInflectionPoint = true;
            {
                Profile profile;
                profile = this.ReadProfile(
                    "C:\\Users\\masa.makarainen\\Documents\\Customers\\HDC\\20170920_BYD_HDC\\2017-09-20_10.59.48_00000000_0.csv");
                this.MeasureAngle(profile, 5000, useInflectionPoint);

                profile = this.ReadProfile(
                    "C:\\Users\\masa.makarainen\\Documents\\Customers\\HDC\\20170920_BYD_HDC\\2017-09-20_10.59.52_00000001_0.csv");

                this.MeasureAngle(profile, 5000, useInflectionPoint);

            }

            return true;
        }

        private Profile ReadProfile(string filePath)
        {
            IList<FsApi.Point> points = new List<FsApi.Point>();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                string[] columns = line.Split(';');
                float x, y;
                if (columns.Length < 3 || 
                    !float.TryParse(columns[0], out x) ||
                    !float.TryParse(columns[1], out y))
                {
                    continue;
                }

                var point = new FsApi.Point()
                {
                    Intensity = 0,
                    X = x * 1000,
                    Y = y * 1000
                };
                points.Add(point);
            }
            FsApi.Header notused = new FsApi.Header();
            return new Profile(points, notused);           
        }
    }

}
