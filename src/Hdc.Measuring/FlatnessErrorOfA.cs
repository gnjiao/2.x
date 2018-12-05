using System;

namespace Hdc.Measuring
{
    public static class FlatnessErrorOfA
    {
        public static void FlatnessErrorOfACalculate(double[] x, double[] y, double[] z, out double[] distance,
            out double f)
        {
            //public double[] x = {get;set};
            //public double[] y = {};
            //public double[] z = {};
            double[] A = new double[9]; //A为最小二乘线性方程组系数
            double[] B = new double[9]; //B为A的逆矩阵
            double[] C = new double[3]; //C[0],C[1],C[2]为平面方程参数：z=C[0]*x+C[1]*y+C[2]
            int nCount = x.Length;
            distance = new double[nCount]; //distance为点到平面的距离

            for (int k = 0; k < nCount; k++)
            {
                A[0] += x[k]*x[k];
                A[1] += x[k]*y[k];
                A[2] += x[k];
                A[3] += x[k]*y[k];
                A[4] += y[k]*y[k];
                A[5] += y[k];
                A[6] += x[k];
                A[7] += y[k];
                //A[8] = 9f;
                C[0] += x[k]*z[k];
                C[1] += y[k]*z[k];
                C[2] += z[k];
            }
            A[8] = nCount;

            //计算矩阵对应的行列式的代数余子式
            double output = A[0]*(A[4]*A[8] - A[5]*A[7]) - A[1]*(A[3]*A[8] - A[5]*A[6]) +
                            A[2]*(A[3]*A[7] - A[4]*A[6]);
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

            //求解平面方程参数a,b,c
            double a, b, c;
            a = B[0]*C[0] + B[1]*C[1] + B[2]*C[2];
            b = B[3]*C[0] + B[4]*C[1] + B[5]*C[2];
            c = B[6]*C[0] + B[7]*C[1] + B[8]*C[2];

            //点到平面的距离求解
            for (int num = 0; num < nCount; num++)
            {
                distance[num] = (float) ((a*x[num] + b*y[num] - z[num] + c)/Math.Sqrt(a*a + b*b + 1));
            }

            //求出最大最小误差，并输出平面误差度
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
            f = max - min;
        }

        public static void CalculateFlatFunction(double[] x, double[] y, double[] z, out double a, out double b,out double c)
        {
            //public double[] x = {get;set};
            //public double[] y = {};
            //public double[] z = {};
            double[] A = new double[9]; //A为最小二乘线性方程组系数
            double[] B = new double[9]; //B为A的逆矩阵
            double[] C = new double[3]; //C[0],C[1],C[2]为平面方程参数：z=C[0]*x+C[1]*y+C[2]
            int nCount = x.Length;

            for (int k = 0; k < nCount; k++)
            {
                A[0] += x[k] * x[k];
                A[1] += x[k] * y[k];
                A[2] += x[k];
                A[3] += x[k] * y[k];
                A[4] += y[k] * y[k];
                A[5] += y[k];
                A[6] += x[k];
                A[7] += y[k];
                //A[8] = 9f;
                C[0] += x[k] * z[k];
                C[1] += y[k] * z[k];
                C[2] += z[k];
            }
            A[8] = nCount;

            //计算矩阵对应的行列式的代数余子式
            double output = A[0] * (A[4] * A[8] - A[5] * A[7]) - A[1] * (A[3] * A[8] - A[5] * A[6]) +
                            A[2] * (A[3] * A[7] - A[4] * A[6]);
            //计算逆矩阵
            B[0] = (A[4] * A[8] - A[5] * A[7]) / output;
            B[1] = (A[2] * A[7] - A[1] * A[8]) / output;
            B[2] = (A[1] * A[5] - A[2] * A[4]) / output;
            B[3] = (A[5] * A[6] - A[3] * A[8]) / output;
            B[4] = (A[0] * A[8] - A[2] * A[6]) / output;
            B[5] = (A[2] * A[3] - A[0] * A[5]) / output;
            B[6] = (A[3] * A[7] - A[4] * A[6]) / output;
            B[7] = (A[1] * A[6] - A[0] * A[7]) / output;
            B[8] = (A[0] * A[4] - A[1] * A[3]) / output;

            //求解平面方程参数a,b,c
//            double a, b, c;
            a = B[0] * C[0] + B[1] * C[1] + B[2] * C[2];
            b = B[3] * C[0] + B[4] * C[1] + B[5] * C[2];
            c = B[6] * C[0] + B[7] * C[1] + B[8] * C[2];

//            return new FlatFunction(a, b, c);
        }
    }
}