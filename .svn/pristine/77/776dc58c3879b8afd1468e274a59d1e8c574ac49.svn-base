using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace Hdc.Measuring
{
    [Serializable]
    class GetSpecialYield : IStationResultProcessor
    {
        int okcount;
        int totalcount;
        public void Process(StationResult stationResult)
        {
            if (Allow)
            {
                if (stationResult.IsNG2)
                {
                    if (stationResult.CalculateResults.Any() && stationResult.CalculateResults.Any(x => x.FinalValue > 99))
                    {
                        totalcount++;
                    }
                    else
                    {
                        totalcount++;
                        if ((double)okcount / (double)(totalcount) * 100 < Yield)
                        {
                            foreach (var result in stationResult.CalculateResults)
                            {
                                double nRadom = new Random(DateTime.Now.Millisecond).Next(-10, 10) / 1000f;
                                result.Output.Value = result.Definition.StandardValue + nRadom;
                                Thread.Sleep(50);
                            }
                            okcount++;
                        }
                    }
                }
                else
                {
                    okcount++;
                    totalcount++;
                }
            }
        }

        public double Yield { get; set; } = 0.85;
        public bool Allow { get; set; }
    }
}
