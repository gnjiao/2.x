using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Windows.Markup;
using Hdc.Mercury;
using Hdc.Mv.LocatingService;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty(nameof(CommonTaskSchemas))]
    public class CommonTcpSrvInitializer : IInitializer
    {
        private readonly Subject<MeasureEvent> _sensorInPositionEvent = new Subject<MeasureEvent>();
        private readonly Subject<MeasureEvent> _stationCompletedEvent = new Subject<MeasureEvent>();
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        
        private Socket _sendToRobotclient;
        public Thread RxThread;
        private bool _endRxThread = false;

        public IObservable<MeasureEvent> SensorInPositionEvent => _sensorInPositionEvent;
        public IObservable<MeasureEvent> StationCompletedEvent => _stationCompletedEvent;
        public Collection<CommonTaskSchemas> CommonTaskSchemas { get; set; } = new Collection<CommonTaskSchemas>();
        public static CommonTcpSrvInitializer Singleton { get; private set; }        
        public int Port { get; set; } = 2002;
        public string Hostname { get; set; } = "192.168.0.1";        

        public CommonTcpSrvInitializer()
        {
            if (Singleton != null)
                return;

            Singleton = this;
        }
        
        private MeasureEvent _measureEvent;        

        public bool IsSessionConnected
            => _sendToRobotclient != null;

        public void Initialize()
        {
            SocketConnectServer();

            Console.WriteLine($"{nameof(EsponTcpSrvInitializer)}.Initialize() begin at " + DateTime.Now);
        }

        private void SocketConnectServer()
        {
            if (_sendToRobotclient != null)
            {
                _sendToRobotclient.Close();
                _endRxThread = true;
                RxThread.Join();
            }

            _sendToRobotclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _sendToRobotclient.Connect(new IPEndPoint(IPAddress.Parse(Hostname), Port));

            RxThread = new Thread(new ThreadStart(ReceiveProcess));
            RxThread.Start();
        }

        private void ReceiveProcess()
        {
            while (!_endRxThread)
            {
                var buffer = new byte[1024];
                var recv = _sendToRobotclient.Receive(buffer);
                var data = Encoding.Default.GetString(buffer, 0, recv);

                var key = data.TrimStart('\n', '\r').TrimEnd('\n', '\r');

                Console.WriteLine("_receiveFromRobotServer_NewRequestReceived.Key(Trimmed): " + key);

                switch (key)
                {
                    case "OK#":

                        var message = _measureEvent.Message;

                        if (_measureEvent == null)
                            _measureEvent = new MeasureEvent() { Message = message };
                        
                        _sensorInPositionEvent.OnNext(_measureEvent);

                        Debug.WriteLine("_receiveFromRobotServer OK# received.");

                        break;

                    case "Done#":

                         _autoResetEvent.Set();

                        break;

                    default:
                        break;
                }
                //receiveMsg.AppendText(stringdata + "\r\n");

                Thread.Sleep(100);
            }
        }

        public void WorkInPositionCommand(MeasureEvent measureEvent)
        {
            var tasks = CommonTaskSchemas.Single(x => x.StationIndex == measureEvent.StationIndex);

            foreach (var task in tasks.CommonTaskSchema)
            {
                var operation = task.Operation;
                var disableSensorInPositionEvent = task.DisableSensorInPositionEvent;

                _sendToRobotclient?.Send(System.Text.Encoding.Default.GetBytes($"{operation}\r\n"));

                _measureEvent = new MeasureEvent
                {
                    PointIndex = task.Index,
                    StationIndex = measureEvent.StationIndex,
                    PositionIndex = measureEvent.PositionIndex,
                    Message = measureEvent.Message
                };

                //if (!disableSensorInPositionEvent)
                _autoResetEvent.WaitOne(task.TimeOut);

                Thread.Sleep(100);
            }

            _stationCompletedEvent.OnNext(new MeasureEvent()
            {
                StationIndex = measureEvent.StationIndex,
                Message = _measureEvent.Message
            });
        }

        public void SensorInPositionCommand()
        {
            _autoResetEvent.Set();
        }

    }
}
