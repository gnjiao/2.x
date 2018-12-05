﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using EasyNetQ;
using Hdc;
using Hdc.IO;
using Hdc.Measuring;
using Hdc.Measuring.VinsML;
using Hdc.Mercury;
using Hdc.Mercury.Configs;
using Hdc.Reactive;
using Hdc.Reactive.Linq;
using Hdc.Reflection;
using Hdc.Serialization;
using Hdc.Toolkit;
using Topshelf.Logging;
using Vins.ML.Domain;

namespace Vins.ML.OpcService
{
    public class Bootstrapper
    {
        private IBus _bus;
        private readonly string _assemblyDirectoryPath;
        private readonly string _reportsDir;
        public Config Config => ConfigProvider.Config;
        private DateTime _bootDateTime = DateTime.Now;
        private readonly OpcInitializer _opcInitializer = new OpcInitializer();
//        private readonly Guid _clientGuid = Guid.NewGuid();
        public IDeviceGroupProvider DeviceGroupProvider => OpcInitializer.DeviceGroupProvider;
        public IDeviceGroup RootDeviceGroup => DeviceGroupProvider.RootDeviceGroup;
        private IDevice<int> _totalCountDevice;  
        private IDevice<int> _jobCountDevice;
        private IDevice<int> _isOkCountDevice;
        private IDevice<int> _isNgCountDevice;
        private IDevice<int> _isNgDevice;
        private Thread _mqThread;
        private Thread _opcThread;

        private Dispatcher _mqDispatcher;
        private Dispatcher _opcDispatcher;
        private AutoResetEvent InitEasyNetQEvent = new AutoResetEvent(false);
        private AutoResetEvent InitOpcEvent = new AutoResetEvent(false);
        private AutoResetEvent StartOpcEvent = new AutoResetEvent(false);
        private AutoResetEvent OpcStartedEvent = new AutoResetEvent(false);
        private ParameterMetadataSchema _parameterMetadataSchema;

        public Bootstrapper()
        {
            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.ctor(): begin");

            _assemblyDirectoryPath = typeof (Program).Assembly.GetAssemblyDirectoryPath();
            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.ctor(): end");
        }

        public void Start()
        {
            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.Start(): Service starting...");

            InitEasyNetQ();
            InitEasyNetQEvent.WaitOne();

            InitOpc();
            InitOpcEvent.WaitOne();

            InitParameterMetadataSchema();

            StartOpcEvent.Set();

            var ret = OpcStartedEvent.WaitOne(10000);
            if (!ret)
            {
                TopShelfLogAndConsole.WriteLineInDebug(
                    "Bootstrapper.Start(): Service start failed: Start OPC timeout !!!");
                throw new InvalidOperationException("Bootstrapper.Start(): Service start failed: Start OPC timeout !!!");
            }

            RootDeviceGroup.GetDevice<bool>("UpdateWatchdogCommand").WriteTrue();
            Observable.Interval(TimeSpan.FromSeconds(10)).ObserveOnTaskPool()
                .Subscribe(x => { RootDeviceGroup.GetDevice<bool>("UpdateWatchdogCommand").WriteTrue(); });
            
            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.Start(): Service start successful.");
        }

        private readonly List<Tuple<ParameterMetadata, IDevice<bool>>> _boolAlarmDevices =
            new List<Tuple<ParameterMetadata, IDevice<bool>>>();

        private readonly List<Tuple<ParameterMetadata, IDevice<int>>> _intAlarmDevices =
            new List<Tuple<ParameterMetadata, IDevice<int>>>();

