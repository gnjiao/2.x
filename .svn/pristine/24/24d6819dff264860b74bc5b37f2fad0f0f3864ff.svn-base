using System;
using System.Diagnostics;
using Hdc.Mercury;
using Hdc.Reactive.Linq;

namespace Hdc.Measuring
{
    [Serializable]
    public class RobotResetCompleatedPluginInitializer: IInitializer
    {
        private IDevice<bool> _robotResetCompletedAppEventDevice;

        public void Initialize()
        {
            _robotResetCompletedAppEventDevice = OpcInitializer.DeviceGroupProvider.RootDeviceGroup
                .GetDevice<bool>("RobotResetCompletedAppEvent");

            AD7230Service.Singletone.RobotResetCompleatedRobotEvent.ObserveOnTaskPool().Subscribe(
                x =>
                {
                    Debug.WriteLine("RobotResetCompletedAppEventDevice Changed: " + x + ", at " + DateTime.Now);
                    _robotResetCompletedAppEventDevice.Write(true);
                    Debug.WriteLine("RobotResetCompletedAppEventDevice Set to 'false'" + ", at " + DateTime.Now);
                });
        }
    }
}