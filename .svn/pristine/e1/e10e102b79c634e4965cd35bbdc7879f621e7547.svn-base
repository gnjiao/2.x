using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty(nameof(PointPositions))]
    public class SliderPositionTestCalculateOperation : ICalculationOperationPlus
    {
        public IEnumerable<MeasureOutput> Calculate(IList<MeasureResult> measureResults)
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

            /*
            for (int i = 0; i < PointPositions.Count; i++)
            {
                Console.WriteLine($"FlatnessErrorOf8: Point{i}.X={x[i]}, Y={y[i]}, Z={z[i]}");
            }
            */

            double[] A = new double[9];
            double[] B = new double[9];
            double[] C = new double[3];

            double[] A1 = new double[9];
            double[] B1 = new double[9];
            double[] C1 = new double[3];

            
            A[0] = x[0]*x[0] + x[1]*x[1] + x[2]*x[2] + x[3]*x[3]
                   + x[8]*x[8] + x[9]*x[9] + x[10]*x[10] + x[11]*x[11];
            A[1] = x[0] + x[1] + x[2] + x[3];
            A[2] = x[8] + x[9] + x[10] + x[11];
            A[3] = x[0] + x[1] + x[2] + x[3];
            A[4] = 4;
            A[5] = 0;
            A[6] = x[8] + x[9] + x[10] + x[11];
            A[7] = 0;
            A[8] = 4;
            
            /*
            A[0] = (x[0]+z[0]) * (x[0] + z[0]) + (x[1]+z[1]) * (x[1] + z[1]) + (x[2]+z[2]) * (x[2] + z[2]) + (x[3]+z[3]) * (x[3] + z[3])
                   + (x[8]+z[8]) * (x[8] + z[8]) + (x[9]+z[9]) * (x[9] + z[9]) + (x[10]+z[10]) * (x[10] + z[10]) + (x[11]+z[11]) * (x[11] + z[11]);
            A[1] = x[0] + z[0] + x[1] + z[1] + x[2] + z[2] + x[3] + z[3];
            A[2] = x[8] + z[8] + x[9] + z[9] + x[10] + z[10] + x[11] + z[11];
            A[3] = x[0] + z[0] + x[1] + z[1] + x[2] + z[2] + x[3] + z[3];
            A[4] = 4;
            A[5] = 0;
            A[6] = x[8] + z[8] + x[9] + z[9] + x[10] + z[10] + x[11] + z[11];
            A[7] = 0;
            A[8] = 4;
            */
            
            A1[0] = (x[4] + z[4])*(x[4] + z[4]) + (x[5] + z[5])*(x[5] + z[5]) + (x[6] + z[6])*(x[6] + z[6]) +
                    (x[7] + z[7])*(x[7] + z[7])
                    + (x[12] + z[12])*(x[12] + z[12]) + (x[13] + z[13])*(x[13] + z[13]) +
                    (x[14] + z[14])*(x[14] + z[14]) + (x[15] + z[15])*(x[15] + z[15]);
            A1[1] = x[4] + z[4] + x[5] + z[5] + x[6] + z[6] + x[7] + z[7];
            A1[2] = x[12] + z[12] + x[13] + z[13] + x[14] + z[14] + x[15] + z[15];
            A1[3] = x[4] + z[4] + x[5] + z[5] + x[6] + z[6] + x[7] + z[7];
            A1[4] = 4;
            A1[5] = 0;
            A1[6] = x[12] + z[12] + x[13] + z[13] + x[14] + z[14] + x[15] + z[15];
            A1[7] = 0;
            A1[8] = 4;
            
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
            Debug.WriteLine(A1[0]);
            Debug.WriteLine(A1[1]);
            Debug.WriteLine(A1[2]);
            Debug.WriteLine(A1[3]);
            Debug.WriteLine(A1[4]);
            Debug.WriteLine(A1[5]);
            Debug.WriteLine(A1[6]);
            Debug.WriteLine(A1[7]);
            Debug.WriteLine(A1[8]);

            //********************
            C[0] = (x[0]*(y[0] + z[0])) + (x[1]*(y[1] + z[1])) + (x[2]*(y[2] + z[2])) + (x[3]*(y[3] + z[3]))
                   + (x[8]*(y[8] + z[8])) + (x[9]*(y[9] + z[9])) + (x[10]*(y[10] + z[10])) + (x[11]*(y[11] + z[11]));
            C[1] = y[0] + z[0] + y[1] + z[1] + y[2] + z[2] + y[3] + z[3];
            C[2] = y[8] + z[8] + y[9] + z[9] + y[10] + z[10] + y[11] + z[11];

            C1[0] = (x[4]*y[4]) + (x[5]*y[5]) + (x[6]*y[6]) + (x[7]*y[7]) + (x[12]*y[12]) + (x[13]*y[13]) +
                    (x[14]*y[14]) + (x[15]*y[15]);
            C1[1] = y[4] + y[5] + y[6] + y[7];
            C1[2] = y[12] + y[13] + y[14] + y[15];

            Debug.WriteLine(C[0]);
            Debug.WriteLine(C[1]);
            Debug.WriteLine(C[2]);
            Debug.WriteLine(C1[0]);
            Debug.WriteLine(C1[1]);
            Debug.WriteLine(C1[2]);

            //计算矩阵对应的行列式的代数余子式
            double output = A[0]*(A[4]*A[8] - A[5]*A[7]) - A[1]*(A[3]*A[8] - A[5]*A[6]) + A[2]*(A[3]*A[7] - A[4]*A[6]);
            double output1 = A1[0]*(A1[4]*A1[8] - A1[5]*A1[7]) - A1[1]*(A1[3]*A1[8] - A1[5]*A1[6]) +
                             A1[2]*(A1[3]*A1[7] - A1[4]*A1[6]);
            Debug.WriteLine(output);
            Debug.WriteLine(output1);

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
            B1[0] = (A1[4]*A1[8] - A1[5]*A1[7])/output1;
            B1[1] = (A1[2]*A1[7] - A1[1]*A1[8])/output1;
            B1[2] = (A1[1]*A1[5] - A1[2]*A1[4])/output1;
            B1[3] = (A1[5]*A1[6] - A1[3]*A1[8])/output1;
            B1[4] = (A1[0]*A1[8] - A1[2]*A1[6])/output1;
            B1[5] = (A1[2]*A1[3] - A1[0]*A1[5])/output1;
            B1[6] = (A1[3]*A1[7] - A1[4]*A1[6])/output1;
            B1[7] = (A1[1]*A1[6] - A1[0]*A1[7])/output1;
            B1[8] = (A1[0]*A1[4] - A1[1]*A1[3])/output1;
            Debug.WriteLine(B[0]);
            Debug.WriteLine(B[1]);
            Debug.WriteLine(B[2]);
            Debug.WriteLine(B[3]);
            Debug.WriteLine(B[4]);
            Debug.WriteLine(B[5]);
            Debug.WriteLine(B[6]);
            Debug.WriteLine(B[7]);
            Debug.WriteLine(B[8]);
            Debug.WriteLine(B1[0]);
            Debug.WriteLine(B1[1]);
            Debug.WriteLine(B1[2]);
            Debug.WriteLine(B1[3]);
            Debug.WriteLine(B1[4]);
            Debug.WriteLine(B1[5]);
            Debug.WriteLine(B1[6]);
            Debug.WriteLine(B1[7]);
            Debug.WriteLine(B1[8]);


            //求解平面方程参数a,b,c
            double a, b, c;
            double a1, b1, c1;
            a = B[0]*C[0] + B[1]*C[1] + B[2]*C[2];
            b = B[3]*C[0] + B[4]*C[1] + B[5]*C[2];
            c = B[6]*C[0] + B[7]*C[1] + B[8]*C[2];
            a1 = B1[0]*C1[0] + B1[1]*C1[1] + B1[2]*C1[2];
            b1 = B1[3]*C1[0] + B1[4]*C1[1] + B1[5]*C1[2];
            c1 = B1[6]*C1[0] + B1[7]*C1[1] + B1[8]*C1[2];

            //求解两对平行线中心线的交点
            double pointx;
            double pointy;
            pointx = (b1 + c1 - b - c)/(2*(a - a1));
            pointy = a*pointx + (b + c)/2;


            var outputs = new List<MeasureOutput>()
            {
                new MeasureOutput()
                {
                    Name = "Slider.X",
                    Value = pointx,
                },
                new MeasureOutput()
                {
                    Name = "Slider.Y",
                    Value = pointy,
                },
                new MeasureOutput()
                {
                    Name = "Slider.KLong",
                    Value = a,
                },
                new MeasureOutput()
                {
                    Name = "Slider.KShort",
                    Value = a1,
                },
                new MeasureOutput()
                {
                    Name = "a",
                    Value = a,
                },
                new MeasureOutput()
                {
                    Name = "b",
                    Value = b,
                },
                new MeasureOutput()
                {
                    Name = "c",
                    Value = c,
                },
                new MeasureOutput()
                {
                    Name = "a1",
                    Value = a1,
                },
                new MeasureOutput()
                {
                    Name = "b1",
                    Value = b1,
                },
                new MeasureOutput()
                {
                    Name = "c1",
                    Value = c1,
                },
            };

            return outputs;
        }


        public Collection<PointPosition> PointPositions { get; set; } = new Collection<PointPosition>();

        public int OutputPointIndex { get; set; }

//        public int PointTop0_X_MeasureIndex { get; set; }
//        public int PointTop0_X_OutputIndex { get; set; }
//
//        public int PointTop0_Y_MeasureIndex { get; set; }
//        public int PointTop0_Y_OutputIndex { get; set; }
//
//        public int PointTop0_X_MeasureIndex { get; set; }
//        public int PointTop0_X_OutputIndex { get; set; }
//
//        public int PointTop0_X_MeasureIndex { get; set; }
//        public int PointTop0_X_OutputIndex { get; set; }
    }
}