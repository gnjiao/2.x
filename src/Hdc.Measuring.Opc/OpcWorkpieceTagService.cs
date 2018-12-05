using Hdc.Mercury;

namespace Hdc.Measuring
{
    public class OpcWorkpieceTagService : IWorkpieceTagService
    {
        private bool _isInitialized;
        public int Tag => _totalCountDevice.Value;

        private IDevice<int> _totalCountDevice;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _totalCountDevice = OpcInitializer.DeviceGroupProvider.RootDeviceGroup
                .GetDevice<int>(DeviceName);
        }

        public OpcWorkpieceTagService()
        {
        }

        public OpcWorkpieceTagService(string deviceName)
        {
            DeviceName = deviceName;
        }

        public string DeviceName { get; set; }
    }
}