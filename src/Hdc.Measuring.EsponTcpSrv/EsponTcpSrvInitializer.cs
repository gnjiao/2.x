using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Threading;
using Hdc.Mercury;
using Hdc.Mv.LocatingService;
using Hdc.Mv.RobotVision.LocatingService;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace Hdc.Measuring
{

    [Serializable]
    [ContentProperty("TcpTaskSchemas")]
    public class EsponTcpSrvInitializer : IInitializer
    {
        private readonly Subject<MeasureEvent> _sensorInPositionEvent = new Subject<MeasureEvent>();
        private readonly Subject<MeasureEvent> _stationCompletedEvent = new Subject<MeasureEvent>();
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        public IObservable<MeasureEvent> SensorInPositionEvent => _sensorInPositionEvent;
        public IObservable<MeasureEvent> StationCompletedEvent => _stationCompletedEvent;

        public Collection<TcpTaskSchemas> TcpTaskSchemas { get; set; } = new Collection<TcpTaskSchemas>();

        public static EsponTcpSrvInitializer Singleton { get; private set; }

        private TerminatorProtocolServer _robotServer = new TerminatorProtocolServer();

        public AppSession SendToRobotServerCurrentSession;

        private IDevice<bool> _work1OnEvent;
        private IDevice<bool> _work2OnEvent;
        private IDevice<bool> _work3OnEvent;
        private IDevice<bool> _work4OnEvent;
        private IDevice<bool> _workInPositon;
        private Thread _opcThread;

        public EsponTcpSrvInitializer()
        {
            if (Singleton != null)
                return;

            Singleton = this;
        }

        private void InitOpc()
        {
            _opcThread = new Thread((() =>
            {
                while (true)
                {
                    _workInPositon = OpcInitializer.DeviceGroupProvider.RootDeviceGroup.GetDevice<bool>("WorkInPositon");
                    
                    bool bInPosition = _workInPositon.Read();

                    if (bInPosition)
                    {
                        MqInitializer.Bus.PublishAsync(new WorkpieceInPositionEvent()
                        {
                            StationIndex = 0,
                            DeviceName = "ABU",
                            WorkpieceTag = 0,
                            DateTime = DateTime.Now,
                        }, p => p.WithExpires(500));

                        _workInPositon.Write(false);
                    }
                     Thread.Sleep(100);
                }
            }));

            _opcThread.SetApartmentState(ApartmentState.STA);
            _opcThread.IsBackground = true;
            _opcThread.Start();

            Console.WriteLine("Bootstrapper.InitOpc(): _opcThread.Start()");
        }

        private void InitOpcData()
        {
            _work1OnEvent = OpcInitializer.DeviceGroupProvider.RootDeviceGroup.GetDevice<bool>("Work1");
            _work2OnEvent = OpcInitializer.DeviceGroupProvider.RootDeviceGroup.GetDevice<bool>("Work2");
            _work3OnEvent = OpcInitializer.DeviceGroupProvider.RootDeviceGroup.GetDevice<bool>("Work3");
            _work4OnEvent = OpcInitializer.DeviceGroupProvider.RootDeviceGroup.GetDevice<bool>("Work4");
        }

        private MeasureEvent _measureEvent;
        private void _robotServer_NewRequestReceived(AppSession session, StringRequestInfo requestInfo)
        {
            Console.WriteLine("_receiveFromRobotServer_NewRequestReceived.Key: " + requestInfo.Key);
            Console.WriteLine("_receiveFromRobotServer_NewRequestReceived.Body: " + requestInfo.Body);

            var key = requestInfo.Key.TrimStart('\n', '\r').TrimEnd('\n', '\r');
            Console.WriteLine("_receiveFromRobotServer_NewRequestReceived.Key(Trimmed): " + key);

            switch (key)
            {
                case "Coord":
                    var message = requestInfo.Body;

                    if (_measureEvent == null)
                        _measureEvent = new MeasureEvent() { Message = message };                   

                    if (message.Contains("PositionIndex") &&
                        message.Contains("PointIndex"))
                    {
                        var entries = message.Split(';');
                        _measureEvent.PositionIndex = int.Parse(entries.Single(p => p.StartsWith("PositionIndex"))
                            .Replace("PositionIndex=", ""));
                        _measureEvent.PointIndex = int.Parse(entries.Single(p => p.StartsWith("PointIndex"))
                            .Replace("PointIndex=", ""));

                        _measureEvent.X = double.Parse(entries.Single(p => p.StartsWith("CX"))
                            .Replace("CX=", ""));
                        _measureEvent.Y = double.Parse(entries.Single(p => p.StartsWith("CY"))
                            .Replace("CY=", ""));
                    }

                    _sensorInPositionEvent.OnNext(_measureEvent);

                    Debug.WriteLine("_receiveFromRobotServer Coord# received.");
                    break;

                case "CoordEnd":
                    _stationCompletedEvent.OnNext(new MeasureEvent { StationIndex = _measureEvent.StationIndex, Message = _measureEvent.Message});
                    Debug.WriteLine("_receiveFromRobotServer CoordEnd# received.");
                    break;

                default:
                    break;
            }
        }

        private void _robotServer_SessionClosed(AppSession session, CloseReason value)
        {
            Debug.WriteLine("_sendToRobotServer_sessionClosed");
            if (session == SendToRobotServerCurrentSession)
            {
                Debug.WriteLine("_sendToRobotServerCurrentSession equals. set to null.");
                SendToRobotServerCurrentSession = null;
            }
        }

        private void _robotServer_NewSessionConnected(AppSession session)
        {
            SendToRobotServerCurrentSession?.Close();
            SendToRobotServerCurrentSession = session;

            Debug.WriteLine("_sendToRobotServer connected.");
        }

        public bool IsSessionConnected
            => SendToRobotServerCurrentSession != null;

        public void Initialize()
        {
            InitOpc();

            InitOpcData();

            _robotServer = new TerminatorProtocolServer();

            if (!_robotServer.Setup(Port))
            {
                Debug.WriteLine("Failed to setup!");

                throw new Exception("Failed to setup!");
            }
            _robotServer.Start();

            Console.WriteLine("StarSend Is Ok");

            _robotServer.NewSessionConnected += _robotServer_NewSessionConnected;
            _robotServer.NewRequestReceived += _robotServer_NewRequestReceived;
            _robotServer.SessionClosed += _robotServer_SessionClosed;

            Console.WriteLine($"{nameof(EsponTcpSrvInitializer)}.Initialize() begin at " + DateTime.Now);

        }

        public void WorkInPositionCommand(MeasureEvent measureEvent)
        {
            var tasks = TcpTaskSchemas.Single(x => x.StationIndex == measureEvent.StationIndex);

            var isWorkPieceOn = true;

            switch (measureEvent.StationIndex)
            {
                case 1:
                    isWorkPieceOn = _work1OnEvent.Read();
                    break;
                case 2:
                    isWorkPieceOn = _work2OnEvent.Read();
                    break;
                case 3:
                    isWorkPieceOn = _work3OnEvent.Read();
                    break;
                case 4:
                    isWorkPieceOn = _work4OnEvent.Read();
                    break;
            }

            if (!isWorkPieceOn)
            {
                _stationCompletedEvent.OnNext(new MeasureEvent
                {
                    StationIndex = -1,
                    Message = measureEvent.Message
                });

                return;
            }

            SendToRobotServerCurrentSession?.Send(measureEvent.StationIndex.ToString());

            foreach (var task in tasks.TcpTaskSchema)
            {
                _measureEvent = new MeasureEvent
                {
                    PointIndex = task.Index,
                    StationIndex = measureEvent.StationIndex,
                    PositionIndex = measureEvent.PositionIndex,
                    Message = measureEvent.Message
                };

                _autoResetEvent.WaitOne(task.TimeOut);

                SendToRobotServerCurrentSession?.Send("30");

                Thread.Sleep(200);
            }
        }

        public void SensorInPositionCommand()
        {
            _autoResetEvent.Set();
        }

        [DefaultValue(2001)]
        public int Port { get; set; } = 2001;
    }
}
