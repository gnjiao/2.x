using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("QueueTaskSchemas")]
    public class MultipleQueueSchedulingInitializer : IInitializer
    {
        private int _queueIndex = 0;
        private int _revStationIndex = 0;       
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        private readonly Subject<MeasureEvent> _sensorInPositionEvent = new Subject<MeasureEvent>();
        private readonly Subject<MeasureEvent> _stationCompletedEvent = new Subject<MeasureEvent>();
        public IObservable<MeasureEvent> SensorInPositionEvent => _sensorInPositionEvent;
        public IObservable<MeasureEvent> StationCompletedEvent => _stationCompletedEvent;
        public Collection<QueueTaskSchemas> QueueTaskSchemas { get; set; } = new Collection<QueueTaskSchemas>();
        public static MultipleQueueSchedulingInitializer Singleton { get; private set; }
        public string SubscriptionId { get; set; } = nameof(MultipleQueueSchedulingInitializer);

        public Collection<StationWorkInPieceInfo> StationInfos { get; set; } = new Collection<StationWorkInPieceInfo>();
        public MultipleQueueSchedulingInitializer()
        {
            if (Singleton != null)
                return;

            Singleton = this;
        }

        public void Initialize()
        {
            Console.WriteLine($"{nameof(QueueSchedulingInitializer)}.Initialize() begin at " + DateTime.Now);
        }

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

                for (var i = 1; i <= 4; i++)
                {
                    MqInitializer.Bus.PublishAsync(new SetCommandOpcEvent()
                    {
                        OpcPointName = $"SensorInAction{i:D2}",
                        DataType = "int",
                        Value = index

                    }, p => p.WithExpires(5000));

                    Thread.Sleep(50);
                }
                
                _autoResetEvent.WaitOne(task.TimeOut);

                Thread.Sleep(200);
            }

            for (var i = 1; i <= 4; i++)
            {
                _stationCompletedEvent.OnNext(new MeasureEvent
                {
                    StationIndex = i,
                    Message = measureEvent.Message
                });

                Thread.Sleep(50);
            }
        }

        public void SensorInPositionCommand(MeasureEvent measureEvent)
        {
            _autoResetEvent.Set();
        }
    }
}
