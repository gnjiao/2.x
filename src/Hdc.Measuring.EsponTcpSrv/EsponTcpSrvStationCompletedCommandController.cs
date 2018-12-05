using System;
using System.ComponentModel;
using Hdc.Mercury;

namespace Hdc.Measuring
{

    [Serializable]
    public class EsponTcpSrvStationCompletedCommandController : ICommandController
    {
        private bool _isInitialized;
        private IDevice<bool> _stationComplete;

        public void Initialize()
        {
            if (_isInitialized) return;

            _stationComplete = OpcInitializer.DeviceGroupProvider.RootDeviceGroup.GetDevice<bool>("StationComplete");

            _isInitialized = true;
        }

        public void Command(MeasureEvent measureEvent)
        {
            MqInitializer.Bus.PublishAsync(new GeneralMqCommand()
            {
                CommandName = $"StationCompletedAppEvent{measureEvent.StationIndex.ToString("00")}"
            }, p => p.WithExpires(5000));

            if (measureEvent.Message == "END")
            {
                _stationComplete.Write(true);
            }
        }        
    }
}
