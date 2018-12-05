using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Threading;
using SuperSocket.SocketBase;

namespace Hdc.Mv.RobotVision.HandEyeCalibrator
{
    public class SuperSocketRobotService : IRobotService
    {
        /// <summary>
        /// epson, #203, tcpip_out_data, port: 2002
        /// </summary>
        private readonly TerminatorProtocolServer _receiveFromRobotServer = new TerminatorProtocolServer();

        /// <summary>
        /// epson, #204, tcpip_in_data, port: 2003
        /// </summary>
        private readonly TerminatorProtocolServer _sendToRobotServer = new TerminatorProtocolServer();

        private AppSession _receiveFromRobotServerCurrentSession;
        private AppSession _sendToRobotServerCurrentSession;

        private readonly Subject<RobotPoint> _robotPointUpdatedEvent = new Subject<RobotPoint>();

        public Subject<bool> _SessionChangedEvent = new Subject<bool>();

        public IObservable<RobotPoint> RobotPointUpdatedEvent => _robotPointUpdatedEvent;
        public IObservable<bool> SessionChangedEvent => _SessionChangedEvent;

        private readonly AutoResetEvent _getHerePointEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _getOriToolInBasePointEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _getRefToolInBasePointEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoOriToolInBaseEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoRefToolInBaseEvent = new AutoResetEvent(false);

        private readonly AutoResetEvent _GoHereXInc10mmEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereXDec10mmEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereXInc1mmEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereXDec1mmEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereXInc100umEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereXDec100umEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereXInc10umEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereXDec10umEvent = new AutoResetEvent(false);

        private readonly AutoResetEvent _GoHereYInc10mmEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereYDec10mmEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereYInc1mmEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereYDec1mmEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereYInc100umEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereYDec100umEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereYInc10umEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereYDec10umEvent = new AutoResetEvent(false);

        private readonly AutoResetEvent _GoHereZInc1mmEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereZDec1mmEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereZInc100umEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereZDec100umEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereZInc10umEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _GoHereZDec10umEvent = new AutoResetEvent(false);
        private RobotPoint _herePoint;
        private RobotPoint _OriToolInBasePoint;
        private RobotPoint _RefToolInBasePoint;

        public void Init()
        {
            if (!_receiveFromRobotServer.Setup(2002)) //Setup with listening port
            {
                Debug.WriteLine("Failed to setup!");

                throw new Exception("Failed to setup!");
            }

            if (!_sendToRobotServer.Setup(2003)) //Setup with listening port
            {
                Debug.WriteLine("Failed to setup!");

                throw new Exception("Failed to setup!");
            }

            _receiveFromRobotServer.NewSessionConnected += _receiveFromRobotServer_NewSessionConnected;
            _receiveFromRobotServer.NewRequestReceived += _receiveFromRobotServer_NewRequestReceived;
            _receiveFromRobotServer.SessionClosed += _receiveFromRobotServer_SessionClosed;

            _sendToRobotServer.NewSessionConnected += _sendToRobotServer_NewSessionConnected;
            _sendToRobotServer.SessionClosed += _sendToRobotServer_SessionClosed;

            if (!_receiveFromRobotServer.Start())
            {
                Debug.WriteLine("Failed to start!");

                throw new Exception("Failed to start!");
            }
            Debug.WriteLine("_receiveFromRobotServer started.");

            if (!_sendToRobotServer.Start())
            {
                Debug.WriteLine("Failed to start!");

                throw new Exception("Failed to start!");
            }
            Debug.WriteLine("_sendToRobotServer started.");
        }

        private void _receiveFromRobotServer_SessionClosed(AppSession session, CloseReason value)
        {
            Debug.WriteLine("_receiveFromRobotServer_SessionClosed");

            if (session == _receiveFromRobotServerCurrentSession)
            {
                Debug.WriteLine("_receiveFromRobotServerCurrentSession equals. set to null.");
                _receiveFromRobotServerCurrentSession = null;
            }

            _SessionChangedEvent.OnNext(IsSessionConnected);
        }

        private void _sendToRobotServer_SessionClosed(AppSession session, CloseReason value)
        {
            Debug.WriteLine("_sendToRobotServer_SessionClosed");

            if (session == _sendToRobotServerCurrentSession)
            {
                Debug.WriteLine("_sendToRobotServerCurrentSession equals. set to null.");
                _sendToRobotServerCurrentSession = null;
            }

            _SessionChangedEvent.OnNext(IsSessionConnected);
        }

