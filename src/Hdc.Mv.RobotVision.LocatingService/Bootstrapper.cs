using System;
using System.IO;
using System.Windows;
using HalconDotNet;
using Hdc.IO;
using Hdc.Mv.Halcon;
using Hdc.Mv.ImageAcquisition;
using Hdc.Mv.ImageAcquisition.Halcon;
using Hdc.Mv.Inspection;
using Hdc.Mv.RobotVision;
using Hdc.Mv.RobotVision.LocatingService;
using Hdc.Reflection;
using Hdc.Serialization;
using SuperSocket.SocketBase;

namespace Hdc.Mv.LocatingService
{
    public class Bootstrapper
    {
        /// <summary>
        /// epson, #202, tcpip_out_data, port: 2001
        /// </summary>
        private readonly TerminatorProtocolServer _receiveFromRobotServer = new TerminatorProtocolServer();

        /// <summary>
        /// epson, #201, tcpip_in_data, port: 2000
        /// </summary>
        private readonly TerminatorProtocolServer _sendToRobotServer = new TerminatorProtocolServer();

        private AppSession _receiveFromRobotServerCurrentSession;
        private AppSession _sendToRobotServerCurrentSession;
        private readonly HalconCamera _camera = new HalconCamera();
        private readonly HalconInspectionSchemaInspector _oriInspector;
        private readonly HalconInspectionSchemaInspector _refInspector;
        private InspectionResult _oriResult;
        private InspectionResult _refResult;
        private readonly HHomConverter _converter;
        private readonly HandEyeCalibrationSchema _calibrationSchema;

        public Bootstrapper()
        {
            var dir = typeof (Bootstrapper).Assembly.GetAssemblyDirectoryPath();
            var calibrationSchemaFileName = Path.Combine(dir, Config.Mv_HandEyeCalibrationSchemaFileName);
            _calibrationSchema = calibrationSchemaFileName.DeserializeFromXamlFile<HandEyeCalibrationSchema>();

            _converter = new HHomConverter(_calibrationSchema);

            // display CalibrationSchema
            Console.WriteLine($"--CalibrationSchema.CalibToolInBaseVector = {_calibrationSchema.CalibToolInBaseVector}");
            Console.WriteLine($"--CalibrationSchema.OriToolInBaseVector = {_calibrationSchema.OriToolInBaseVector}");
            Console.WriteLine($"--CalibrationSchema.RefToolInBaseVector = {_calibrationSchema.RefToolInBaseVector}");
            Console.WriteLine($"--CalibrationSchema.OriInCamVector = {_calibrationSchema.OriInCamVector}");
            Console.WriteLine($"--CalibrationSchema.RefInCamVector = {_calibrationSchema.RefInCamVector}");
            Console.WriteLine($"--CalibrationSchema.OriInBaseVector = {_calibrationSchema.OriInBaseVector}");
            Console.WriteLine($"--CalibrationSchema.RefInBaseVector = {_calibrationSchema.RefInBaseVector}");
            Console.WriteLine($"--CalibrationSchema.CenterInBaseVector = {_calibrationSchema.CenterInBaseVector}");

            // calculate SourceCenterInBase, Local 11
            var sourceOriInBase = _converter.ConvertToBase(_calibrationSchema.OriInCamVector,
                      _calibrationSchema.OriToolInBaseVector);
            var sourceRefInBase = _converter.ConvertToBase(_calibrationSchema.RefInCamVector,
                _calibrationSchema.RefToolInBaseVector);

            var sourceCenterInBase = (sourceOriInBase + sourceRefInBase) / 2.0;
            Console.WriteLine($"--NewCalculate: Source_OriInBase: X={sourceOriInBase.X}, Y={sourceOriInBase.Y}");
            Console.WriteLine($"--NewCalculate: Source_OriInBase: X={sourceRefInBase.X}, Y={sourceRefInBase.Y}");
            Console.WriteLine($"--NewCalculate: Source_CenterInBase: X={sourceCenterInBase.X}, Y={sourceCenterInBase.Y}");

            if (Math.Abs(sourceOriInBase.X - _calibrationSchema.OriInBaseVector.X) > 0.0001)
            {
                Console.WriteLine($"_calibrationSchema.OriInBaseVector.X error");
                return;
            }

            if (Math.Abs(sourceOriInBase.Y - _calibrationSchema.OriInBaseVector.Y) > 0.0001)
            {
                Console.WriteLine($"_calibrationSchema.OriInBaseVector.Y error");
                return;
            }

            if (Math.Abs(sourceRefInBase.X - _calibrationSchema.RefInBaseVector.X) > 0.0001)
            {
                Console.WriteLine($"_calibrationSchema.RefInBaseVector.X error");
                return;
            }

            if (Math.Abs(sourceRefInBase.Y - _calibrationSchema.RefInBaseVector.Y) > 0.0001)
            {
                Console.WriteLine($"_calibrationSchema.RefInBaseVector.Y error");
                return;
            }

            //
            if (!_receiveFromRobotServer.Setup(2001)) //Setup with listening port
            {
                Console.WriteLine("Failed to setup!");
                Console.ReadKey();

                throw new Exception("Failed to setup!");
            }

            if (!_sendToRobotServer.Setup(2000)) //Setup with listening port
            {
                Console.WriteLine("Failed to setup!");
                Console.ReadKey();

                throw new Exception("Failed to setup!");
            }

            _receiveFromRobotServer.NewSessionConnected += _receiveFromRobotServer_NewSessionConnected;
            _receiveFromRobotServer.NewRequestReceived += _receiveFromRobotServer_NewRequestReceived;

            _sendToRobotServer.NewSessionConnected += _sendToRobotServer_NewSessionConnected;

            _oriInspector = new HalconInspectionSchemaInspector()
            {
                InspectionSchemaDir = ConfigProvider.Config.Mv_OriInspectionSchemaDirctory
            };

            _refInspector = new HalconInspectionSchemaInspector()
            {
                InspectionSchemaDir = ConfigProvider.Config.Mv_RefInspectionSchemaDirctory
            };
        }

