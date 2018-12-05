using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using FocalSpec.FsApiNet.Model;
using Hdc.Collections.Generic;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("MeasureDataMapEntries")]
    public class FocalSpecInspectorMeasureDevice : IMeasureDevice
    {
        const int MAXNUMPTS = 10000;
        public IObservable<List<Profile>> CsvAcquisitedEvent => _csvAcquisitedEvent;
        public IObservable<List<List<double>>> CsvTempEvent => _csvTempEvent;

        public Collection<IMeasureDataMapEntry> MeasureDataMapEntries { get; set; } =
            new Collection<IMeasureDataMapEntry>();
        public bool IsInitialized =>_isInitialized ;

        private bool _isInitialized = false;

        private int _tagCounter;
        private readonly Subject<List<Profile>> _csvAcquisitedEvent = new Subject<List<Profile>>();
        private readonly Subject<List<List<double>>> _csvTempEvent = new Subject<List<List<double>>>();

        public bool IsShortStandardConfig { get; set; }
        public bool IsReverseY { get; set; }

        public float maxGap { get; set; } = 20; // 20 um gap is allowed
        public float minEdge { get; set; } = 30; // minimum z change for edge detection

        public float startX { get; set; } = 500; // in micrometers

        private const int removeListCount  = 50; //末尾截除50个点

        public int LedPulseWidth { get; set; } = 800;

        public void Initialize()
        {            
            if(_isInitialized)
                return;

            _isInitialized = true;
            //            Console.WriteLine($"{nameof(FocalSpecInspectorMeasureDevice)}.Initialize(), begin");
            //            Console.WriteLine($"{nameof(FocalSpecInspectorMeasureDevice)}.Initialize(), end");

            
        }

        private void Flush()
        {
            const double timeout = 3000;
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (!FocalSpecInspectorInitializer.Singleton.CameraManager.IsCameraBufferEmpty)
            {
                if (stopwatch.Elapsed.TotalMilliseconds > timeout)
                    break;
            }
        }

        public MeasureResult Measure(MeasureEvent measureEvent)
        {
            FocalSpecInspectorInitializer.Singleton.CameraManager.SetPulseWidth(LedPulseWidth,Defines.DefaultMaxPulseWidth);

            Flush();

            var profiles = FocalSpecInspectorInitializer.Singleton.CameraManager.StartSelfGrabbing();

            var maxCount = profiles.Max(p=>p.Points.Count);

            profiles = new List<Profile>(profiles.Where(x => x.Points.Count == maxCount).Take(1));
            
            var results = new List<double>();

            _csvAcquisitedEvent.OnNext(profiles);

            foreach (var profile in profiles)
            {
                //删除最后50个设备异常点    
                var dataPoints = new List<FsApi.Point>(profile.Points);
                dataPoints.RemoveRange(dataPoints.Count - removeListCount, removeListCount);

                profile.Points = dataPoints;

                if (IsReverseY)
                {
                    var tempPoints = new List<FsApi.Point>(profile.Points.Reverse());                    

                    for (var index = 0; index < profile.Points.Count; index++)
                    {
                        var pt = profile.Points[index];
                        //pt.X = float.Parse($"{tempPoints[index].X * Defines.ProfileScale:0.000000}");
                        //pt.Y = float.Parse($"{tempPoints[index].Y * Defines.ProfileScale:0.000000}");
                        pt.Y = tempPoints[index].Y;
                        profile.Points[index] = pt;
                    }
                }

                var listX = new List<double>();
                var listY = new List<double>();

                int nPtsTest = 0;
                double[] xTest = new double[MAXNUMPTS];
                double[] yTest = new double[MAXNUMPTS];
                var result = 0.0;

                var edgePoint = FindEndPoint(profile, maxGap, minEdge, startX);  //寻找结束点

                for (var index = 0; index < profile.Points.Count; index++)
                {
                    var point = profile.Points[index];

                    if (Math.Abs(edgePoint.X - point.X) < 0.00000001)
                        break;

                    listX.Add(point.X);
                    listY.Add(point.Y);

                    xTest[index] = double.Parse($"{point.X * Defines.ProfileScale:0.000000}");
                    yTest[index] = double.Parse($"{point.Y * Defines.ProfileScale:0.000000}");
                }

                nPtsTest = listX.Count;
                
                var dataTemp = new List<List<double>> {listX, listY};
                                
                _csvTempEvent.OnNext(dataTemp);               

                int nPtsTestOut = -1;
                double[] xTestOut = new double[MAXNUMPTS];
                double[] yTestOut = new double[MAXNUMPTS]; 
                double[] yTestResOut = new double[MAXNUMPTS];
                
                if (IsShortStandardConfig) //短边
                {
                    result = NativeMethods.mwProfErrCal(
                        FocalSpecInspectorInitializer.Singleton.nPtsRefShort,
                        FocalSpecInspectorInitializer.Singleton.xRefShort,
                        FocalSpecInspectorInitializer.Singleton.yRefShort,
                        nPtsTest, xTest, yTest,
                        ref nPtsTestOut, xTestOut, yTestOut, yTestResOut,
                        0L, 0.05, 10L, 1L, 1e-6, "");

                    Export2($"D:/CSV/{DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss")}_TestOut.csv",
                        MAXNUMPTS, xTestOut, yTestOut, yTestResOut, FocalSpecInspectorInitializer.Singleton.xRefShort, FocalSpecInspectorInitializer.Singleton.yRefShort);
                }
                else //边
                {
                    result = NativeMethods.mwProfErrCal(
                        FocalSpecInspectorInitializer.Singleton.nPtsRefLong,
                        FocalSpecInspectorInitializer.Singleton.xRefLong,
                        FocalSpecInspectorInitializer.Singleton.yRefLong, 
                        nPtsTest, xTest, yTest,
                        ref nPtsTestOut, xTestOut, yTestOut, yTestResOut,
                        0L, 0.05, 10L, 1L, 1e-6, "");

                    Export2($"D:/CSV/{DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss")}_TestOut.csv",
                        MAXNUMPTS, xTestOut, yTestOut, yTestResOut, FocalSpecInspectorInitializer.Singleton.xRefLong, FocalSpecInspectorInitializer.Singleton.yRefLong);
                }


                

                // NativeMethods.mwLibAbort();

                //LineConfocalWhiteLightProcess.LineConfocalWhiteLightProcessCulate(listX,listY, StandardConfigDataPath, ref result);

                results.Add(result);
            }
                        
            Thread.Sleep(1000);

            var measureResult = new MeasureResult
            {
                Tag = _tagCounter,
                MeasureDateTime = DateTime.Now,
            };

            var ouputCount = results.Count;

            if (ouputCount == 0)
            {
                measureResult.Outputs.Add(new MeasureOutput()
                {
                    Index = 0,
                    Validity = MeasureValidity.Wait,
                    Judge = MeasureOutputJudge.Ng,
                    Value = 99999.999
                });
            }
            else
            {
                for (var i = 0; i < ouputCount; i++)
                {
                    measureResult.Outputs.Add(new MeasureOutput()
                    {
                        Index = i,
                        Validity = MeasureValidity.Wait,
                        Judge = MeasureOutputJudge.Go,
                        Value = results[i]
                    });
                }
            }

            _tagCounter++;

            return measureResult;
        }

        public static void Export2(string file, int nPts, double[] ls1, double[] ls2, double[] ls3, double[] ls4, double[] ls5)
        {
            var str = new StringBuilder();

            for (var i = 0; i < nPts; i++)
            {
                str.AppendLine(i < ls4.Length
                    ? $"{ls1[i]:0.000000},{ls2[i]:0.000000},{ls3[i]:0.000000},{ls4[i]:0.000000},{ls5[i]:0.000000}"
                    : $"{ls1[i]:0.000000},{ls2[i]:0.000000},{ls3[i]:0.000000}");
            }

            File.WriteAllText(file, str.ToString());
        }

        private static FsApi.Point FindEndPoint(Profile profile, float maxGap, float edgeDist, float startX)
        {
            var prevPoint = profile.Points[0];

            for (var index = 0; index < profile.Points.Count; index++)
            {
                var point = profile.Points[index];

                if (point.X < startX)
                {
                    prevPoint = point;
                    continue;
                }

                if ((point.X - prevPoint.X) > maxGap  && (index > profile.Points.Count / 2))
                {
                    return prevPoint;
                }

                if (Math.Abs(point.Y - prevPoint.Y) > edgeDist && (index > profile.Points.Count / 2))
                {
                    return prevPoint;
                }

                prevPoint = point;
            }

            return prevPoint;
        }
    }
}
