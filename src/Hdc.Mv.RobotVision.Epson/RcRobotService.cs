//yx 20170409
using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using SuperSocket.SocketBase;
//using SpelNetLib70;
using RCAPINet;

namespace Hdc.Mv.RobotVision.HandEyeCalibrator
{
    public class RcRobotService : IRobotService
    {

        public Spel m_spel;
        public SpelPoint here;
        private RobotPoint _herePoint;
        private RobotPoint _OriToolInBasePoint;
        private RobotPoint _RefToolInBasePoint;

        private readonly Subject<RobotPoint> _robotPointUpdatedEvent = new Subject<RobotPoint>();
        public IObservable<RobotPoint> RobotPointUpdatedEvent => _robotPointUpdatedEvent;

        public Subject<bool> _SessionChangedEvent = new Subject<bool>();
        public IObservable<bool> SessionChangedEvent => _SessionChangedEvent;

        public void Init()
        {
            here = new SpelPoint();
            m_spel = new Spel();
            m_spel.Initialize();
            m_spel.Project = @"C:\EpsonRC70\projects\TestApi01\TestApi01.sprj";
            m_spel.MotorsOn = true;
        }

        public void StartAutoCalib()//need updata
        {
            float offset = 2;
            float offsetX, offsetY;

            m_spel.Here("OriToolInBase");
            var spelPoint = m_spel.GetPoint("OriToolInBase");
            spelPoint.X = spelPoint.X - offset;
            spelPoint.Y = spelPoint.Y - offset;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    offsetX = i * offset;
                    offsetY = j * offset;

                    var newPoint = new SpelPoint(spelPoint.X, spelPoint.Y, spelPoint.Z, spelPoint.U);
                    newPoint.Hand = spelPoint.Hand;

                    newPoint.X += offsetX;
                    newPoint.Y += offsetY;

                    m_spel.Go(newPoint);

                    _robotPointUpdatedEvent.OnNext(new RobotPoint()
                    {
                        Index = i * 3 + j,
                        BaseX = double.Parse(newPoint.X.ToString("F3")),
                        BaseY = double.Parse(newPoint.Y.ToString("F3")),
                        BaseZ = double.Parse(newPoint.Z.ToString("F3")),
                        BaseU = double.Parse(newPoint.U.ToString("F3")),
                    });
                }
            }