        private void InitParameterMetadataSchema()
        {
            var paraSchemaPath = _assemblyDirectoryPath + "\\ParameterMetadataSchema.xaml";
            if (!File.Exists(paraSchemaPath)) return;

            _parameterMetadataSchema = paraSchemaPath.DeserializeFromXamlFile<ParameterMetadataSchema>();

            foreach (var entry in _parameterMetadataSchema.ParameterMetadatas)
            {
                if (entry.IsBoolean)
                {
                    IDevice<bool> boolDevice = DeviceGroupProvider.RootDeviceGroup.GetDevice<bool>(entry.Name);

                    if (boolDevice == null)
                    {
                        Console.WriteLine($"InitParameterMetadataSchema(): !!! cannot find device: {entry.Name}");
                        continue;
                    }

                    _boolAlarmDevices.Add(new Tuple<ParameterMetadata, IDevice<bool>>(entry, boolDevice));

                    boolDevice?.ObserveOnTaskPool().Subscribe(
                        x =>
                        {
                            var message = new ParameterValueChangedMqEvent()
                            {
                                ParameterMetadata = entry,
                                Value = x ? 1 : 0,
                            };
                            _bus.PublishAsync(message, p => p.WithExpires(5000));

                            Console.WriteLine($"ParameterValueChangedMqEvent Published: " +
                                              $"Device: {message.ParameterMetadata.Name}, " +
                                              $"DataType: Boolean, " +
                                              $"Value: {message.Value}, " +
                                              $"at " +
                                              DateTime.Now);
                        });
                }
                else
                {
                    var int32Device = DeviceGroupProvider.RootDeviceGroup.GetDevice<int>(entry.Name);

                    if (int32Device == null)
                    {
                        Console.WriteLine($"InitParameterMetadataSchema(): !!! cannot find device: {entry.Name}");
                        continue;
                    }

                    _intAlarmDevices.Add(new Tuple<ParameterMetadata, IDevice<int>>(entry, int32Device));
                    int32Device?.ObserveOnTaskPool().Subscribe(
                        x =>
                        {
                            var message = new ParameterValueChangedMqEvent()
                            {
                                ParameterMetadata = entry,
                                Value = x,
                            };
                            _bus.PublishAsync(message, p => p.WithExpires(5000));

                            Console.WriteLine($"ParameterValueChangedMqEvent Published: " +
                                              $"Device: {message.ParameterMetadata.Name}, " +
                                              $"DataType: Int32, " +
                                              $"Value: {message.Value}, " +
                                              $"at " +
                                              DateTime.Now);
                        });
                }
            }
        }