        private void _receiveFromRobotServer_NewRequestReceived(AppSession session,
            SuperSocket.SocketBase.Protocol.StringRequestInfo requestInfo)
        {
            Console.WriteLine("_receiveFromRobotServer_NewRequestReceived.Key: " + requestInfo.Key);
            Console.WriteLine("_receiveFromRobotServer_NewRequestReceived.Body: " + requestInfo.Body);
            //session.SocketSession.
            var key = requestInfo.Key.TrimStart('\n', '\r').TrimEnd('\n', '\r');
            Console.WriteLine("_receiveFromRobotServer_NewRequestReceived.Key(Trimmed): " + key);

            switch (key)
            {
                case "Coord":
                    var message = requestInfo.Body;
                    var entrys = message.Split(',');
                    var indexString = entrys[0].Split(':')[1];
                    var xString = entrys[1].Split(':')[1];
                    var yString = entrys[2].Split(':')[1];
                    var zString = entrys[3].Split(':')[1];
                    var uString = entrys[4].Split(':')[1];
                    var indexValue = int.Parse(indexString);
                    var xValue = double.Parse(xString);
                    var yValue = double.Parse(yString);
                    var zValue = double.Parse(zString);
                    var uValue = double.Parse(uString);

                    Debug.WriteLine("_receiveFromRobotServer Coord# received.");
                    _robotPointUpdatedEvent.OnNext(new RobotPoint()
                    {
                        Index = indexValue,
                        BaseX = xValue,
                        BaseY = yValue,
                        BaseZ = zValue,
                        BaseU = uValue,
                    });

                    _sendToRobotServerCurrentSession?.Send("CoordAck#");
                    Debug.WriteLine("_sendToRobotServerCurrentSession CoordAck# sent.");
                    break;
                case "CoordEnd":
                    Debug.WriteLine("_receiveFromRobotServer CoordEnd# received.");
                    _robotPointUpdatedEvent.OnNext(new RobotPoint()
                    {
                        Index = -1,
                    });

                    _sendToRobotServerCurrentSession?.Send("CoordEndAck#");
                    Debug.WriteLine("_sendToRobotServerCurrentSession CoordEndAck# sent.");
                    break;
                case "GetHereAck":
                    Debug.WriteLine("_receiveFromRobotServer GetHereAck# received.");

                    var entrys2 = requestInfo.Body.Split(',');

                    _herePoint = new RobotPoint()
                    {
                        BaseX = double.Parse(entrys2[0].Split(':')[1]),
                        BaseY = double.Parse(entrys2[1].Split(':')[1]),
                        BaseZ = double.Parse(entrys2[2].Split(':')[1]),
                        BaseU = double.Parse(entrys2[3].Split(':')[1]),
                    };

                    _getHerePointEvent.Set();
                    break;
                case "GetOriToolInBaseAck":
                    Debug.WriteLine("_receiveFromRobotServer GetOriToolInBasePointAck# received.");

                    _OriToolInBasePoint = new RobotPoint()
                    {
                        BaseX = double.Parse(requestInfo.Body.Split(',')[0].Split(':')[1]),
                        BaseY = double.Parse(requestInfo.Body.Split(',')[1].Split(':')[1]),
                        BaseZ = double.Parse(requestInfo.Body.Split(',')[2].Split(':')[1]),
                        BaseU = double.Parse(requestInfo.Body.Split(',')[3].Split(':')[1]),
                    };

                    _getOriToolInBasePointEvent.Set();
                    break;
                case "GetRefToolInBaseAck":
                    Debug.WriteLine("_receiveFromRobotServer GetRefToolInBasePointAck# received.");

                    _RefToolInBasePoint = new RobotPoint()
                    {
                        BaseX = double.Parse(requestInfo.Body.Split(',')[0].Split(':')[1]),
                        BaseY = double.Parse(requestInfo.Body.Split(',')[1].Split(':')[1]),
                        BaseZ = double.Parse(requestInfo.Body.Split(',')[2].Split(':')[1]),
                        BaseU = double.Parse(requestInfo.Body.Split(',')[3].Split(':')[1]),
                    };

                    _getRefToolInBasePointEvent.Set();
                    break;
                case "GoOriToolInBaseAck":
                    Debug.WriteLine("_receiveFromRobotServer GoOriToolInBaseAck# received.");
                    _GoOriToolInBaseEvent.Set();
                    break;
                case "GoRefToolInBaseAck":
                    Debug.WriteLine("_receiveFromRobotServer GoRefToolInBaseAck# received.");
                    _GoRefToolInBaseEvent.Set();
                    break;
                case "GoHereXInc10mm":
                    Debug.WriteLine("_receiveFromRobotServer GoHereXInc10mm# received.");
                    _GoHereXInc10mmEvent.Set();
                    break;
                case "GoHereXDec10mm":
                    Debug.WriteLine("_receiveFromRobotServer GoHereXDec10mm# received.");
                    _GoHereXDec10mmEvent.Set();
                    break;
                case "GoHereXInc1mm":
                    Debug.WriteLine("_receiveFromRobotServer GoHereXInc1mm# received.");
                    _GoHereXInc1mmEvent.Set();
                    break;
                case "GoHereXDec1mm":
                    Debug.WriteLine("_receiveFromRobotServer GoHereXDec1mm# received.");
                    _GoHereXDec1mmEvent.Set();
                    break;
                case "GoHereXInc100um":
                    Debug.WriteLine("_receiveFromRobotServer GoHereXInc100um# received.");
                    _GoHereXInc100umEvent.Set();
                    break;
                case "GoHereXDec100um":
                    Debug.WriteLine("_receiveFromRobotServer GoHereXDec100um# received.");
                    _GoHereXDec100umEvent.Set();
                    break;
                case "GoHereXInc10um":
                    Debug.WriteLine("_receiveFromRobotServer GoHereXInc10um# received.");
                    _GoHereXInc10umEvent.Set();
                    break;
                case "GoHereXDec10um":
                    Debug.WriteLine("_receiveFromRobotServer GoHereXDec10um# received.");
                    _GoHereXDec10umEvent.Set();
                    break;
                case "GoHereYInc10mm":
                    Debug.WriteLine("_receiveFromRobotServer GoHereYInc10mm# received.");
                    _GoHereYInc10mmEvent.Set();
                    break;
                case "GoHereYDec10mm":
                    Debug.WriteLine("_receiveFromRobotServer GoHereYDec10mm# received.");
                    _GoHereYDec10mmEvent.Set();
                    break;
                case "GoHereYInc1mm":
                    Debug.WriteLine("_receiveFromRobotServer GoHereYInc1mm# received.");
                    _GoHereYInc1mmEvent.Set();
                    break;
                case "GoHereYDec1mm":
                    Debug.WriteLine("_receiveFromRobotServer GoHereYDec1mm# received.");
                    _GoHereYDec1mmEvent.Set();
                    break;
                case "GoHereYInc100um":
                    Debug.WriteLine("_receiveFromRobotServer GoHereYInc100um# received.");
                    _GoHereYInc100umEvent.Set();
                    break;
                case "GoHereYDec100um":
                    Debug.WriteLine("_receiveFromRobotServer GoHereYDec100um# received.");
                    _GoHereYDec100umEvent.Set();
                    break;
                case "GoHereYInc10um":
                    Debug.WriteLine("_receiveFromRobotServer GoHereYInc10um# received.");
                    _GoHereYInc10umEvent.Set();
                    break;
                case "GoHereYDec10um":
                    Debug.WriteLine("_receiveFromRobotServer GoHereYDec10um# received.");
                    _GoHereYDec10umEvent.Set();
                    break;
                case "GoHereZInc1mm":
                    Debug.WriteLine("_receiveFromRobotServer GoHereZInc1mm# received.");
                    _GoHereYInc1mmEvent.Set();
                    break;
                case "GoHereZDec1mm":
                    Debug.WriteLine("_receiveFromRobotServer GoHereZDec1mm# received.");
                    _GoHereYDec1mmEvent.Set();
                    break;
                case "GoHereZInc100um":
                    Debug.WriteLine("_receiveFromRobotServer GoHereZInc100um# received.");
                    _GoHereZInc100umEvent.Set();
                    break;
                case "GoHereZDec100um":
                    Debug.WriteLine("_receiveFromRobotServer GoHereZDec100um# received.");
                    _GoHereZDec100umEvent.Set();
                    break;
                case "GoHereZInc10um":
                    Debug.WriteLine("_receiveFromRobotServer GoHereZInc10um# received.");
                    _GoHereZInc10umEvent.Set();
                    break;
                case "GoHereZDec10um":
                    Debug.WriteLine("_receiveFromRobotServer GoHereZDec10um# received.");
                    _GoHereZDec10umEvent.Set();
                    break;
                default:
                    break;
            }
        }