        public void Start()
        {
            if (!_receiveFromRobotServer.Start())
            {
                Console.WriteLine("Failed to start!");
                Console.ReadKey();

                throw new Exception("Failed to start!");
            }
            Console.WriteLine("_receiveFromRobotServer started.");

            if (!_sendToRobotServer.Start())
            {
                Console.WriteLine("Failed to start!");
                Console.ReadKey();

                throw new Exception("Failed to start!");
            }
            Console.WriteLine("_sendToRobotServer started.");


            _camera.OpenFramegrabber_Name = "GigEVision";
            _camera.OpenFramegrabber_Generic = Config.OpenFramegrabber_Generic;
            _camera.ParamEntries.Add(new FrameGrabberParamEntry("AcquisitionMode", "SingleFrame"));
            _camera.ParamEntries.Add(new FrameGrabberParamEntry("ExposureMode", "Timed"));
            _camera.ParamEntries.Add(new FrameGrabberParamEntry("ExposureAuto", "Off"));
            _camera.ParamEntries.Add(new FrameGrabberParamEntry("ExposureTime", Config.ExposureTime.ToString()) {DataType = FrameGrabberParamEntryDataType.Double});

            Console.WriteLine($"_camera init(), begin, at " + DateTime.Now);

            bool cameraInitOk = false;
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"_camera init(), index:{i+1}/3, begin, at " + DateTime.Now);
                try
                {
                    var ret = _camera.Init();
                    if (ret)
                    {
                        cameraInitOk = true;
                        break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"_camera init(), index:{i+1}/3, error, at " + DateTime.Now);
                }
                finally
                {
                    Console.WriteLine($"_camera init(), index:{i+1}/3, end, at " + DateTime.Now);
                }
            }

