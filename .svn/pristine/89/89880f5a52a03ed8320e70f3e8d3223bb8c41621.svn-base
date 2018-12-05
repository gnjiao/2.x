using System;
using System.Threading;
using Hdc.FA.KeyenceLasers.LK;

namespace Hdc.Measuring
{
    [Serializable]
    public class LkMeasureDevice : IMeasureDevice
    {
        private int _tagCounter;

        public void Initialize()
        {
        }

        public bool IsInitialized => true;

        public void BeforeMeasure(Action action)
        {
            
        }

        public void AfterMeasure(Action action)
        {
        }

        public MeasureResult Measure(MeasureEvent measureEvent)
        {
            var now = DateTime.Now;
            var measureResult = new MeasureResult()
            {
                Tag = _tagCounter,
                MeasureDateTime = now,
            };

            for (int i = 0; i < 2; i++)
            {
                measureResult.Outputs.Add(new MeasureOutput()
                {
                    Index = i,
                    Validity = MeasureValidity.Wait,
                    Judge = MeasureOutputJudge.Ng,
                    MeasureEvent = measureEvent,
                    Value = 0,
                });
            }

            var output0 = measureResult.Outputs[0];
            var output1 = measureResult.Outputs[1];

            LkIF.LKIF_FLOATVALUE calcData1 = new LkIF.LKIF_FLOATVALUE();
            LkIF.LKIF_FLOATVALUE calcData2 = new LkIF.LKIF_FLOATVALUE();

            if (LkIF.LKIF_GetCalcData(ref calcData1, ref calcData2) == 1)
            {
                GetValue(calcData1, output0);
                GetValue(calcData2, output1);

                measureResult.Message = "LKIF_GetCalcData ended normally." + now;
            }
            else
            {
                output0.Validity = MeasureValidity.Error;
                output1.Validity = MeasureValidity.Error;
                measureResult.HasWarning = true;
                measureResult.Message = "LKIF_GetCalcData terminated abnormally." + now;
            }

            Console.WriteLine("LkIF.LKIF_FLOATVALUE-1: " + calcData1.Value.ToString("00.000") + "_" + now);
            Console.WriteLine("LkIF.LKIF_FLOATVALUE-2: " + calcData2.Value.ToString("00.000") + "_" + now);

            _tagCounter++;

            return measureResult;
        }

//        public string Name { get; set; }

        private static void GetValue(LkIF.LKIF_FLOATVALUE calcData, MeasureOutput output0)
        {
            switch (calcData.FloatResult)
            {
                case LkIF.LKIF_FLOATRESULT.LKIF_FLOATRESULT_VALID:
                    output0.Validity = MeasureValidity.Valid;
                    output0.Value = calcData.Value;
                    break;
                case LkIF.LKIF_FLOATRESULT.LKIF_FLOATRESULT_RANGEOVER_P:
                    output0.Validity = MeasureValidity.Valid;
                    output0.Message = "over range at positive (+) side";
                    break;
                case LkIF.LKIF_FLOATRESULT.LKIF_FLOATRESULT_RANGEOVER_N:
                    output0.Validity = MeasureValidity.Valid;
                    output0.Message = "over range at negative (-) side";
                    break;
                case LkIF.LKIF_FLOATRESULT.LKIF_FLOATRESULT_WAITING:
                    output0.Validity = MeasureValidity.Wait;
                    output0.Message = "comparator result";
                    break;
                default:
                    throw new Exception("LKIF_GetCalcData abnormal");
            }
        }
    }
}