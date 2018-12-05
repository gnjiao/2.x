using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows.Markup;
using Hdc.Mv.RobotVision;
using RCAPINet;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("RobotPointSchemas")]
    public class XyRcApiInitializer : IInitializer
    {
        private readonly Subject<MeasureEvent> _sensorInPositionEvent = new Subject<MeasureEvent>();
        private readonly Subject<MeasureEvent> _stationCompletedEvent = new Subject<MeasureEvent>();
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        private Spel _spel;
        private readonly Dictionary<int, SpelPoint> _pointsInRobot = new Dictionary<int, SpelPoint>();

        public static XyRcApiInitializer Singleton { get; private set; }

        public XyRcApiInitializer()
        {
            if (Singleton != null)
                return;

            Singleton = this;
        }

        public void Initialize()
        {
            _spel = new Spel();
            _spel.Initialize();
            _spel.Connect(ConnectionNumber);

            _spel.EventReceived += (o, args) =>
            {
                Console.WriteLine("_spel.EventReceived: Event={0}, Message={1}", args.Event, args.Message);
            };

            _spel.EnableEvent(SpelEvents.Error, true);
            _spel.EnableEvent(SpelEvents.AllTasksStopped, true);
            _spel.EnableEvent(SpelEvents.Print, true);

            _spel.Reset();            
            _spel.MotorsOn = true;

            Thread.Sleep(1000);

            GetPointsInRobot();

            _spel.LimZ(LimZ);
            _spel.PowerHigh = false;
            _spel.Speed(SpeedInit);
            _spel.Accel(AccelInit, AccelInit);

            if (Origin_UsePointInRobot_Enabled)
            {
                var spelPoint = _spel.GetPoint(Origin_UsePointInRobot_PointIndex);
                _spel.Jump(Origin_UsePointInRobot_PointIndex);
            }
            else
                _spel.Jump(OriginSpelPoint);

            _spel.PowerHigh = PowerHigh;
            _spel.Fine(J1MaxErr, J2MaxErr, J3MaxErr, J4MaxErr,0,0);
            Console.WriteLine($"_spel.Fine({J1MaxErr},{J2MaxErr},{J3MaxErr},{J4MaxErr},0,0)");

            _spel.Speed(SpeedMeasure);
            _spel.Accel(AccelMeasure, AccelMeasure);

            Console.WriteLine("_spel.Start(0)");
        }

        private void GetPointsInRobot()
        {
            Console.WriteLine($"---------- GetPointsInRobot: BEGIN ----------");

            if (Origin_UsePointInRobot_Enabled)
            {
                Console.WriteLine($"Origin_UsePointInRobot_Enabled = true");
                var spelPoint = _spel.GetPoint(Origin_UsePointInRobot_PointIndex);
                _pointsInRobot.Add(Origin_UsePointInRobot_PointIndex, spelPoint);
                Console.WriteLine($"----_spel.GetPoint({Origin_UsePointInRobot_PointIndex})");

                OriginX = spelPoint.X;
                OriginY = spelPoint.Y;
                OriginZ = spelPoint.Z;
                OriginU = spelPoint.U;

                Console.WriteLine($"----Origin updated: " +
                                  $"PointIndex({Origin_UsePointInRobot_PointIndex}), \n ----" +
                                  $"X({OriginX:0000.0000})," +
                                  $"Y({OriginY:0000.0000})," +
                                  $"Z({OriginZ:0000.0000})," +
                                  $"U({OriginU:0000.0000})");
            }

            foreach (var robotPointSchema in RobotPointSchemas)
            {
                foreach (var robotPoint in robotPointSchema.RobotPoints)
                {
                    if (robotPoint.UsePointInRobot_Enabled)
                    {
                        Console.WriteLine($"----------------------------------------------------");
                        Console.WriteLine($"RobotPoint[{robotPoint.Index}].UsePointInRobot_Enabled = true");

                        SpelPoint spelPoint;
                        if (_pointsInRobot.ContainsKey(robotPoint.UsePointInRobot_PointIndex))
                        {
                            Console.WriteLine($"----_pointsInRobot.ContainsKey({robotPoint.UsePointInRobot_PointIndex}) = true");
                            spelPoint = _pointsInRobot[robotPoint.UsePointInRobot_PointIndex];
                            Console.WriteLine($"----_pointsInRobot[{robotPoint.UsePointInRobot_PointIndex}]");
                        }
                        else
                        {
                            Console.WriteLine($"----_pointsInRobot.ContainsKey({robotPoint.UsePointInRobot_PointIndex}) = false");
                            spelPoint = _spel.GetPoint(robotPoint.UsePointInRobot_PointIndex);
                            _pointsInRobot.Add(robotPoint.UsePointInRobot_PointIndex, spelPoint);
                            Console.WriteLine($"----_spel.GetPoint({robotPoint.UsePointInRobot_PointIndex})");
                        }

                        robotPoint.BaseX = spelPoint.X;
                        robotPoint.BaseY = spelPoint.Y;
                        robotPoint.BaseZ = spelPoint.Z;
                        robotPoint.BaseU = spelPoint.U;

                        Console.WriteLine($"----RobotPoint[{robotPoint.Index}] updated: " +
                                          $"PointIndex({robotPoint.UsePointInRobot_PointIndex}), \n ----" +
                                          $"X({robotPoint.BaseX:0000.0000})," +
                                          $"Y({robotPoint.BaseY:0000.0000})," +
                                          $"Z({robotPoint.BaseZ:0000.0000})," +
                                          $"U({robotPoint.BaseU:0000.0000})");
                    }
                }
            }

            Console.WriteLine($"---------- GetPointsInRobot: END ----------");
        }

        private SpelPoint OriginSpelPoint
        {
            get
            {
                var spelPoint = new SpelPoint(OriginX, OriginY, OriginZ, OriginU)
                {
                    Hand = OriginHandRight.ToSpelHand()
                };
                return spelPoint;
            }
        }

        public void WorkInPositionCommand(int stationIndex)
        {
            var pointSchema = RobotPointSchemas.Single(x=>x.StationIndex == stationIndex);

            foreach (var robotPoint in pointSchema.RobotPoints)
            {
                var spelPoint = new SpelPoint()
                {
                    X = (float)robotPoint.BaseX,
                    Y = (float)robotPoint.BaseY,
                    Z = (float)robotPoint.BaseZ,
                    U = (float)robotPoint.BaseU,
                    Hand = OriginHandRight.ToSpelHand(),
                };

                if(robotPoint.SpeedBefore>0)
                    _spel.Speed(robotPoint.SpeedBefore);

                if(robotPoint.AccelBefore>0)
                    _spel.Accel(robotPoint.AccelBefore, robotPoint.AccelBefore);

                switch (robotPoint.RobotMoveType)
                {
                    case RobotMoveType.Go:
                        if(robotPoint.UsePointInRobot_Enabled)
                            _spel.Go(robotPoint.UsePointInRobot_PointIndex);
                        else
                            _spel.Go(spelPoint);
                        break;
                    case RobotMoveType.Move:
                        if (robotPoint.UsePointInRobot_Enabled)
                            _spel.Move(robotPoint.UsePointInRobot_PointIndex);
                        else
                            _spel.Move(spelPoint, "");
                        break;
                    case RobotMoveType.Pass:
                        if (robotPoint.UsePointInRobot_Enabled)
                        {
                            robotPoint.DisableSensorInPositionEvent = true;
                            _spel.Pass(robotPoint.UsePointInRobot_PointIndex);
                        }
                        else
                            throw new InvalidOperationException(
                                $"RobotMoveType.Pass must be UsePointInRobot_Enabled = true");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (robotPoint.SpeedAfter > 0)
                    _spel.Speed(robotPoint.SpeedAfter);

                if (robotPoint.AccelAfter > 0)
                    _spel.Accel(robotPoint.AccelAfter, robotPoint.AccelAfter);

                if (!robotPoint.DisableSensorInPositionEvent)
                {
                    _sensorInPositionEvent.OnNext(new MeasureEvent()
                    {
                        StationIndex = stationIndex,
                        PointIndex = robotPoint.Index,
                        PositionIndex = -999,
                        X = robotPoint.BaseX,
                        Y = robotPoint.BaseY,
                        Z = robotPoint.BaseZ,
                        U = robotPoint.BaseU,
                    });
                    
                    _autoResetEvent.WaitOne(robotPoint.Timeout);                    
                }
                else
                {
                    if(robotPoint.Timeout > 0)
                        _autoResetEvent.WaitOne(robotPoint.Timeout);
                }
            }

            _stationCompletedEvent.OnNext(new MeasureEvent() {StationIndex = stationIndex});
        }

        public IObservable<MeasureEvent> SensorInPositionEvent => _sensorInPositionEvent;

        public IObservable<MeasureEvent> StationCompletedEvent => _stationCompletedEvent;

        public int ConnectionNumber { get; set; } = 1;

        public int SpeedInit { get; set; } = 30;
        public int AccelInit { get; set; } = 30;

        public int SpeedMeasure { get; set; } = 60;
        public int AccelMeasure { get; set; } = 60;

        public float OriginX { get; set; } 
        public float OriginY { get; set; } 
        public float OriginZ { get; set; } 
        public float OriginU { get; set; } 
        public bool OriginHandRight { get; set; }
        public bool Origin_UsePointInRobot_Enabled { get; set; }
        public int Origin_UsePointInRobot_PointIndex { get; set; }

        public float LimZ { get; set; }
        public int J1MaxErr { get; set; } = 1000;
        public int J2MaxErr { get; set; } = 1000;
        public int J3MaxErr { get; set; } = 1000;
        public int J4MaxErr { get; set; } = 1000;

        /// <summary>
        /// false: PoweLow, true: PowerHigh
        /// </summary>
        public bool PowerHigh { get; set; } = true;

        public void SensorInPositionCommand()
        {
            _autoResetEvent.Set();
        }

        public Collection<RobotPointSchema> RobotPointSchemas { get; set; } = new Collection<RobotPointSchema>();

        public void SetIoOn(bool onOff, int bitNumber)
        {
            if (onOff)
            {
                _spel.On(bitNumber);
            }
            else
                _spel.Off(bitNumber);
        }

        public void SetIoOn(bool onOff, string label)
        {
            if (onOff)
            {
                _spel.On(label);
            }
            else
                _spel.Off(label);
        }
    }
}