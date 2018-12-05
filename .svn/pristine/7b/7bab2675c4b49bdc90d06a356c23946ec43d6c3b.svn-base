using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;

namespace Hdc.Measuring
{
    [Serializable]
    public class Ad7230MeasureRequestEventController : IEventController
    {
        private readonly Subject<MeasureEvent> _subject = new Subject<MeasureEvent>();

        private int _measureDataReadyRobotEventAbnormalPulseCounter;

        public void Initialize()
        {
            AD7230Service.Singletone.MeasureDataReadyRobotEvent
                .Subscribe(x =>
                {
                    // Verify _ad7230Service.ReadLine0
                    Thread.Sleep(10);

                    if (!AD7230Service.Singletone.ReadLine0())
                    {
                        var message = "MeasureDataReadyRobotEvent abnormal at " + DateTime.Now
                                      + "\n>>> ReadLine0 == false. abnormal total count = " +
                                      _measureDataReadyRobotEventAbnormalPulseCounter;
                        Console.WriteLine(message);
                        _measureDataReadyRobotEventAbnormalPulseCounter++;
                        return;
                    }

                    _subject.OnNext(new MeasureEvent());
                });
        }

        public IObservable<MeasureEvent> Event => _subject;

        public void Acknowledge()
        {
        }
    }
}