        private void InitEasyNetQ()
        {
            _mqThread = new Thread(() =>
            {
                TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitEasyNetQ(): begin");

                _bus = EasyNetQEx.CreateBusAndWaitForConnnected(
                    ConfigProvider.Config.EasyNetQ_Host,
                    ConfigProvider.Config.EasyNetQ_VirtualHost,
                    ConfigProvider.Config.EasyNetQ_Username,
                    ConfigProvider.Config.EasyNetQ_Password,
                    5000,
                    isConnected => TopShelfLogAndConsole.WriteLineInDebug(
                        $"OpcService:: IBus.IsConnected: {isConnected}, at {DateTime.Now}"));

                _bus.WaitForConnected();

                _bus.RespondAsync<QueryTotalCountRequest, QueryTotalCountResponse>(
                    req =>
                    {
                        return Task.Run(() => new QueryTotalCountResponse()
                        {
                            TotalCount = (int)_totalCountDevice.Value,
                            JobCount = (int)_jobCountDevice.Value,
                        });
                    });

                
                _bus.RespondAsync<ResetTotalCountRequest, ResetTotalCountResponse>(
                    req =>
                    {
                        return Task.Run(() =>
                        {

                            _isNgCountDevice.Write(0);
                            _isOkCountDevice.Write(0);
                            _jobCountDevice.Write(0);
                            _totalCountDevice.Write(0);
                            return new ResetTotalCountResponse();
                        });
                    });

                _bus.RespondAsync<ResetJobCountRequest, ResetJobCountResponse>(
                    req =>
                    {
                        return Task.Run(() =>
                        {
                            _isNgCountDevice.Write(0);
                            _isOkCountDevice.Write(0);
                            _jobCountDevice.Write(0);
                            return new ResetJobCountResponse();
                        });
                    });

                _bus.RespondAsync<ReadParameterValuesMqRequest, ReadParameterValuesMqResponse>(
                    req =>
                    {
                        return Task.Run(() =>
                        {
                            var readParameterValuesMqResponse = new ReadParameterValuesMqResponse();

                            if (_parameterMetadataSchema == null)
                                return readParameterValuesMqResponse;

                            foreach (var entry in _boolAlarmDevices)
                            {
                                readParameterValuesMqResponse.ChangedMqEvents.Add(new ParameterValueChangedMqEvent()
                                {
                                    ParameterMetadata = entry.Item1,
                                    Value = entry.Item2.Value ? 1 : 0,
                                });
                            }

                            foreach (var entry in _intAlarmDevices)
                            {
                                readParameterValuesMqResponse.ChangedMqEvents.Add(new ParameterValueChangedMqEvent()
                                {
                                    ParameterMetadata = entry.Item1,
                                    Value = entry.Item2.Value,
                                });
                            }
                            Console.WriteLine($"ReadParameterValuesMqResponse sent. at " + DateTime.Now);

                            return readParameterValuesMqResponse;
                        });
                    });


//                _bus.RespondAsync<RobotResetCommandMqRequest, object>(
//                    req =>
//                    {
//                        return Task.Run(async () =>
//                        {
//                            RootDeviceGroup.GetDevice<bool>("RobotStopCommand0" + req.Index).WriteTrue();
//                            await Task.Delay(1500);
//                            RootDeviceGroup.GetDevice<bool>("RobotStartCommand0" + req.Index).WriteTrue();
//                            return new object();
//                        });
//                    });
//                _bus.RespondAsync<RobotStartCommandMqRequest, object>(
//                    req =>
//                    {
//                        return Task.Run(() =>
//                        {
//                            RootDeviceGroup.GetDevice<bool>("RobotStartCommand0" + req.Index).WriteTrue();
//                            return new object();
//                        });
//                    });
//                _bus.RespondAsync<RobotStopCommandMqRequest, object>(
//                    req =>
//                    {
//                        return Task.Run(() =>
//                        {
//                            RootDeviceGroup.GetDevice<bool>("RobotStopCommand0" + req.Index).WriteTrue();
//                            return new object();
//                        });
//                    });
//                _bus.RespondAsync<StationStartCommandMqRequest, object>(
//                    req =>
//                    {
//                        return Task.Run(() =>
//                        {
//                            RootDeviceGroup.GetDevice<bool>("StationStartCommand0" + req.Index).WriteTrue();
//                            return new object();
//                        });
//                    });
//                _bus.RespondAsync<StationResetCommandMqRequest, object>(
//                    req =>
//                    {
//                        return Task.Run(() =>
//                        {
//                            RootDeviceGroup.GetDevice<bool>("StationResetCommand0" + req.Index).WriteTrue();
//                            return new object();
//                        });
//                    });
//                _bus.RespondAsync<ResetWorkpieceLocatingReadyPlcEventMqRequest, object>(
//                    req =>
//                    {
//                        return Task.Run(() =>
//                        {
//                            RootDeviceGroup.GetDevice<bool>("WorkpieceLocatingReadyPlcEvent0" + req.Index).WriteFalse();
//                            return new object();
//                        });
//                    });
//                _bus.RespondAsync<SetWorkpieceLocatingReadyPlcEventMqRequest, object>(
//                    req =>
//                    {
//                        return Task.Run(() =>
//                        {
//                            RootDeviceGroup.GetDevice<bool>("WorkpieceLocatingReadyPlcEvent0" + req.Index).WriteTrue();
//                            return new object();
//                        });
//                    });
//                _bus.RespondAsync<SetAllMeasureDataProcessedAppEventMqRequest, object>(
//                    req =>
//                    {
//                        return Task.Run(() =>
//                        {
//                            RootDeviceGroup.GetDevice<bool>("AllMeasureDataProcessedAppEvent0" + req.Index).WriteTrue();
//                            return new object();
//                        });
//                    });

                _bus.Subscribe<GeneralMqCommand>("OpcService",
                    x => 
//                    Task.Run(()=> 
                    { RootDeviceGroup.GetDevice<bool>(x.CommandName).WriteTrue(); }
//                    )
                    );

                _bus.Subscribe<GeneralMqEvent>("OpcService", x =>
//                    Task.Run(() =>
                    {
                        if (!x.EventName.EndsWith("Acknowledge")) return;
                        var deviceName = x.EventName.Replace("Acknowledge", "");
                        var device = RootDeviceGroup.GetDevice<bool>(deviceName);
                        device.WriteFalse();
                    }
//)
                );

                _bus.Subscribe<StationCompletedMqEvent>("OpcService", x =>
                    {
                        _isNgDevice.Write(x.StationResult.IsNG2 ? 2 : 1);
                    }
                );

                _bus.RespondAsync<QueryPlcWorkDataCountRequest, QueryPlcWorkDataCountResponse>(
                    req =>
                    {
                        return Task.Run(() => new QueryPlcWorkDataCountResponse
                        {
                            TotalCount = _totalCountDevice.Value,
                            JobCount = _jobCountDevice.Value,
                            IsNgCount = _isNgCountDevice.Value,
                            IsOkCount = _isOkCountDevice.Value,
                        });
                    });

                _mqDispatcher = Dispatcher.CurrentDispatcher;
                InitEasyNetQEvent.Set();
                TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitEasyNetQ(): end.");
                TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitEasyNetQ(): Dispatcher.Run()");

                Dispatcher.Run();
            });

            _mqThread.SetApartmentState(ApartmentState.MTA);
            _mqThread.IsBackground = true;
            _mqThread.Start();
            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitEasyNetQ(): _mqThread.Start()");
        }

        public void Stop()
        {
        }