            _robotPointUpdatedEvent.OnNext(new RobotPoint()
            {
                Index = -1,
            });
        }

        public RobotPoint GetHere()
        {
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);

            RobotPoint newposition = new RobotPoint
            {
                BaseX = here.X,
                BaseY = here.Y,
                BaseZ = here.Z,
                BaseU = here.U
            };
            return newposition;
        }


        public RobotPoint GetOriToolInBase()
        {
            //*
            //_sendToRobotServerCurrentSession?.Send("GetOriToolInBase#");
            //var ret = _getOriToolInBasePointEvent.WaitOne(TimeSpan.FromSeconds(3));
            var ret = new RobotPoint();
            var mid = m_spel.GetPoint("OriToolInBase");
            ret.BaseX = mid.X;
            ret.BaseY = mid.Y;
            ret.BaseZ = mid.Z;
            ret.BaseU = mid.U;
            return ret;
            //*/

        }

        public RobotPoint GetRefToolInBase()
        {
            //_sendToRobotServerCurrentSession?.Send("GetRefToolInBase#");
            //var ret = _getRefToolInBasePointEvent.WaitOne(TimeSpan.FromSeconds(3));
            var ret = new RobotPoint(999.999, 999.999, 999.999, 999.999);
            return ret;
        }

        public void GoOriToolInBase()
        {
            m_spel.Go("OriToolInBase");//
        }

        public void GoRefToolInBase()
        {

        }

        public void GoHereXInc10mm()
        {

            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            here.X = here.X + 10;
            newposition.Y = here.Y;
            newposition.Z = here.Z;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereXDec10mm()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereXDec10mm#");
            //_GoHereXDec10mmEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X - 10;
            newposition.Y = here.Y;
            newposition.Z = here.Z;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereXInc1mm()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereXInc1mm#");
            //_GoHereXInc1mmEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X + 1;
            newposition.Y = here.Y;
            newposition.Z = here.Z;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereXDec1mm()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereXDec1mm#");
            //_GoHereXDec1mmEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X - 1;
            newposition.Y = here.Y;
            newposition.Z = here.Z;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereXInc100um()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereXInc100um#");
            //_GoHereXInc100umEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X + 0.1f;
            newposition.Y = here.Y;
            newposition.Z = here.Z;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereXDec100um()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereXDec100um#");
            //_GoHereXDec100umEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X - 0.1f;
            newposition.Y = here.Y;
            newposition.Z = here.Z;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereXInc10um()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereXInc10um#");
            //_GoHereXInc10umEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X + 0.01f;
            newposition.Y = here.Y;
            newposition.Z = here.Z;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereXDec10um()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereXDec10um#");
            //_GoHereXDec10umEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X - 0.01f;
            newposition.Y = here.Y;
            newposition.Z = here.Z;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereYInc10mm()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereYInc10mm#");
            //_GoHereYInc10mmEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X;
            newposition.Y = here.Y + 10;
            newposition.Z = here.Z;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereYDec10mm()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereYDec10mm#");
            //_GoHereYDec10mmEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X;
            newposition.Y = here.Y - 10;
            newposition.Z = here.Z;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereYInc1mm()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereYInc1mm#");
            //_GoHereYInc1mmEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X;
            newposition.Y = here.Y + 1;
            newposition.Z = here.Z;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereYDec1mm()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereYDec1mm#");
            //_GoHereYDec1mmEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X;
            newposition.Y = here.Y - 1;
            newposition.Z = here.Z;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereYInc100um()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereYInc100um#");
            //_GoHereYInc100umEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X;
            newposition.Y = here.Y + 0.1f;
            newposition.Z = here.Z;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereYDec100um()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereYDec100um#");
            //_GoHereYDec100umEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X;
            newposition.Y = here.Y - 0.1f;
            newposition.Z = here.Z;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereYInc10um()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereYInc10um#");
            //_GoHereYInc10umEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X;
            newposition.Y = here.Y + 0.01f;
            newposition.Z = here.Z;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereYDec10um()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereYDec10um#");
            //_GoHereYDec10umEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X;
            newposition.Y = here.Y - 0.01f;
            newposition.Z = here.Z;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void OpenRobotManager()
        {
           // _sendToRobotServerCurrentSession?.Send("OpenRobotManager#");
           // _OpenRobotManagerEvent.WaitOne(TimeSpan.FromSeconds(3));
            m_spel.RunDialog(SpelDialogs.RobotManager);
        }

        public void GoHereZInc1mm()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereZInc1mm#");
            //_GoHereZInc1mmEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X;
            newposition.Y = here.Y;
            newposition.Z = here.Z + 1;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereZDec1mm()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereZDec1mm#");
            //_GoHereZDec1mmEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X;
            newposition.Y = here.Y;
            newposition.Z = here.Z - 1;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereZInc100um()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereZInc100um#");
            //_GoHereZInc100umEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X;
            newposition.Y = here.Y;
            newposition.Z = here.Z + 0.1f;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereZDec100um()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereZDec100um#");
            //_GoHereZDec100umEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X;
            newposition.Y = here.Y;
            newposition.Z = here.Z - 0.1f;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereZInc10um()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereZInc100um#");
            //_GoHereZInc100umEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X;
            newposition.Y = here.Y;
            newposition.Z = here.Z + 0.01f;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }

        public void GoHereZDec10um()
        {
            //_sendToRobotServerCurrentSession?.Send("GoHereZDec10um#");
            //_GoHereZDec10umEvent.WaitOne(TimeSpan.FromSeconds(3));
            SpelPoint newposition = new SpelPoint();
            m_spel.Here(999);//999点作为中间变量
            here = m_spel.GetPoint(999);
            newposition.X = here.X;
            newposition.Y = here.Y;
            newposition.Z = here.Z - 0.01f;
            newposition.U = here.U;
            newposition.Hand = here.Hand;
            m_spel.Go(newposition);
        }


        //public bool IsSessionConnected
        //    => _sendToRobotServerCurrentSession != null && _receiveFromRobotServerCurrentSession != null;
    }
}