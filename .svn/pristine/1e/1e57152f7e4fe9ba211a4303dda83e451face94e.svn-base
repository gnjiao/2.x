using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class SimLkMeasureDevice : IMeasureDevice
    {
        private int _counter;

        public int OutputCount { get; set; }

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
            var measureResult = new MeasureResult()
            {
            };

            for (int i = 0; i < OutputCount; i++)
            {
                measureResult.Outputs.Add(new MeasureOutput()
                {
                    Validity = MeasureValidity.Valid,
                    Judge = MeasureOutputJudge.Go,
                    Value = _counter + i*0.01,
                });
            }

            _counter++;

            return measureResult;
        }

        public string Name { get; }
    }
}