using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class GeneralMqCommandController : ICommandController
    {
        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
        }

        public void Command(MeasureEvent measureEvent)
        {
            MqInitializer.Bus.PublishAsync(new GeneralMqCommand()
            {
                CommandName = CommandName,
            }, p => p.WithExpires(5000));
        }

        public string CommandName { get; set; }
    }
}