            if (cameraInitOk)
            {
                Console.WriteLine($"_camera init(), end successful, at " + DateTime.Now);
            }
            else
            {
                Console.WriteLine($"_camera init(), error, at " + DateTime.Now);
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        public void Stop()
        {
            _receiveFromRobotServer.Stop();
            _sendToRobotServer.Stop();
        }

        public Config Config => ConfigProvider.Config;

        private void _receiveFromRobotServer_NewRequestReceived(AppSession session,
            SuperSocket.SocketBase.Protocol.StringRequestInfo requestInfo)
        {
            Console.WriteLine("AppServer_NewRequestReceived.Key: " + requestInfo.Key);
            Console.WriteLine("AppServer_NewRequestReceived.Body: " + requestInfo.Body);

            switch (requestInfo.Key)
            {
                case "Ori":
                    try
                    {
                        var image = _camera.AcquisitionOfHImage();
                        _oriResult = _oriInspector.Inspect(image);
                        image.WriteImage("tiff", 0, "B:\\Ori_Cache.tif");
                        image.Dispose();

                        Console.WriteLine($"--TargetOriInCamVector: X={_oriResult.Circles[0].Circle.CenterX}, Y={_oriResult.Circles[0].Circle.CenterY}");

                        _sendToRobotServerCurrentSession.Send("1,111,222,333");
                    }
                    catch (Exception e)
                    {
                        _sendToRobotServerCurrentSession.Send("0,111,222,333");
                        Console.WriteLine("Ori has Exception. " + e.Message);
                    }
                    Console.WriteLine("receiveFromRobotServer_NewRequestReceived.Key: " + requestInfo.Key);
                    break;
                case "Ref":
                    try
                    {
                        var image2 = _camera.AcquisitionOfHImage();
                        _refResult = _refInspector.Inspect(image2);
                        image2.WriteImage("tiff", 0, "B:\\Ref_Cache.tif");
                        image2.Dispose();

                        Console.WriteLine($"--TargetRefInCamVector: X={_refResult.Circles[0].Circle.CenterX}, Y={_refResult.Circles[0].Circle.CenterY}");

                        //
                        var targetOriInCamVector = new Vector(_oriResult.Circles[0].Circle.CenterX,
                            _oriResult.Circles[0].Circle.CenterY);
                        var targetRefInCamVector = new Vector(_refResult.Circles[0].Circle.CenterX,
                            _refResult.Circles[0].Circle.CenterY);

                        // 
                        var sourceOriInBase = _converter.ConvertToBase(_calibrationSchema.OriInCamVector,
                            _calibrationSchema.OriToolInBaseVector);
                        var sourceRefInBase = _converter.ConvertToBase(_calibrationSchema.RefInCamVector,
                            _calibrationSchema.RefToolInBaseVector);

                        var targetOriInBase = _converter.ConvertToBase(targetOriInCamVector,
                            _calibrationSchema.OriToolInBaseVector);
                        var targetRefInBase = _converter.ConvertToBase(targetRefInCamVector,
                            _calibrationSchema.RefToolInBaseVector);

                        // test for vectors center
                        var sourceCenter = (sourceOriInBase + sourceRefInBase)/2.0;
                        var targetCenter = (targetOriInBase + targetRefInBase)/2.0;
                        var centerDiffer = targetCenter - sourceCenter;

                        var sourceOriToCenter = sourceOriInBase - sourceCenter;
                        var sourceRefToCenter = sourceRefInBase - sourceCenter;

                        var targetOriToCenter = targetOriInBase - targetCenter;
                        var targetRefToCenter = targetRefInBase - targetCenter;

                        var angleOfOriToCenter = sourceOriToCenter.GetAngleTo(targetOriToCenter);
                        var angleOfRefToCenter = sourceRefToCenter.GetAngleTo(targetRefToCenter);
                        var angleForAverage = (angleOfOriToCenter + angleOfRefToCenter)/2.0;

                        Console.WriteLine($"--SourceOriInBase: X={sourceOriInBase.X:0.000}, Y={sourceOriInBase.Y:0.000}");
                        Console.WriteLine($"--SourceRefInBase: X={sourceRefInBase.X:0.000}, Y={sourceRefInBase.Y:0.000}");

                        Console.WriteLine($"--TargetOriInBase: X={targetOriInBase.X:0.000}, Y={targetOriInBase.Y:0.000}");
                        Console.WriteLine($"--TargetRefInBase: X={targetRefInBase.X:0.000}, Y={targetRefInBase.Y:0.000}");

                        Console.WriteLine($"--CenterDiffer:    X={centerDiffer.X:0.000}, Y={centerDiffer.Y:0.000}, Angle={angleForAverage:0.000}");

                        string message = $"1,{centerDiffer.X*1000:0000},{centerDiffer.Y * 1000:0000},{angleForAverage * 1000:0000}";
                        Console.WriteLine($"--Send to robot: " + message);

                        _sendToRobotServerCurrentSession.Send(message);
                    }
                    catch (Exception e)
                    {
                        _sendToRobotServerCurrentSession.Send("0,111,222,333");
                        Console.WriteLine("Ref has Exception. " + e.Message);
                    }

                    Console.WriteLine("receiveFromRobotServer_NewRequestReceived.Key: " + requestInfo.Key);

                    break;
            }
        }

        private void _receiveFromRobotServer_NewSessionConnected(AppSession session)
        {
            _receiveFromRobotServerCurrentSession?.Close();
            _receiveFromRobotServerCurrentSession = session;

            Console.WriteLine("_receiveFromRobotServer connected.");
        }

        private void _sendToRobotServer_NewSessionConnected(AppSession session)
        {
            _sendToRobotServerCurrentSession?.Close();
            _sendToRobotServerCurrentSession = session;

            Console.WriteLine("_sendToRobotServer connected.");
        }
    }
}