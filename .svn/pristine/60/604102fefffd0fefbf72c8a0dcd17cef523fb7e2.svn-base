using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using EasyNetQ;
using Hdc;
using Hdc.IO;
using Hdc.Reflection;
using Hdc.Serialization;
using Hdc.Toolkit;
using Hdc.Windows;
using Vins.ML.Domain;

namespace Vins.ML.MeasureStationLanucher
{
    public class Bootstrapper
    {
        private readonly string _assemblyDirectoryPath;
        public Config Config => ConfigProvider.Config;
        private IBus _bus;
        private Thread _mqThread;
        private AutoResetEvent InitEasyNetQEvent = new AutoResetEvent(false);
        private Dispatcher _mqDispatcher;
        private string _subscriptionId = $"{nameof(MeasureStationLanucher)}" + ConfigProvider.Config.StationName;
     
        private FileInfo _processfileInfo;

        public Bootstrapper()
        {
            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.ctor(): begin");
            _assemblyDirectoryPath = typeof (Program).Assembly.GetAssemblyDirectoryPath();
            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.ctor(): end");
        }

        public void Start()
        {
            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.Start begin, at " + DateTime.Now);

            InitEasyNetQ();
            InitEasyNetQEvent.WaitOne();


            Observable
                .Interval(TimeSpan.FromSeconds(2))
                .Subscribe(x =>
                {
                    _bus.PublishAsync(new LauncherWatchdogMqEvent()
                    {
                        StationIndex = Config.StationIndex,
                        StationName = Config.StationName,
                        DateTime = DateTime.Now,
                    }, cfg => cfg.WithExpires(5000));
                });

            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.Start end, at " + DateTime.Now);

//            while (true)
//            {
//                Console.WriteLine("press any key to start process.");
//                Console.ReadKey();
//                StartProcess();
//            }
            ;
        }

        public void Stop()
        {
            KillProcess();
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
                        $"{_subscriptionId}:: IBus.IsConnected: {isConnected}, at {DateTime.Now}"));

                _bus.WaitForConnected();

                _bus.Subscribe<StartMeasureStationServiceMqEvent>(_subscriptionId,
                    x =>
                    {
                        if (x.StationIndex != Config.StationIndex)
                        {
                            return;
                        }

                        KillProcess();
                        StartProcess(x.ConfigIndex);
                    });

                _bus.Subscribe<StopMeasureStationServiceMqEvent>(_subscriptionId,
                    x =>
                    {
                        if (x.StationIndex != Config.StationIndex)
                        {
                            return;
                        }

                        KillProcess();
                    });

                _bus.Subscribe<ShutdownComputerMqEvent>(_subscriptionId,
                    x =>
                    {
                        if (x.StationIndex != Config.StationIndex)
                        {
                            return;
                        }

                        TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper." + nameof(ShutdownComputerMqEvent));
                        TopShelfLogAndConsole.WriteLineInDebug("Computer will shutdown");

                        KillProcess();

                        Thread.Sleep(2000);

                        WindowsApi.ShutDown();
                        Environment.Exit(0);
                    });

