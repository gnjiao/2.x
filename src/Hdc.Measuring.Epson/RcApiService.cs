using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using RCAPINet;

namespace Hdc.Measuring
{
    public class RcApiService
    {
        public static RcApiService Singletone { get; private set; } = new RcApiService();

        private readonly Subject<SpelEvent> _subject = new Subject<SpelEvent>();

        private Spel _spel;

        public IObservable<SpelEvent> EventReceivedEvent => _subject;

        private RcApiService()
        {
        }

        /// <summary>
        /// *.sprj
        /// </summary>
        /// <param name="projectFileName"></param>
        public void Initialize(string projectFileName, int connectionNumber=1)
        {
            _spel = new Spel();
            _spel.Initialize();
            _spel.Connect(connectionNumber);

            try
            {
                _spel.Project = projectFileName;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("_spel.Project error: " + projectFileName, e);
            }

            _spel.EventReceived += (o, args) =>
            {
                Console.WriteLine("_spel.EventReceived: Event={0}, Message={1}", args.Event, args.Message);

                _subject.OnNext(new SpelEvent()
                {
                    EventNumber = (int) args.Event,
                    Message = args.Message,
                });
            };

            _spel.EnableEvent(SpelEvents.Error, true);
            _spel.EnableEvent(SpelEvents.AllTasksStopped, true);
            _spel.EnableEvent(SpelEvents.Print, true);

            _spel.Reset();
//            _spel.Stop();

            Thread.Sleep(1000);

//            _spel.PowerHigh = true;
//            _spel.Accel(50,50);
//            _spel.Speed(50);

            _spel.Start(0);

            Console.WriteLine("_spel.Start(0)");
        }

        public void SetVar(string varName, object value)
        {
            _spel.SetVar(varName, value);
        }
    }
}