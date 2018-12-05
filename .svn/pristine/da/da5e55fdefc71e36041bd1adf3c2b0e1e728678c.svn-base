using System;
using System.Diagnostics;
using Hdc.Reactive.Linq;

namespace Hdc.Measuring
{
    [Serializable]
    public class MqRobotResetCompleatedPluginInitializer: IInitializer
    {
        public void Initialize()
        {
            AD7230Service.Singletone.RobotResetCompleatedRobotEvent.ObserveOnTaskPool().Subscribe(
                x =>
                {
                    Debug.WriteLine("RobotResetCompletedAppEventDevice Changed: " + x + ", at " + DateTime.Now);
                    MqInitializer.Bus.PublishAsync(new GeneralMqCommand() {CommandName = DeviceName }, p => p.WithExpires(5000));


                    Debug.WriteLine("RobotResetCompletedAppEventDevice Set to 'false'" + ", at " + DateTime.Now);
                });
        }

        public string DeviceName { get; set; }
    }
}