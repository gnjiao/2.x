using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using Hdc.FA.KeyenceLasers.LJV7;

namespace Hdc.Measuring
{
    [Serializable]
    public class LjvMeasureDevice : IMeasureDevice
    {
        private int _tagCounter;
        private static bool _isInitialized;

        public void Initialize()
        {
            NativeMethods.LJV7IF_Initialize();
            NativeMethods.LJV7IF_UsbOpen(0);
            _isInitialized = true;
        }

        public bool IsInitialized => _isInitialized;

        public void BeforeMeasure(Action action)
        {
            action();
        }

        public void AfterMeasure(Action action)
        {
            action();
        }

        public MeasureResult Measure(MeasureEvent measureEvent)
        {
            var measureResult = new MeasureResult()
            {
                Tag = _tagCounter,
                MeasureDateTime = DateTime.Now,
            };

            var laserOuputCount = 4;
            for (int i = 0; i < laserOuputCount; i++)
            {
                measureResult.Outputs.Add(new MeasureOutput()
                {
                    Index = i,
                    Validity = MeasureValidity.Wait,
                    Judge = MeasureOutputJudge.Ng,
                    Value = 0,
                });
            }

            //
            LJV7IF_MEASURE_DATA[] mds = new LJV7IF_MEASURE_DATA[16];
            var ret= NativeMethods.LJV7IF_GetMeasurementValue(0, mds);
            if (ret != 0)
            {
                throw new InvalidOperationException("LJV7IF_GetMeasurementValue error");
            }

            Console.WriteLine("LJV7IF_MEASURE_DATA-1: " + mds[0].fValue.ToString("00.000") + "_" + DateTime.Now);

            //
            LJV7IF_MEASURE_DATA[] mds2 = new LJV7IF_MEASURE_DATA[16];
            ret = NativeMethods.LJV7IF_GetMeasurementValue(0, mds2);
            if (ret != 0)
            {
                throw new InvalidOperationException("LJV7IF_GetMeasurementValue error");
            }
            Console.WriteLine("LJV7IF_MEASURE_DATA-2: " + mds2[0].fValue.ToString("00.000") + "_" + DateTime.Now);

            //
            var differ = Math.Abs(mds[0].fValue - mds2[0].fValue);
            if (differ > 0.1)
            {
                var message = "LJV7IF_MEASURE_DATA-1to2 overflow: " + differ.ToString("00.000") + "_" + DateTime.Now;
                Console.WriteLine(message);
                Debug.WriteLine(message);
            }

            for (int i = 0; i < laserOuputCount; i++)
            {
                var md = mds[i];
                var output = measureResult.Outputs[i];
                output.Value = md.fValue;
            }

            _tagCounter++;

            return measureResult;
        }
        
    }
}