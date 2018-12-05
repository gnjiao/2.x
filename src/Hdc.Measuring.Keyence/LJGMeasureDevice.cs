using System;
using Hdc.FA.KeyenceLasers.LJG;

namespace Hdc.Measuring
{
    [Serializable]
    public class LJGMeasureDevice : IMeasureDevice
    {
        private int _tagCounter;
        private const int SUCCESS = 0;
        private static bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            if (LJGInitializer.Singletone == null)
            {
                Console.WriteLine($"{nameof(LJGMeasureDevice)}.Initialize(), begin");
                var ret = LJIF.LJIF_OpenDeviceUSB();
                if (ret != 0)
                    throw new InvalidOperationException($"{nameof(LJGMeasureDevice)}.Initialize error");
            }

            Console.WriteLine($"{nameof(LJGMeasureDevice)}.Initialize(), end");
        }

        public bool IsInitialized => _isInitialized;

        public MeasureResult Measure(MeasureEvent measureEvent)
        {
            var measureResult = new MeasureResult()
            {
                Tag = _tagCounter,
                MeasureDateTime = DateTime.Now,
            };

            var laserOuputCount = 8;
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

            LJIF.LJIF_MEASUREDATA[] MesureParam = new LJIF.LJIF_MEASUREDATA[laserOuputCount];

            int rc = LJIF.LJIF_GetMeasureValue(ref MesureParam[0], laserOuputCount);
            if (rc == SUCCESS)
            {
//                for (int OutCount = 0; OutCount <= 7; OutCount++)
//                {
//                    switch (MesureParam[OutCount].FloatResult)
//                    {
//                    }
//                }

                for (int i = 0; i < laserOuputCount; i++)
                {
                    var md = MesureParam[i];
                    var output = measureResult.Outputs[i];
                    output.Value = md.fValue;
                }
            }
            else
            {
                throw new InvalidOperationException("LJIF.LJIF_GetMeasureValue error");
            }

            _tagCounter++;

            return measureResult;
        }
    }
}