using System;
using System.Reactive.Subjects;

namespace Hdc.Measuring
{
    public class InMemoryEventControllerService
    {
        public static InMemoryEventControllerService Singletone { get; private set; } =
            new InMemoryEventControllerService();

        private readonly Subject<MeasureEvent> _subject = new Subject<MeasureEvent>();

        private InMemoryEventControllerService()
        {
        }

        public IObservable<MeasureEvent> EventReceivedEvent => _subject;

        public void Trigger(MeasureEvent measureEvent)
        {
            _subject.OnNext(measureEvent);
        }
    }
}