        private void _receiveFromRobotServer_NewSessionConnected(AppSession session)
        {
            _receiveFromRobotServerCurrentSession?.Close();
            _receiveFromRobotServerCurrentSession = session;

            Debug.WriteLine("_receiveFromRobotServer connected.");

            _SessionChangedEvent.OnNext(IsSessionConnected);
        }

        private void _sendToRobotServer_NewSessionConnected(AppSession session)
        {
            _sendToRobotServerCurrentSession?.Close();
            _sendToRobotServerCurrentSession = session;

            Debug.WriteLine("_sendToRobotServer connected.");

            _SessionChangedEvent.OnNext(IsSessionConnected);
        }

        public void StartAutoCalib()
        {
            _sendToRobotServerCurrentSession?.Send("AutoCalib#");
        }

        public RobotPoint GetHere()
        {
            _sendToRobotServerCurrentSession?.Send("GetHere#");
            var ret = _getHerePointEvent.WaitOne(TimeSpan.FromSeconds(3));
            return ret ? _herePoint : new RobotPoint(999.999,999.999,999.999,999.999);
        }

        public RobotPoint GetOriToolInBase()
        {
            _sendToRobotServerCurrentSession?.Send("GetOriToolInBase#");
            var ret = _getOriToolInBasePointEvent.WaitOne(TimeSpan.FromSeconds(3));
            return ret ? _OriToolInBasePoint : new RobotPoint(999.999, 999.999, 999.999, 999.999);
        }

