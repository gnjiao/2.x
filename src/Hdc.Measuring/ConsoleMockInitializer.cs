using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Hdc.Measuring
{
    [Serializable]
    public class ConsoleMockInitializer : IInitializer
    {
        public static ConsoleMockInitializer Singletone { get; private set; }

        private readonly Subject<MeasureEvent> _subject = new Subject<MeasureEvent>();

        public Subject<MeasureEvent> MeasureEventChanged => _subject;

        public ConsoleMockInitializer()
        {
            if(Singletone!=null)
                throw new InvalidOperationException();

            Singletone = this;
        }

        public void Initialize()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Console.WriteLine("Press input PointIndex to MockMeasureDevice.Measure()");
                    var key = Console.ReadLine();
                    Console.WriteLine();
                    _subject.OnNext(new MeasureEvent()
                    {
                        Message = key,
                    });
                }
            });
        }
    }
}