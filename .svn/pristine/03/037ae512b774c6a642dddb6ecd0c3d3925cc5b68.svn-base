using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Threading;
using Hdc.Mercury;
using Hdc.Reactive.Linq;
using Hdc.Toolkit;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty(nameof(QueueTaskSchemas))]
    public class QueueSchedulingInitializer : IInitializer
    {
        private readonly Subject<MeasureEvent> _sensorInPositionEvent = new Subject<MeasureEvent>();
        private readonly Subject<MeasureEvent> _stationCompletedEvent = new Subject<MeasureEvent>();
        public IObservable<MeasureEvent> SensorInPositionEvent => _sensorInPositionEvent;
        public IObservable<MeasureEvent> StationCompletedEvent => _stationCompletedEvent;

        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        public Collection<QueueTaskSchemas> QueueTaskSchemas { get; set; } = new Collection<QueueTaskSchemas>();
        public static QueueSchedulingInitializer Singleton { get; private set; }
        public string SubscriptionId { get; set; }
        public QueueSchedulingInitializer()
        {
            if (Singleton != null)
                return;            
                                    
            Singleton = this;
        }

        public void Initialize()
        {
            MqInitializer.Bus.Subscribe<SensorInPositionOpcEvent>(SubscriptionId, x =>
            {
                Task.Run(() =>
                {
                    if (x.StationIndex == _revStationIndex)
                    {
                        _sensorInPositionEvent.OnNext(new MeasureEvent
                        {
                            PointIndex = _queueIndex,
                            StationIndex = x.StationIndex,
                            PositionIndex = _queueIndex,
                            Message = x.Message,
                            WorkpieceTag = x.WorkpieceTag
                        });
                    }
                });
            });

            Console.WriteLine($"{nameof(QueueSchedulingInitializer)}.Initialize() begin at " + DateTime.Now);
        }

        private int _revStationIndex = 0;
        private int _queueIndex = 0;

        public void WorkInPositionCommand(MeasureEvent measureEvent)
        {
            _revStationIndex = measureEvent.StationIndex;

            var tasks = QueueTaskSchemas.Single(x => x.StationIndex == measureEvent.StationIndex);
            _queueIndex = 0;
            var index = 0;
            foreach (var task in tasks.QueueTaskSchema)
            {                
                index++;
                _queueIndex = task.Index;

                MqInitializer.Bus.PublishAsync(new SetCommandOpcEvent()
                {
                    OpcPointName = "SensorInAction" + measureEvent.StationIndex.ToString("D2"),
                    DataType = "int",
                    Value = index

                }, p => p.WithExpires(5000));

                _autoResetEvent.WaitOne(task.TimeOut);

                Thread.Sleep(200);
            }            
            
            _stationCompletedEvent.OnNext(new MeasureEvent
            {
                StationIndex = measureEvent.StationIndex,
                Message = measureEvent.Message
            });
        }

        public void SensorInPositionCommand(MeasureEvent measureEvent)
        {            
            _autoResetEvent.Set();
        }
    }
}