        public RobotPoint GetRefToolInBase()
        {
            _sendToRobotServerCurrentSession?.Send("GetRefToolInBase#");
            var ret = _getRefToolInBasePointEvent.WaitOne(TimeSpan.FromSeconds(3));
            return ret ? _RefToolInBasePoint : new RobotPoint(999.999, 999.999, 999.999, 999.999);
        }

        public void GoOriToolInBase()
        {
            _sendToRobotServerCurrentSession?.Send("GoOriToolInBase#");
             _GoOriToolInBaseEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoRefToolInBase()
        {
            _sendToRobotServerCurrentSession?.Send("GoRefToolInBase#");
            _GoRefToolInBaseEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereXInc10mm()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereXInc10mm#");
            _GoHereXInc10mmEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereXDec10mm()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereXDec10mm#");
            _GoHereXDec10mmEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereXInc1mm()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereXInc1mm#");
            _GoHereXInc1mmEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereXDec1mm()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereXDec1mm#");
            _GoHereXDec1mmEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereXInc100um()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereXInc100um#");
            _GoHereXInc100umEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereXDec100um()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereXDec100um#");
            _GoHereXDec100umEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereXInc10um()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereXInc10um#");
            _GoHereXInc10umEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereXDec10um()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereXDec10um#");
            _GoHereXDec10umEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereYInc10mm()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereYInc10mm#");
            _GoHereYInc10mmEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereYDec10mm()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereYDec10mm#");
            _GoHereYDec10mmEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereYInc1mm()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereYInc1mm#");
            _GoHereYInc1mmEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereYDec1mm()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereYDec1mm#");
            _GoHereYDec1mmEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereYInc100um()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereYInc100um#");
            _GoHereYInc100umEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereYDec100um()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereYDec100um#");
            _GoHereYDec100umEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereYInc10um()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereYInc10um#");
            _GoHereYInc10umEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereYDec10um()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereYDec10um#");
            _GoHereYDec10umEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void OpenRobotManager()
        {

        }

        public void GoHereZInc1mm()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereZInc1mm#");
            _GoHereZInc1mmEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereZDec1mm()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereZDec1mm#");
            _GoHereZDec1mmEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereZInc100um()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereZInc100um#");
            _GoHereZInc100umEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereZDec100um()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereZDec100um#");
            _GoHereZDec100umEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereZInc10um()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereZInc10um#");
            _GoHereZInc10umEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public void GoHereZDec10um()
        {
            _sendToRobotServerCurrentSession?.Send("GoHereZDec10um#");
            _GoHereZDec10umEvent.WaitOne(TimeSpan.FromSeconds(3));
        }

        public bool IsSessionConnected
            => _sendToRobotServerCurrentSession != null && _receiveFromRobotServerCurrentSession != null;
    }
}