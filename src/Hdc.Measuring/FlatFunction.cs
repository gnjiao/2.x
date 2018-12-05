using System;

namespace Hdc.Measuring
{
    public class FlatFunction
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        public FlatFunction()
        {
        }

        public FlatFunction(double a, double b, double c)
        {
            A = a;
            B = b;
            C = c;
        }

        public double GetDistance(double x, double y, double z)
        {
            var distance = (float)((A * x + B * y - z + C) / Math.Sqrt(A * A + B * B + 1));
            return distance;
        }

        public double GetFlatnessError(double[] x, double[] y, double[] z, out double[] distance)
        {
            int nCount = x.Length;
            distance = new double[nCount]; //distanceΪ�㵽ƽ��ľ���

            //�㵽ƽ��ľ������
            for (int num = 0; num < nCount; num++)
            {
                distance[num] = (float)((A * x[num] + B * y[num] - z[num] + C) / Math.Sqrt(A * A + B * B + 1));
            }

            //��������С�������ƽ������
            double max = distance[0], min = distance[0];
            //double f;
            for (int num1 = 0; num1 < nCount; num1++)
            {
                if (distance[num1] >= max)
                {
                    max = distance[num1];
                }
            }
            for (int num2 = 0; num2 < nCount; num2++)
            {
                if (distance[num2] <= min)
                {
                    min = distance[num2];
                }
            }
            var f = max - min;
            return f;
        }

        public void CalculateFlatFunction(double[] x, double[] y, double[] z)
        {
            double a, b, c;
            FlatnessErrorOfA.CalculateFlatFunction(x, y, z, out a, out b, out c);

            A = a;
            B = b;
            C = c;

            double[] distances;
            FlatnessError = GetFlatnessError(x, y, z, out distances);
        }

        public double FlatnessError { get; set; }
    }
}