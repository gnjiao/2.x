using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using EasyNetQ;
using Hdc;
using Hdc.IO;
using Hdc.Measuring;
using Hdc.Reactive.Linq;
using Hdc.Reflection;
using Hdc.Serialization;
using Hdc.Toolkit;
using MassTransit;
using Topshelf.Logging;
using Vins.ML.Domain;
using Vins.ML.Infrastructure;
using IBus = EasyNetQ.IBus;


namespace Vins.ML.MeasureLineDataService
{
    public class Bootstrapper
    {
        private readonly string _assemblyDirectoryPath;
        public Config Config => ConfigProvider.Config;
        private IMeasureResultRepository _repository;
        private IBus _bus;
        private Thread _initEasyNetQThread;
        private Dispatcher _initEasyNetQDispatcher;
        private readonly AutoResetEvent _initEasyNetQAutoResetEvent = new AutoResetEvent(false);
        private string _clientId = Guid.NewGuid().ToString();

        public Bootstrapper()
        {
            //            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper(): begin, at " + DateTime.Now);
            _assemblyDirectoryPath = typeof(Program).Assembly.GetAssemblyDirectoryPath();
            //            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper(): end, at " + DateTime.Now);
        }

        public void Start()
        {
            TopShelfLogAndConsole.WriteLineInDebug("Start(): begin, at " + DateTime.Now);
            //            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.MeasureDevices.dll");
            //            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.VinsML.dll");

            if (ConfigProvider.Config.RepositoryType == nameof(NDatabaseRepository))
            {
                _repository = new NDatabaseRepository(ConfigProvider.Config.DbFileName);
            }
            else if (ConfigProvider.Config.RepositoryType == nameof(SiaqodbRepository))
            {
                var siaqodbRepository = new SiaqodbRepository(ConfigProvider.Config.DbFileName);
                siaqodbRepository.MaxStationResultsCount = Config.MaxStationResultsCount;
                _repository = siaqodbRepository;
            }
            else if (string.IsNullOrEmpty(ConfigProvider.Config.RepositoryType))
            {
                throw new NotImplementedException();
            }

            //
            _initEasyNetQThread = new Thread(InitEasyNetQ);
            _initEasyNetQThread.SetApartmentState(ApartmentState.MTA);
            _initEasyNetQThread.IsBackground = true;
            _initEasyNetQThread.Start();
            _initEasyNetQAutoResetEvent.WaitOne();

            //
            TopShelfLogAndConsole.WriteLineInDebug("Start(): end, at " + DateTime.Now);
        }

