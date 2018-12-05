using System;
using System.Threading;

namespace Hdc.Measuring
{
    [Serializable]
    public class QueueSchedulingStationCompletedCommandController : ICommandController
    {
        public void Initialize()
        {
        }

        public void Command(MeasureEvent measureEvent)
        {

//            MqInitializer.Bus.PublishAsync(new SetCommandOpcEvent()
//            {
//                OpcPointName = "StationCompleted" + measureEvent.StationIndex.ToString("D2"),
//                DataType = "int",
//                Value = 1
//
//            }, p => p.WithExpires(5000));
//
//            Thread.Sleep(50);

            MqInitializer.Bus.PublishAsync(new GeneralMqCommand()
            {
                CommandName = $"StationCompletedAppEvent{measureEvent.StationIndex:00}"

            }, p => p.WithExpires(5000));
        }
    }
}
