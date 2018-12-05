using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Hdc.Measuring
{
    [Serializable]
    public class RcApiEventController : IEventController
    {
        public void Initialize()
        {
            Event = RcApiService.Singletone.EventReceivedEvent
                .Where(x => x.EventNumber == EventNumber)
                .Select(x =>
                {
                    var measureEvent = new MeasureEvent()
                    {
                        EventNumber = x.EventNumber,
                        Message = x.Message,
                    };

                    if (x.Message.Contains("PositionIndex") &&
                        x.Message.Contains("PointIndex"))
                    {
                        var entries = x.Message.Split(';');
                        measureEvent.PositionIndex = int.Parse(entries.Single(p => p.StartsWith("PositionIndex"))
                            .Replace("PositionIndex=", ""));
                        measureEvent.PointIndex = int.Parse(entries.Single(p => p.StartsWith("PointIndex"))
                            .Replace("PointIndex=", ""));
                        measureEvent.X = double.Parse(entries.Single(p => p.StartsWith("CX"))
                            .Replace("CX=", ""));
                        measureEvent.Y = double.Parse(entries.Single(p => p.StartsWith("CY"))
                            .Replace("CY=", ""));
                    }

                    return measureEvent;
                });
        }

        public IObservable<MeasureEvent> Event { get; private set; }

        public int EventNumber { get; set; }
    }
}