using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class TriggerLightByADLinkDAQMeasureTrigger : IMeasureTrigger
    {
        public int TargetIndex { get; set; }
        public TriggerType TriggerType { get; set; }

        public void Action(IMeasureDevice measureDevice, int pointIndex, MeasureResult measureResult)
        {
            if (State == LightState.Open)
            {
                AD7230Service.Singletone.OpenLight(ChannelIndex);
            }
            else
            {
                AD7230Service.Singletone.CloseLight(ChannelIndex);
            }
        }

        public int ChannelIndex { get; set; }
        public LightState State { get; set; }
    }
}