                _bus.Subscribe<ChangeMeasureSchemaCommandMqEvent>(_subscriptionId,
                    x =>
                    {
                        if (x.StationIndex != Config.StationIndex)
                        {
                            return;
                        }

                        TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper." +
                                                               nameof(ChangeMeasureSchemaCommandMqEvent));

                        if (x.MeasureSchemaIndex >= Config.MeasureSchemaInfos.Count)
                        {
                            _bus.Publish(new ChangeMeasureSchemaErrorMqEvent()
                            {
                                StationIndex = x.StationIndex,
                                MeasureSchemaIndex = x.MeasureSchemaIndex,
                                MeasureSchemaName = x.MeasureSchemaName,
                                ErrorMessage = "MeasureSchemaIndex >= Count"
                            });
                        }
                        else
                        {
                            var msi = Config.MeasureSchemaInfos[x.MeasureSchemaIndex];

                            try
                            {
                                File.Copy(msi.SourceMeasureSchemaFileName, msi.TargetMeasureSchemaFileName, true);

                                foreach (var fileNameReference in msi.FileNameReferences)
                                {
                                    File.Copy(fileNameReference.SourceName, fileNameReference.TargetName, true);
                                }

                                _bus.Publish(new ChangeMeasureSchemaCompletedMqEvent()
                                {
                                    StationIndex = x.StationIndex,
                                    MeasureSchemaIndex = x.MeasureSchemaIndex,
                                    MeasureSchemaName = msi.Name,
                                    MeasureSchemaInfo = msi,
                                });
                            }
                            catch (Exception e)
                            {
                                _bus.Publish(new ChangeMeasureSchemaErrorMqEvent()
                                {
                                    StationIndex = x.StationIndex,
                                    MeasureSchemaIndex = x.MeasureSchemaIndex,
                                    MeasureSchemaName = x.MeasureSchemaName,
                                    MeasureSchemaInfo = msi,
                                    ErrorMessage = "Error:File.Copy()",
                                    Exception = e,
                                });
                            }
                        }
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

        private void StartProcess(int configIndex)
        {
            string processStartFileName;
            string arguments;
            bool createNoWindow;
            bool useShellExecute;

            if (Config.ProcessStartInfos == null || Config.ProcessStartInfos.Count == 0)
            {
                processStartFileName = Config.ProcessStart_FileName;
                arguments = Config.ProcessStart_Arguments;
                createNoWindow = Config.ProcessStart_CreateNoWindow;
                useShellExecute = Config.ProcessStart_UseShellExecute;
            }
            else
            {
                try
                {
                    var processStartConfig = Config.ProcessStartInfos.FirstOrDefault(x=>x.Index == configIndex);

                    processStartFileName = processStartConfig.FileName;
                    arguments = processStartConfig.Arguments;
                    createNoWindow = processStartConfig.CreateNoWindow;
                    useShellExecute = processStartConfig.UseShellExecute;
                }
                catch (Exception e)
                {
                    TopShelfLogAndConsole.WriteLineInDebug("Config.ProcessStartInfos[configIndex] error: " +
                                                    "configIndex = " + configIndex + ", at " + DateTime.Now);
                    return;
                }
            }

            if (File.Exists(processStartFileName))
            {
            }
            else if (File.Exists(_assemblyDirectoryPath + "\\" + Config.ProcessStart_FileName))
            {
                processStartFileName = _assemblyDirectoryPath + "\\" + Config.ProcessStart_FileName;
            }
            else
            {
                TopShelfLogAndConsole.WriteLineInDebug("Config.ProcessStart_FileName error: " +
                                                       Config.ProcessStart_FileName + ", at " + DateTime.Now);
                return;
            }

            if (processStartFileName != null)
                _processfileInfo = new FileInfo(processStartFileName);

            ProcessStartInfo _processStartInfo;
            _processStartInfo = new ProcessStartInfo()
            {
                FileName = processStartFileName,
                Arguments = arguments,
                CreateNoWindow = createNoWindow,
                UseShellExecute = useShellExecute,
            };

            try
            {
                var process = Process.Start(_processStartInfo);
                TopShelfLogAndConsole.WriteLineInDebug(
                    $"MeasureStationService processes started: " + process.ProcessName +
                    ", at {DateTime.Now}");
            }
            catch (Exception e)
            {
                TopShelfLogAndConsole.WriteLineInDebug(
                    $"MeasureStationService start error: {e.Message}, at {DateTime.Now}");
                return;
            }
        }

        private void KillProcess()
        {
            if (_processfileInfo != null)
            {
                var fileNameWithoutExt = _processfileInfo.GetFileNameWithoutExt();
                KillProcessInner(fileNameWithoutExt);
            }

            if (!string.IsNullOrEmpty(Config.ProcessToKill1))
                KillProcessInner(Config.ProcessToKill1);
            if (!string.IsNullOrEmpty(Config.ProcessToKill2))
                KillProcessInner(Config.ProcessToKill2);
            if (!string.IsNullOrEmpty(Config.ProcessToKill3))
                KillProcessInner(Config.ProcessToKill3);
        }

        private static void KillProcessInner(string fileNameWithoutExt)
        {
            Process[] chromes = Process.GetProcesses();
            var serviceProcess = chromes.Where(p => p.ProcessName.StartsWith(fileNameWithoutExt)).ToList();
            Console.WriteLine("MeasureStationService processes count={0}", chromes.Length);
            if (serviceProcess.Any())
            {
                foreach (var process1 in serviceProcess)
                {
                    process1.Kill();
                }

                Thread.Sleep(2000);

                TopShelfLogAndConsole.WriteLineInDebug(
                    $"MeasureStationService all killed, at {DateTime.Now}");
            }
        }
    }
}