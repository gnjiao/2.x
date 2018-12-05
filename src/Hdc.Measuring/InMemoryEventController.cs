using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Hdc.Measuring
{
    [Serializable]
    public class InMemoryEventController : IEventController
    {
        public void Initialize()
        {
            Event = InMemoryEventControllerService.Singletone.EventReceivedEvent
                .Where(x => x.Message == EventName)
                .Select(x => x);
        }

        public IObservable<MeasureEvent> Event { get; private set; }

        public string EventName { get; set; }
    }
}