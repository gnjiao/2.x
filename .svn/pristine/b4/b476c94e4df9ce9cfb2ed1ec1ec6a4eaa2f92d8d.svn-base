using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hdc.Measuring
{
    public static class LineConfocalWhiteLightProcess
    {
        public static void LineConfocalWhiteLightProcessCulate(List<double> x, List<double> y, string standardConfigDataPath, ref double result)
        {
            try
            {
                List<double> standardConfigX = new List<double>();
                List<double> standardConfigY = new List<double>();
                List<double> measureConfigX = new List<double>();
                List<double> measureConfigY = new List<double>();

                #region

                List<double> excelX = new List<double>();
                List<double> excelY = new List<double>();
                List<double> excelZ = new List<double>();
                List<double> excelXorg = new List<double>();
                List<double> excelYorg = new List<double>();
                List<double> zStandardBase = new List<double>();

                FileStream fileStream = null;
                StreamReader streamReader = null;
                try
                {
                    fileStream = File.Open(standardConfigDataPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    streamReader = new StreamReader(fileStream, Encoding.UTF8);
                    while (!streamReader.EndOfStream)
                    {
                        var str = streamReader.ReadLine();
                        if (str != null)
                        {
                            var xu = str.Split(',');
                            var xGet = xu[0];
                            var yGet = xu[1]; //实际Z轴高度
                            //var zGet = xu[2];//接受光强度
                            excelXorg.Add(Convert.ToDouble(xGet));
                            standardConfigX.Add(Convert.ToDouble(xGet));
                            excelYorg.Add(Convert.ToDouble(yGet));
                            //zStandardBase.Add(Convert.ToDouble(yGet));
                            standardConfigY.Add(Convert.ToDouble(yGet));
                            //excelZ.Add(Convert.ToDouble(zGet));
                        }
                        else
                        {
                            string s = "Please Make Sure The File Is Not Empty !";
                        }
                    }
                    SlectLength(x, y, excelXorg, excelYorg, ref excelX, ref excelY);
                }
                finally
                {
                    streamReader?.Close();
                    fileStream?.Close();
                }

                #endregion

                #region

                zStandardBase = excelX.ToList();
                standardConfigY = excelX.ToList();
                var numup = excelX.FindIndex(t => t > (excelX[0] + 400));
                var fitlineStandardX = excelX.Take(numup).ToArray();
                var fitlineStandardY = excelY.Take(numup).ToArray();
                double aStandard = 0, bStandard = 0, angleStandard = 0;
                LeastSquareMethodFitBaseLine(fitlineStandardX, fitlineStandardY, ref aStandard, ref bStandard,
                    ref angleStandard);
                for (var i = 0; i < excelX.Count; i++)
                {
                    zStandardBase[i] = aStandard*excelX[i] + bStandard;
                    //ZReal[i] = ZBase[i] - Y[i];
                    standardConfigY[i] = Math.Abs(aStandard*excelX[i] + bStandard - excelY[i])/
                                         Math.Sqrt(aStandard*aStandard + 1);
                }
                var pase1 = new List<double>();
                var pase2 = new List<double>();
                if (Math.Cos(angleStandard) > 0.0000001)
                {
                    for (var i = 0; i < excelX.Count; i++)
                    {
                        standardConfigX[i] = standardConfigY[i]*Math.Tan(angleStandard)*Math.Cos(angleStandard) +
                                             excelX[i];
                        pase1.Add(standardConfigX[i]);
                        pase2.Add(standardConfigX[i]);
                    }
                    //pase2.Remove(X.Count - 1);
                    for (var i = 0; i < excelX.Count - 1; i++)
                    {
                        pase2[i] = (pase1[i + 1] - pase1[i])/Math.Cos(angleStandard);

                    }
                    pase2.Reverse();
                    pase2[0] = 0;

                    for (var i = 0; i < excelX.Count; i++)
                    {
                        double xvalue = 0;
                        for (var j = 0; j < i + 1; j++)
                        {
                            xvalue = xvalue + pase2[j];
                        }
                        standardConfigX[i] = xvalue;
                    }
                }
                else
                {
                    for (var i = 0; i < excelX.Count; i++)
                    {
                        standardConfigX[i] = -(standardConfigY[i]*Math.Tan(angleStandard)*Math.Cos(angleStandard)) +
                                             excelX[i];
                        pase1.Add(standardConfigX[i]);
                        pase2.Add(standardConfigX[i]);
                    }
                    for (var i = 0; i < excelX.Count - 1; i++)
                    {
                        pase2[i] = (pase1[i + 1] - pase1[i])/Math.Cos(angleStandard);
                    }
                    pase2.Reverse();
                    pase2[0] = 0;
                    for (var i = 0; i < excelX.Count - 1; i++)
                    {
                        double xvalue = 0;
                        for (var j = 0; j < i + 1; j++)
                        {
                            xvalue = xvalue + pase2[j];
                        }
                        standardConfigX[i] = xvalue;
                    }
                }
                standardConfigY.Reverse();

                #endregion

                #region

                var zMeasureBase = x.ToList();
                measureConfigY = x.ToList();
                measureConfigX = x.ToList();
                var numdown = x.FindIndex(t2 => t2 > x[0] + 400);
                var fitlineMeasureX = x.Take(numdown).ToArray();
                var fitlineMeasureY = y.Take(numdown).ToArray();
                double aMeasure = 0, bMeasure = 0, angleMeasure = 0;
                LeastSquareMethodFitBaseLine(fitlineMeasureX, fitlineMeasureY, ref aMeasure, ref bMeasure,
                    ref angleMeasure);
                for (var i = 0; i < x.Count; i++)
                {
                    zMeasureBase[i] = aMeasure*x[i] + bMeasure;
                    //ZReal[i] = ZBase[i] - Y[i];
                    measureConfigY[i] = Math.Abs(aMeasure*x[i] + bMeasure - y[i])/Math.Sqrt(aMeasure*aMeasure + 1);
                }
                var pase3 = new List<double>();
                var pase4 = new List<double>();
                if (Math.Cos(angleMeasure) > 0.0000001)
                {
                    for (var i = 0; i < x.Count; i++)
                    {
                        measureConfigX[i] = measureConfigY[i]*Math.Tan(angleMeasure)*Math.Cos(angleMeasure) + x[i];
                        pase3.Add(measureConfigX[i]);
                        pase4.Add(measureConfigX[i]);
                    }
                    //pase2.Remove(X.Count - 1);
                    for (var i = 0; i < x.Count - 1; i++)
                    {
                        pase4[i] = (pase3[i + 1] - pase3[i])/Math.Cos(angleMeasure);

                    }
                    pase4.Reverse();
                    pase4[0] = 0;

                    for (var i = 0; i < x.Count; i++)
                    {
                        double xvalue = 0;
                        for (var j = 0; j < i + 1; j++)
                        {
                            xvalue = xvalue + pase4[j];
                        }
                        measureConfigX[i] = xvalue;
                    }
                }
                else
                {

                    for (var i = 0; i < x.Count; i++)
                    {
                        measureConfigX[i] = -(measureConfigY[i]*Math.Tan(angleMeasure)*Math.Cos(angleMeasure)) + x[i];
                        pase3.Add(measureConfigX[i]);
                        pase4.Add(measureConfigX[i]);
                    }
                    for (var i = 0; i < x.Count - 1; i++)
                    {
                        pase4[i] = (pase3[i + 1] - pase3[i])/Math.Cos(angleMeasure);
                    }
                    pase4.Reverse();
                    pase4[0] = 0;
                    for (var i = 0; i < x.Count - 1; i++)
                    {
                        double xvalue = 0;
                        for (var j = 0; j < i + 1; j++)
                        {
                            xvalue = xvalue + pase4[j];
                        }
                        measureConfigX[i] = xvalue;
                    }
                }
                measureConfigY.Reverse();

                #endregion

                #region

                List<double> standardProfileValue = new List<double>();
                List<double> measuredProfileValue = new List<double>();
                NormalizeProfile(standardConfigX, standardConfigY, ref standardProfileValue);
                NormalizeProfile(measureConfigX, measureConfigY, ref measuredProfileValue);
                List<double> compareResults =
                    standardProfileValue.Select((t, i) => measuredProfileValue[i] - t).ToList();
                compareResults.Remove(0);
                compareResults.RemoveAt(compareResults.Count-1);
                var result1 = compareResults.Max();
                var result2 = compareResults.Min();
                result = Math.Abs(result1) > Math.Abs(result2) ? result1 : result2;

                //var lists = new List<List<double>> {standardProfileValue, measuredProfileValue, compareResults};

                //Export($"D:\\lineOutput\\{DateTime.Now.ToString("yyyymmddHHMMss")}.csv", lists);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="angle"></param>
        public static void LeastSquareMethodFitBaseLine(double[] x, double[] y, ref double a, ref double b, ref double angle)
        {
            double sumX = 0;
            double sumY = 0;
            double sumXy = 0;
            double sumX2 = 0;
            //double Angle = 0;

            var dataCnt = Math.Min(x.Length, y.Length);

            if (dataCnt == 1)
            {
                a = 0;
                b = y[0];
                return;
            }

            for (var i = 0; i < dataCnt; i++)
            {
                //X和
                sumX += x[i];
                //Y和
                sumY += y[i];
                //X*Y和
                sumXy += x[i] * y[i];
                //X2和
                sumX2 += x[i] * x[i];
            }

            //nΣx2-(Σx)2
            var divisor = dataCnt * sumX2 - sumX * sumX;

            if (!(Math.Abs(divisor) > 1E-06)) return;
            // a=(nΣxy - ΣxΣy)/[nΣx2-(Σx)2]
            a = (dataCnt * sumXy - sumX * sumY) / divisor;

            // b=(Σx2Σy - ΣxyΣx)/[nΣx2-(Σx)2]
            b = (sumX2 * sumY - sumXy * sumX) / divisor;
            angle = Math.Atan(a);
        }

        public static void NormalizeProfile(List<double> inputX, List<double> inputY, ref List<double> zTest)
        {
            List<double> XTest = new List<double>();
            double profileLength = 6000;
            //var profileLength = Math.Floor(XReal.Max());
            const int profileCount = 100;
            XTest.Add(0);
            zTest.Add(inputY[0]);
            for (var i = 1; i < profileCount; i++)
            {
                XTest.Add((profileLength / profileCount) * i);
                var selectIndex = inputX.FindIndex(x => x > profileLength / profileCount * i);
                if (selectIndex == -1)
                {
                    zTest.Add(9999);
                    continue;
                }
                var indexMin = selectIndex - 1;
                var indexMax = selectIndex;
                var deltaZReal = inputY[indexMin] - inputY[indexMax];
                var deltaXReal = inputX[indexMin] - inputX[indexMax];
                var deltaXTest = inputX[indexMin] - ((profileLength / profileCount) * i);
                zTest.Add(inputY[indexMin] + deltaZReal / deltaXReal * deltaXTest);
            }

        }
        
        //自适应截取标准轮廓线的长度
        public static void SlectLength(List<double> inputX1, List<double> inputY1, List<double> inputX2, List<double> inputY2, ref List<double> outputX,
            ref List<double> outputY)
        {
            double deletaX = inputX1[inputX1.Count - 1] - inputX1[0];
            double outputstartX = inputX2[inputX2.Count - 1] - deletaX;
            var selectnumber = inputX2.FindIndex(x => x > outputstartX);
            for (var i = selectnumber - 1; i < inputX2.Count; i++)
            {
                outputX.Add(inputX2[i]);
                outputY.Add(inputY2[i]);
            }
        }

        public static void Export(string file, List<List<double>> lists)
        {
            if(lists.Count < 3)
                return;

            var str = new StringBuilder();

            for (var i = 0; i < lists[0].Count - 1; i++)
            {
                str.AppendLine($"{lists[0][i]:0.000000},{lists[1][i]:0.000000},{lists[2][i]:0.000000}");
            }

            File.WriteAllText(file, str.ToString());
        }
    }
}
