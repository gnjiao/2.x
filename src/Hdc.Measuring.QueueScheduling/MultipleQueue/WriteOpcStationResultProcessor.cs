using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hdc.Measuring
{
    public class WriteOpcStationResultProcessor :  IStationResultProcessor
    {
        public void Process(StationResult stationResult)
        {
            var isNg = stationResult.IsNG2 ? 1 : 2;

            var stationIndex = stationResult.StationIndex;

            MqInitializer.Bus.PublishAsync(new SetCommandOpcEvent()
            {
                OpcPointName = $"IsNg{stationIndex:D2}",
                DataType = "int",
                Value = isNg

            }, p => p.WithExpires(5000));
        }
    }
}
