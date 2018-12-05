using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class GeneralMqEventTrigger: IMeasureTrigger
    {
        public TriggerType TriggerType { get; set; }

        public void Action(IMeasureDevice measureDevice, int pointIndex, MeasureResult measureResult)
        {
            MqInitializer.Bus.PublishAsync(new GeneralMqCommand()
            {
                CommandName = CommandName,
            }, p => p.WithExpires(5000));
        }

        public string CommandName { get; set; }
    }
}