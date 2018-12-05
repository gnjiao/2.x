using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Obsolete]
    [Browsable(false)]
    [Serializable]
    [ContentProperty(nameof(PointPositions))]
    public class FlatnessErrorOf8PointsCalculateOperation : ICalculateOperation
    {
        public MeasureOutput Calculate(IList<MeasureResult> measureResults, CalculateDefinition definition)
        {
            //            float[] x = new float[9] { 1, 1, 1, 2, 2, 2, 3, 3, 3 };
            //            float[] y = new float[9] { 1, 2, 3, 1, 2, 3, 1, 2, 3 };
            //            float[] z = new float[9] { 1, 2, 3, 3, 4, 5, 5, 6, 7 };

            double[] x = PointPositions.Select(p =>
            {
                if (Math.Abs(p.X) > 0.000001)
                    return p.X;

                return measureResults.Single(m => m.Index == p.MeasureResultIndex)
                    .MeasureEvent.X;
            }).ToArray();

            double[] y = PointPositions.Select(p =>
            {
                if (Math.Abs(p.Y) > 0.000001)
                    return p.Y;

                return measureResults.Single(m => m.Index == p.MeasureResultIndex)
                    .MeasureEvent.Y;
            }).ToArray();

            double[] z = PointPositions.Select(p => measureResults.Single(m => m.Index == p.MeasureResultIndex)
                .Outputs[p.MeasureOutputIndex].Value).ToArray();


            for (int i = 0; i < PointPositions.Count; i++)
            {
                Console.WriteLine($"FlatnessErrorOf8: Point{i}.X={x[i]}, Y={y[i]}, Z={z[i]}");
            }


            double[] A = new double[9]; //A为最小二乘线性方程组系数
            double[] B = new double[9]; //B为A的逆矩阵
            double[] C = new double[9]; //C[0],C[1],C[2]为平面方程参数：z=C[0]*x+C[1]*y+C[2]
            double[] distance = new double[9]; //distance为点到平面的距离
            A[0] = x[0]*x[0] + x[1]*x[1] + x[2]*x[2] + x[3]*x[3] + x[4]*x[4] + x[5]*x[5] + x[6]*x[6] + x[7]*x[7];
                // + x[8] * x[8];
            A[1] = x[0]*y[0] + x[1]*y[1] + x[2]*y[2] + x[3]*y[3] + x[4]*y[4] + x[5]*y[5] + x[6]*y[6] + x[7]*y[7];
                // + x[8] * y[8];
            A[2] = x[0] + x[1] + x[2] + x[3] + x[4] + x[5] + x[6] + x[7]; // + x[8];
            A[3] = x[0]*y[0] + x[1]*y[1] + x[2]*y[2] + x[3]*y[3] + x[4]*y[4] + x[5]*y[5] + x[6]*y[6] + x[7]*y[7];
                // + x[8] * y[8];
            A[4] = y[0]*y[0] + y[1]*y[1] + y[2]*y[2] + y[3]*y[3] + y[4]*y[4] + y[5]*y[5] + y[6]*y[6] + y[7]*y[7];
                // + y[8] * y[8];
            A[5] = y[0] + y[1] + y[2] + y[3] + y[4] + y[5] + y[6] + y[7]; // + y[8];
            A[6] = x[0] + x[1] + x[2] + x[3] + x[4] + x[5] + x[6] + x[7]; // + x[8];
            A[7] = y[0] + y[1] + y[2] + y[3] + y[4] + y[5] + y[6] + y[7]; // + y[8];
            A[8] = 8f;
            //A矩阵输出
            Debug.WriteLine(A[0]);
            Debug.WriteLine(A[1]);
            Debug.WriteLine(A[2]);
            Debug.WriteLine(A[3]);
            Debug.WriteLine(A[4]);
            Debug.WriteLine(A[5]);
            Debug.WriteLine(A[6]);
            Debug.WriteLine(A[7]);
            Debug.WriteLine(A[8]);

            //********************
            C[0] = (x[0]*z[0]) + (x[1]*z[1]) + (x[2]*z[2]) + (x[3]*z[3]) + (x[4]*z[4]) + (x[5]*z[5]) + (x[6]*z[6]) +
                   (x[7]*z[7]); // + (x[8] * z[8]);
            C[1] = (y[0]*z[0]) + (y[1]*z[1]) + (y[2]*z[2]) + (y[3]*z[3]) + (y[4]*z[4]) + (y[5]*z[5]) + (y[6]*z[6]) +
                   (y[7]*z[7]); //+ (y[8] * z[8]);
            C[2] = z[0] + z[1] + z[2] + z[3] + z[4] + z[5] + z[6] + z[7]; // + z[8];
            Debug.WriteLine(C[0]);
            Debug.WriteLine(C[1]);
            Debug.WriteLine(C[2]);
            //计算矩阵对应的行列式的代数余子式
            double output = A[0]*(A[4]*A[8] - A[5]*A[7]) - A[1]*(A[3]*A[8] - A[5]*A[6]) + A[2]*(A[3]*A[7] - A[4]*A[6]);
            Debug.WriteLine(output);
            //计算逆矩阵
            B[0] = (A[4]*A[8] - A[5]*A[7])/output;
            B[1] = (A[2]*A[7] - A[1]*A[8])/output;
            B[2] = (A[1]*A[5] - A[2]*A[4])/output;
            B[3] = (A[5]*A[6] - A[3]*A[8])/output;
            B[4] = (A[0]*A[8] - A[2]*A[6])/output;
            B[5] = (A[2]*A[3] - A[0]*A[5])/output;
            B[6] = (A[3]*A[7] - A[4]*A[6])/output;
            B[7] = (A[1]*A[6] - A[0]*A[7])/output;
            B[8] = (A[0]*A[4] - A[1]*A[3])/output;

            Debug.WriteLine(B[0]);
            Debug.WriteLine(B[1]);
            Debug.WriteLine(B[2]);
            Debug.WriteLine(B[3]);
            Debug.WriteLine(B[4]);
            Debug.WriteLine(B[5]);
            Debug.WriteLine(B[6]);
            Debug.WriteLine(B[7]);
            Debug.WriteLine(B[8]);

            //求解平面方程参数a,b,c
            double a, b, c;
            a = B[0]*C[0] + B[1]*C[1] + B[2]*C[2];
            b = B[3]*C[0] + B[4]*C[1] + B[5]*C[2];
            c = B[6]*C[0] + B[7]*C[1] + B[8]*C[2];
            Debug.WriteLine(a);
            Debug.WriteLine(b);
            Debug.WriteLine(c);
            //点到平面的距离求解
            for (int num = 0; num < 8; num++)
            {
                //distance[num] = (float) (Math.Abs(a*x[num] + b*y[num] - z[num] + c)/Math.Sqrt(a*a + b*b + 1));
                distance[num] = (float)((a * x[num] + b * y[num] - z[num] + c) / Math.Sqrt(a * a + b * b + 1));
                Debug.WriteLine(distance[num]);
            }


            //求出最大最小误差，并输出平面误差度
            double max = distance[0], min = distance[0];
            double f;
            for (int num1 = 0; num1 < 8; num1++)
            {
                if (distance[num1] >= max)
                {
                    max = distance[num1];
                }
            }
            for (int num2 = 0; num2 < 8; num2++)
            {
                if (distance[num2] <= min)
                {
                    min = distance[num2];
                }
            }
            f = max - min;

            //return new MeasureOutput() { Value = distance[OutputPointIndex] };

            Console.WriteLine($"FlatnessErrorOf8: Result={f}");
            return new MeasureOutput() {Value = f};
        }


        public Collection<PointPosition> PointPositions { get; set; } = new Collection<PointPosition>();

        public int OutputPointIndex { get; set; }
    }
}