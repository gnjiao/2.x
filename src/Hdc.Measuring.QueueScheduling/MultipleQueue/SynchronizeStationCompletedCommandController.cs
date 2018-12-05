using System;
using System.Threading;

namespace Hdc.Measuring
{
    [Serializable]
    public class SynchronizeStationCompletedCommandController : ICommandController
    {
        public void Initialize()
        {
        }

        public void Command(MeasureEvent measureEvent)
        {
            //foreach (var si in MultipleQueueSchedulingInitializer.Singleton.StationInfos)

            for(var i=1; i<=4; i++)
            {
                MqInitializer.Bus.PublishAsync(new SetCommandOpcEvent
                {
                    OpcPointName = $"StationCompleted{i:D2}",
                    DataType = "int",
                    Value = 1

                }, p => p.WithExpires(5000));

                Thread.Sleep(50);

                MqInitializer.Bus.PublishAsync(new GeneralMqCommand
                {
                    CommandName = $"StationCompletedAppEvent{i:D2}"

                }, p => p.WithExpires(5000));

                Thread.Sleep(50);
            } 
        }
    }
}
