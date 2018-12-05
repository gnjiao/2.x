using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class TriggerInMemoryEventCommandController : ICommandController
    {
        public void Initialize()
        {
        }

        public void Command(MeasureEvent measureEvent)
        {
            InMemoryEventControllerService.Singletone.Trigger(
                new MeasureEvent
                {
                    PositionIndex = measureEvent.PositionIndex,
                    Message = Message,
                    StationIndex = measureEvent.StationIndex,
                    PointIndex = measureEvent.PointIndex
                });
        }

        public int PositionIndex { get; set; }
        public string Message { get; set; }
    }
}