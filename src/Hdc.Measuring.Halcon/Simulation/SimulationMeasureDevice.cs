using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hdc.Mv;

namespace Hdc.Measuring
{
    [Serializable]
    public class SimulationMeasureDevice : IMeasureDevice
    {
        private bool _isInitialized;
        public void Initialize()
        {
            if (_isInitialized) return;

            Console.WriteLine($"{nameof(SimulationMeasureDevice)}:: " +
                              $"begin at: " + DateTime.Now);

            Console.WriteLine($"{nameof(SimulationMeasureDevice)}:: " +
                              $"end at: " + DateTime.Now);
            _isInitialized = true;
        }

        public bool IsInitialized => _isInitialized;
        public MeasureResult Measure(MeasureEvent measureEvent)
        {
            var mr = new MeasureResult();

            mr.Outputs.Add(new MeasureOutput() {Validity = MeasureValidity.Valid, Value = new Random(DateTime.Now.Millisecond).NextDouble()});

            return mr;
        }
    }
    
}
