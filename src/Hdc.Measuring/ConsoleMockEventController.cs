using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Hdc.Measuring
{
    [Serializable]
    public class ConsoleMockEventController : IEventController
    {
        private IObservable<MeasureEvent> _event;

        public void Initialize()
        {
            if (string.IsNullOrEmpty(EndingString))
            {
                _event = ConsoleMockInitializer.Singletone.MeasureEventChanged
                    .Where(x =>
                    {
                        int pointIndex;
                        var isPointIndex = int.TryParse(x.Message, out pointIndex);
                        return isPointIndex;
                    })
                    .Select(x =>
                    {
                        int pointIndex;
                        var isPointIndex = int.TryParse(x.Message, out pointIndex);

                        if (isPointIndex)
                        {
                            var measureEvent = new MeasureEvent()
                            {
                                PointIndex = pointIndex,
                                Message = x.Message,
                            };
                            return measureEvent;
                        }


                        throw new InvalidOperationException($"{nameof(ConsoleMockEventController)}, error");
                    });
            }
            else
            {
                _event = ConsoleMockInitializer.Singletone.MeasureEventChanged
                    .Where(x => x.Message == EndingString)
                    .Select(x =>
                    {
                        var measureEvent = new MeasureEvent()
                        {
                            EventNumber = x.EventNumber,
                            Message = x.Message,
                        };
                        return measureEvent;
                    });
            }
        }

        public IObservable<MeasureEvent> Event
        {
            get { return _event; }
        }

        public string EndingString { get; set; }
    }
}