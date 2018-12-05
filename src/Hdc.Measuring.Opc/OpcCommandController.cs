using System;
using Hdc.Mercury;

namespace Hdc.Measuring
{
    [Serializable]
    public class OpcCommandController : ICommandController
    {
        public OpcCommandController()
        {
        }

        public OpcCommandController(string deviceName)
        {
            DeviceName = deviceName;
        }

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _device = OpcInitializer.DeviceGroupProvider.RootDeviceGroup
                .GetDevice<bool>(DeviceName);
        }

        private IDevice<bool> _device;

        private bool _isInitialized;

        public string DeviceName { get; set; }

        public void Command(MeasureEvent measureEvent)
        {
            Console.WriteLine("OpcCommandSignalController.Command(): " + DeviceName + ", begin at " + DateTime.Now);
            _device.WriteTrue();
            Console.WriteLine("OpcCommandSignalController.Command(): " + DeviceName + ", end at " + DateTime.Now);
        }
    }
}