        private void InitOpc()
        {
            _opcThread = new Thread((() =>
            {
                TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitOpc(): begin");
                TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitOpc(): ServerUrl: " +
                                                       ConfigProvider.Config.OpcXi_ServerUrl);
                TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitOpc(): UserName: " +
                                                       ConfigProvider.Config.OpcXi_UserName);
                TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitOpc(): Password: " +
                                                       ConfigProvider.Config.OpcXi_Password);

                _opcInitializer.OpcXi_ServerUrl = ConfigProvider.Config.OpcXi_ServerUrl;
                _opcInitializer.OpcXi_UserName = ConfigProvider.Config.OpcXi_UserName;
                _opcInitializer.OpcXi_Password = ConfigProvider.Config.OpcXi_Password;
                _opcInitializer.ManualStart = true;

                TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitOpc(): _opcInitializer.Initialize(): begin");
                _opcInitializer.Initialize();
                TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitOpc(): _opcInitializer.Initialize(): end");

                _totalCountDevice = DeviceGroupProvider.RootDeviceGroup.GetDevice<int>("TotalCount");
                _jobCountDevice = DeviceGroupProvider.RootDeviceGroup.GetDevice<int>("JobCount");
                _isOkCountDevice = DeviceGroupProvider.RootDeviceGroup.GetDevice<int>("IsOkCount");
                _isNgCountDevice = DeviceGroupProvider.RootDeviceGroup.GetDevice<int>("IsNgCount");
                _isNgDevice = DeviceGroupProvider.RootDeviceGroup.GetDevice<int>("IsNg");

                _totalCountDevice.Subscribe(async x =>
                {
//                    if (x == 0)
//                        return;

                    TopShelfLogAndConsole.WriteLineInDebug("TotalCountDevice changed to: " + x);

                    await _bus.PublishAsync(new TotalCountChangedMqEvent()
                    {
                        TotalCount = x,
                    }, p => p.WithExpires(5000));

                    TopShelfLogAndConsole.WriteLineInDebug("TotalCountDevice changed to: " + x + ", PublishAsync end");
                });


                for (int i = 0; i < 20; i++)
                {
                    var stationIndex = i;
                    var deviceName = "WorkpieceInPositionPlcEvent" + stationIndex.ToString("D2");
                    var device = DeviceGroupProvider.RootDeviceGroup.GetDevice<uint>(deviceName);

                    device.Subscribe(x =>
                    {
                        if (x == 0) return;

                        var uInt32 = Convert.ToUInt32("01000000000000000000000000000000", 2);
                        var isExistInt = x & uInt32;
                        if (isExistInt != uInt32)
                            return;

                        var uInt32Enabled = Convert.ToUInt32("00100000000000000000000000000000", 2);
                        var enabledInt = x & uInt32Enabled;
                        if (enabledInt != uInt32Enabled)
                            return;

                        var uInt32IsNg = Convert.ToUInt32("00010000000000000000000000000000", 2);
                        var isNGInt = x & uInt32IsNg;
                        if (isNGInt == uInt32IsNg)
                            return;

                        var tag = x & Convert.ToUInt32("00001111111111111111111111111111", 2);
                        var workpieceTag = Convert.ToInt32(tag);

                        _bus.PublishAsync(new WorkpieceInPositionEvent()
                        {
                            StationIndex = stationIndex,
                            DeviceName = deviceName,
                            WorkpieceTag = workpieceTag,
                            DateTime = DateTime.Now,
                        }, p => p.WithExpires(500));

                        $"{deviceName} changed, WpcTag={workpieceTag}, Metadata={x.ToString("X")}, at {DateTime.Now}"
                        .WriteLineInConsoleAndDebug();
                    });
                }

                _opcDispatcher = Dispatcher.CurrentDispatcher;
                InitOpcEvent.Set();
                TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitOpc(): InitOpcEvent.Set() end");

                TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitOpc(): StartOpcEvent.WaitOne() begin...");
                StartOpcEvent.WaitOne();
                TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitOpc(): StartOpcEvent.WaitOne() end.");

                TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitOpc(): _opcInitializer.Start() begin...");
                _opcInitializer.Start();
                TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitOpc(): _opcInitializer.Start() end.");

                OpcStartedEvent.Set();
                TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitOpc(): OpcStartedEvent.Set() end");

                TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitOpc(): Dispatcher.Run()...");
                Dispatcher.Run();
            }));

            _opcThread.SetApartmentState(ApartmentState.STA);
            _opcThread.IsBackground = true;
            _opcThread.Start();
            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.InitOpc(): _opcThread.Start()");
        }
    }
}