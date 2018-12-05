using System;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [ContentProperty(nameof(WrapperMeasureDevice.MeasureDevice))]
    public class WrapperMeasureDevice : IMeasureDevice
    {
        public void Initialize()
        {
            MeasureDevice.Initialize();
        }

        public bool IsInitialized => MeasureDevice.IsInitialized;

        public MeasureResult Measure(MeasureEvent measureEvent)
        {
            int pointIndex;

            while (true)
            {
                var key = Console.ReadLine();

                Console.WriteLine("Press input PointIndex to MockMeasureDevice.Measure()");
                var isPointIndex = int.TryParse(key, out pointIndex);

                if (isPointIndex)
                {
                    var measureResult = MeasureDevice.Measure(measureEvent);
                    
                    return measureResult;
                }
                else
                {
                }
            }
        }

        public IMeasureDevice MeasureDevice { get; set; }
    }
}