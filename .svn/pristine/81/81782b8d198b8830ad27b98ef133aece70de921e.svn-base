using System;

namespace Hdc.Measuring
{
    public class MockMeasureDevice : IMeasureDevice
    {
        public void Initialize()
        {
        }

        public bool IsInitialized { get; }

        public MeasureResult Measure(MeasureEvent measureEvent)
        {
            if (AutoMeasure)
            {
                Console.WriteLine("MockMeasureDevice.Measure(): AutoMeasure");
            }
            else
            {
                Console.WriteLine("Press any key to MockMeasureDevice.Measure()");
                Console.ReadKey();
            }

            var measureResult = new MeasureResult();
            measureResult.Outputs.Add(new MeasureOutput());
            measureResult.Outputs.Add(new MeasureOutput());
            return measureResult;
        }

        public bool AutoMeasure { get; set; }
    }
}