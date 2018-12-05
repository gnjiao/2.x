using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hdc.Measuring
{
    public class GetRandomExpectValueResultProcessor : IStationResultProcessor
    {
        public void Process(StationResult stationResult)
        {
            foreach (var result in stationResult.CalculateResults)
            {
                double nRadom = new Random(DateTime.Now.Millisecond).Next(-10, 10) / 1000f;
                result.Output.Value = result.Definition.StandardValue + nRadom;
                Thread.Sleep(50);
            }
        }
    }
}