        private void InitEasyNetQ()
        {
            TopShelfLogAndConsole.WriteLineInDebug("InitEasyNetQ(): begin, at " + DateTime.Now);

            _bus = EasyNetQEx.CreateBusAndWaitForConnnected(
                ConfigProvider.Config.EasyNetQ_Host,
                ConfigProvider.Config.EasyNetQ_VirtualHost,
                ConfigProvider.Config.EasyNetQ_Username,
                ConfigProvider.Config.EasyNetQ_Password,
                5000,
                isConnected => TopShelfLogAndConsole.WriteLineInDebug(
                    $"{nameof(MeasureLineDataService)}:: IBus.IsConnected: {isConnected}, at {DateTime.Now}"));

            _bus.Respond<QueryWorkpieceResultMqRequest, QueryWorkpieceResultMqResponse>(request =>
            {
                var workpieceResult = _repository.QueryWorkpieceResultByWorkpieceTag(request.WorkpieceTag);

                Console.WriteLine($"QueryWorkpieceResultMqResponse, wpcTag={request.WorkpieceTag}, at {DateTime.Now}");

                if (request.ExportXamlEnabled)
                {
                    Task.Run(() =>
                    {
                        ExportXaml(workpieceResult.DeepClone());
                    });
                }

                return new QueryWorkpieceResultMqResponse()
                {
                    ClientGuid = request.ClientGuid,
                    WorkpieceResult = workpieceResult,
                    WorkpieceTag = request.WorkpieceTag,
                };
            });

            _bus.Respond<QueryWorkpieceResultsByTakeRequest, QueryWorkpieceResultsByTakeResponse>(request =>
            {
                var workpieceResult = _repository.QueryWorkpieceResultsByTake(request.StartIndex, request.Count);

                return new QueryWorkpieceResultsByTakeResponse()
                {
                    WorkpieceResults = workpieceResult
                };
            });

            _bus.Respond<QueryWorkpieceResultsByTakeLastRequest, QueryWorkpieceResultsByTakeLastResponse>(request =>
            {
                var workpieceResult = _repository.QueryWorkpieceResultsByTakeLast(request.StartIndex, request.Count);

                return new QueryWorkpieceResultsByTakeLastResponse()
                {
                    WorkpieceResults = workpieceResult
                };
            });

            _bus.Respond<QueryAllWorkpieceResultsRequest, QueryAllWorkpieceResultsResponse>(request =>
            {
                var workpieceResults = _repository.QueryAllWorkpieceResults();

                return new QueryAllWorkpieceResultsResponse()
                {
                    WorkpieceResults = workpieceResults,
                };
            });

            _bus.RespondAsync<InitializeStationMonitorMqRequest, InitializeStationMonitorMqResponse>(async request =>
            {
                var mrs = _repository.QueryStationResultsByStationIndex(request.StationIndex);
                WorkpieceResult workpieceResult =
                    await _repository.QueryWorkpieceResultByWorkpieceTagAsync(request.WorkpieceTag);

                var response = new InitializeStationMonitorMqResponse()
                {
                    WorkpieceResult = workpieceResult,
                    StationMeasureResults = mrs,
                    ClientGuid = request.ClientGuid,
                };

                return response;
            });

            _bus.RespondAsync<QueryTotalCountOfWorkpieceResultsRequest, QueryTotalCountOfWorkpieceResultsResponse>(async request =>
            {
                var totalCountOfWorkpieceResults = await _repository.QueryTotalCountOfWorkpieceResultsAsync();

                var response = new QueryTotalCountOfWorkpieceResultsResponse()
                {
                    TotalCount = totalCountOfWorkpieceResults,
                };

                return response;
            });

            _bus.RespondAsync<QueryFirstAndLastWorkpieceResultsRequest, QueryFirstAndLastWorkpieceResultsResponse>(async request =>
            {
                var totalCountOfWorkpieceResults = await _repository.QueryTotalCountOfWorkpieceResultsAsync();
                var firstWorkpieceResult = await _repository.QueryFirstWorkpieceResultAsync();
                var lastWorkpieceResult = await _repository.QueryLastWorkpieceResultAsync();

                var response = new QueryFirstAndLastWorkpieceResultsResponse()
                {
                    TotalCount = totalCountOfWorkpieceResults,
                    FirstWorkpieceResult = firstWorkpieceResult,
                    LastWorkpieceResult = lastWorkpieceResult,
                };

                return response;
            });

            _bus.RespondAsync<QueryLastWorkpieceResultRequest, QueryLastWorkpieceResultResponse>(async request =>
            {
                var workpieceResult = await _repository.QueryLastWorkpieceResultAsync();

                var response = new QueryLastWorkpieceResultResponse()
                {
                    WorkpieceResult = workpieceResult,
                };

                return response;
            });

            _bus.RespondAsync<QueryFirstWorkpieceResultRequest, QueryFirstWorkpieceResultResponse>(async request =>
            {
                var workpieceResult = await _repository.QueryFirstWorkpieceResultAsync();

                var response = new QueryFirstWorkpieceResultResponse()
                {
                    WorkpieceResult = workpieceResult,
                };

                return response;
            });

            _bus.SubscribeAsync<StationCompletedMqEvent>(_clientId, async evt =>
            {
                await _repository.StoreMeasureResultAsync(evt.StationResult);

                var mqEvent = new MeasureResultStoredMqEvent()
                {
                    StationResult = evt.StationResult
                };

                await _bus.PublishAsync(mqEvent, x => x.WithExpires(5000));
            });

            _initEasyNetQDispatcher = Dispatcher.CurrentDispatcher;
            TopShelfLogAndConsole.WriteLineInDebug("InitEasyNetQ(): end, at " + DateTime.Now);
            _initEasyNetQAutoResetEvent.Set();
            TopShelfLogAndConsole.WriteLineInDebug("InitEasyNetQ(): Dispatcher.Run(), at " + DateTime.Now);
            Dispatcher.Run();
        }

        public void Stop()
        {
            TopShelfLogAndConsole.WriteLineInDebug("Stop(): begin, at " + DateTime.Now);
            TopShelfLogAndConsole.WriteLineInDebug("Stop(): end, at " + DateTime.Now);
        }

        readonly object _exportXamlLocker = new object();

        public void ExportXaml(WorkpieceResult result)
        {
            lock (_exportXamlLocker)
            {
                var now = DateTime.Now;

                Console.WriteLine($"ExportXaml, wpcTag={result.Tag}, at {DateTime.Now}");
                if (string.IsNullOrEmpty(Config.XamlExportDirectoryPath))
                {
                    Console.WriteLine($"Config.XamlExportPath is null or empty, ExportXaml failed, at {DateTime.Now}");
                    return;
                }

                var isExist = Directory.Exists(Config.XamlExportDirectoryPath);
                if (!isExist)
                {
                    Console.WriteLine($"{nameof(Config.XamlExportDirectoryPath)} dir is not exist: {Config.XamlExportDirectoryPath}, ExportXaml failed, at {DateTime.Now}");
                    return;
                }

                // remove old folders
                if (Config.XamlExportMaxDays > 0)
                {
                    var dirs = Directory.GetDirectories(Config.XamlExportDirectoryPath);
                    foreach (var dir in dirs)
                    {
                        var dirInfo = new DirectoryInfo(dir);
                        var dirName = dirInfo.Name;
                        var date = DateTime.Parse(dirName);
                        var span = now - date;
                        if (span.Days > Config.XamlExportMaxDays)
                        {
                            Directory.Delete(dir);
                            Console.WriteLine($"Directory Delete old dir: {dir}, at {DateTime.Now}");
                        }
                    }
                }

                // Serialize Xaml of Zip
                var today = now.ToString("yyyy-MM-dd");
                var todayDir = Path.Combine(Config.XamlExportDirectoryPath, today);
                if (!Directory.Exists(todayDir)) Directory.CreateDirectory(todayDir);

                var fileName = result.Tag.ToString("D8") + now.ToString("_yyyy-MM-dd_HH.mm.ss") + ".xaml.zip";
                var xamlFileFullName = todayDir.CombilePath(fileName);
                result.SerializeToXamlFileOfZip(xamlFileFullName);

                Console.WriteLine($"ExportXaml OK: {xamlFileFullName}, at {DateTime.Now}");
            }
        }
    }
}