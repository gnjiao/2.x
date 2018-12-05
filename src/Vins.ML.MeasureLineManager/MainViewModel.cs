using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using EasyNetQ;
using Hdc.Collections.Generic;
using Hdc.Collections.ObjectModel;
using Hdc.ComponentModel;
using Hdc.IO;
using Hdc.Measuring;
using Hdc.Mercury.Configs;
using Hdc.Reflection;
using Hdc.Serialization;
using Hdc.Sockets;
using Hdc.Toolkit;
using Microsoft.Practices.Prism.Commands;
using Omu.ValueInjecter;
using Vins.ML.Domain;
using Vins.ML.MeasureLineManager.Annotations;

namespace Vins.ML.MeasureLineManager
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public static MainViewModel Singleton = new MainViewModel();

        private IBus _bus;
        private Guid _clientGuid = Guid.NewGuid();
        private string _subscriptionId = $"{nameof(MeasureLineManager)}_" + Guid.NewGuid();
        private WorkpieceResult _queryWorkpieceResult;
        private WorkpieceResult _selectedWorkpieceResult;
        private WorkpieceResult _downloadWorkpieceResult;
        private readonly string _assemblyDirectoryPath;
        private ParameterMetadataSchema _parameterMetadataSchema;
        private TotalStationCount _totalstationcounts;
        public PlcWorkDataCountViewModel PlcWorkDataCount = new PlcWorkDataCountViewModel();


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel()
        {
            _assemblyDirectoryPath = typeof(MainViewModel).Assembly.GetAssemblyDirectoryPath();

            Init();


            QueryWorkpieceResultsCollectionView = QueryWorkpieceResults.GetDefaultCollectionView();


            //
            AlarmParameterEntriesCollectionView = _alarmViewModels.GetDefaultCollectionView();
            AlarmParameterEntriesCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("GroupDescription"));            
        }

        private ICollectionView _alarmParameterEntriesCollectionView;

        public ICollectionView AlarmParameterEntriesCollectionView
        {
            get { return _alarmParameterEntriesCollectionView; }
            set
            {
                if (Equals(_alarmParameterEntriesCollectionView, value)) return;
                _alarmParameterEntriesCollectionView = value;
                OnPropertyChanged();
            }
        }

        private int _lastWorkpieceTag;
        private int _screenIndex;

        private BindableCollection<ParameterEntryViewModel> _alarmViewModels =
            new BindableCollection<ParameterEntryViewModel>();

        private void InitializeCounter()
        {
            var excelFileName = _assemblyDirectoryPath + "\\Counters.xaml";
            _totalstationcounts = InitializeCounters(excelFileName);
        }
        public TotalStationCount InitializeCounters(string path)
        {
            if (!path.IsFileExist())
            {
                Initializexaml(path);
            }

            TotalStationCount stationcounts = path.DeserializeFromXamlFile<TotalStationCount>();
            return stationcounts;
        }
        public void Initializexaml(string path)
        {
            TotalStationCount totalstationcounts = new TotalStationCount();

            for (int i = 0; i < 10; i++)
            {
                var stationcount = new StationCount();
                stationcount.stationindex = i + 1;
                stationcount.okcount = 0;
                stationcount.ngcount = 0;
                stationcount.totalcount = 0;
                totalstationcounts.StationCounts.Add(stationcount);
            }
            totalstationcounts.SerializeToXamlFile(path);
        }

        private void InitEasyNetQ()
        {
            var thread = new Thread((() =>
            {
                _bus = EasyNetQEx.CreateBusAndWaitForConnnected(
                    ConfigProvider.Config.EasyNetQ_Host,
                    ConfigProvider.Config.EasyNetQ_VirtualHost,
                    ConfigProvider.Config.EasyNetQ_Username,
                    ConfigProvider.Config.EasyNetQ_Password,
                    5000,
                    isConnected => Debug.WriteLine(
                        $"{nameof(MeasureLineManager)}:: IBus.IsConnected: {isConnected}, at {DateTime.Now}"));


                _bus.SubscribeAsync<MeasureResultStoredMqEvent>(_subscriptionId, async request =>
                {
                    return;

                    if (request.StationResult.WorkpieceTag < _lastWorkpieceTag) return;

                    Debug.WriteLine("MeasureResultStoredMqEvent raised. Tag=" +
                                    request.StationResult.WorkpieceTag + ", at " + DateTime.Now);

                    _lastWorkpieceTag = request.StationResult.WorkpieceTag;
                    var response = await _bus.RequestAsync<QueryWorkpieceResultsByTakeRequest,
                        QueryWorkpieceResultsByTakeResponse>(new QueryWorkpieceResultsByTakeRequest()
                        {
                            StartIndex = _lastWorkpieceTag,
                            Count = 20,
                        });

                    Debug.WriteLine("MeasureResultStoredMqEvent end. Tag=" +
                                    request.StationResult.WorkpieceTag + ", at " + DateTime.Now);

                    await Application.Current.Dispatcher.BeginInvoke(
                        new Action(() =>
                        {
                            LastestWorkpieceResults.Clear();
                            LastestWorkpieceResults.AddRange(response.WorkpieceResults);
                        }));
                });

                _bus.SubscribeAsync<TotalCountChangedMqEvent>(_subscriptionId, async request =>
                {
                    var response = 
                        await _bus.RequestAsync<QueryPlcWorkDataCountRequest, QueryPlcWorkDataCountResponse>(
                            new QueryPlcWorkDataCountRequest());

                    await Application.Current.Dispatcher.BeginInvoke(
                        new Action(() =>
                        {
                            if (Config.UseComputerToCount)
                            {
                                PlcWorkDataCount.JobCount = _totalstationcounts.totalcount;
                                PlcWorkDataCount.TotalCount = _totalstationcounts.totalcount;
                                PlcWorkDataCount.IsNgCount = _totalstationcounts.totalngcount;
                                PlcWorkDataCount.IsOkCount = _totalstationcounts.totalokcount;
                                PlcWorkDataCount.OkPrecent = Math.Round((double)_totalstationcounts.totalokcount / (double)(_totalstationcounts.totalcount) * 100, 2);
                            }
                            else
                            {
                                PlcWorkDataCount.JobCount = response.JobCount;
                                PlcWorkDataCount.TotalCount = response.TotalCount;
                                PlcWorkDataCount.IsNgCount = response.IsNgCount;
                                PlcWorkDataCount.IsOkCount = response.IsOkCount;
                                PlcWorkDataCount.OkPrecent = Math.Round((double)response.IsOkCount / (double)(response.IsOkCount + response.IsNgCount) * 100, 2);
                            }
                        }));

                });

                _bus.SubscribeAsync<WorkpieceInPositionEvent>(_subscriptionId, async request =>
                {
                    if (request.StationIndex != Config.DownloadStationPositionOffset)
                        return;

                    Debug.WriteLine("TotalCountChangedMqEvent raised. at " + DateTime.Now);

                    var downloadWpcTag = request.WorkpieceTag;

                    Debug.WriteLine("TotalCountChangedMqEvent QueryWorkpieceResultMqRequest begin. Tag=" +
                                    downloadWpcTag + ", at " + DateTime.Now);
                    var response =
                        await _bus.RequestAsync<QueryWorkpieceResultMqRequest, QueryWorkpieceResultMqResponse>(
                            new QueryWorkpieceResultMqRequest()
                            {
                                ClientGuid = _clientGuid,
                                WorkpieceTag = downloadWpcTag
                            });

                    Debug.WriteLine("TotalCountChangedMqEvent QueryWorkpieceResultMqRequest end. at " + DateTime.Now);

                    await Application.Current.Dispatcher.BeginInvoke(
                        new Action(() => { DownloadWorkpieceResult = response.WorkpieceResult; }));

                });

                _bus.SubscribeAsync<StationCompletedMqEvent>(_subscriptionId, async request =>
                {
                    await Application.Current.Dispatcher.BeginInvoke(new Action<StationCompletedMqEvent>(x =>
                    {
                        if (Config.UseComputerToCount)
                        {

                            for (int i = 0; i < CurrentStationResults.Count; i++)
                            {
                                var cur = CurrentStationResults[i];

                                if (cur.StationIndex != request.StationResult.StationIndex) continue;

                                if (x.StationResult.IsNG2)
                                {
                                    _totalstationcounts.totalcount++;
                                    _totalstationcounts.StationCounts[i].totalcount++;
                                    PlcWorkDataCount.JobCount = _totalstationcounts.totalcount;
                                    PlcWorkDataCount.TotalCount = _totalstationcounts.totalcount;
                                    PlcWorkDataCount.IsNgCount =
                                _totalstationcounts.totalcount - _totalstationcounts.totalokcount;
                                    PlcWorkDataCount.OkPrecent =
                               Math.Round(
                                   (double)_totalstationcounts.totalokcount /
                                   (double)(_totalstationcounts.totalcount) * 100, 2);

                                }
                                else
                                {
                                    _totalstationcounts.totalokcount++;
                                    _totalstationcounts.totalcount++;
                                    _totalstationcounts.StationCounts[i].okcount++;
                                    _totalstationcounts.StationCounts[i].totalcount++;
                                    PlcWorkDataCount.JobCount = _totalstationcounts.totalcount;
                                    PlcWorkDataCount.TotalCount = _totalstationcounts.totalcount;
                                    PlcWorkDataCount.IsOkCount = _totalstationcounts.totalokcount;
                                    PlcWorkDataCount.OkPrecent =
                               Math.Round(
                                   (double)_totalstationcounts.totalokcount /
                                   (double)(_totalstationcounts.totalcount) * 100, 2);
                                }

                                double countofgo = Convert.ToDouble(_totalstationcounts.StationCounts[i].okcount);
                                double countoftotal = Convert.ToDouble(_totalstationcounts.StationCounts[i].totalcount);

                                x.StationResult.StationDescription = string.Format("{0:P}", countofgo / countoftotal);
                                x.StationResult.MeasureTag = _totalstationcounts.StationCounts[i].totalcount;
                                CurrentStationResults[i] = x.StationResult;
                                break;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < CurrentStationResults.Count; i++)
                            {
                                var cur = CurrentStationResults[i];
                                if (cur.StationIndex != request.StationResult.StationIndex) continue;
                                CurrentStationResults[i] = x.StationResult;
                                break;
                            }
                        }
                    }), request);                    
                });

                _bus.Subscribe<ParameterValueChangedMqEvent>(_subscriptionId, request =>
                {
                    var entry = _alarmViewModels.SingleOrDefault(x => x.Name == request.ParameterMetadata.Name);
                    if (entry != null)
                        entry.Value = request.Value;
                });

                _bus.Subscribe<StationWatchdogMqEvent>(_subscriptionId, request =>
                {
                    var entry = StationStatuses.SingleOrDefault(x => x.StationIndex == request.StationIndex);
                    if (entry != null)
                    {
                        entry.StationWatchdogDateTime = DateTime.Now;
                        entry.CurrentConfigName = request.ConfigName;
                    }
                });

                _bus.Subscribe<LauncherWatchdogMqEvent>(_subscriptionId, request =>
                {
                    var entry = StationStatuses.SingleOrDefault(x => x.StationIndex == request.StationIndex);
                    if (entry != null)
                        entry.LauncherWatchdogDateTime = DateTime.Now;
                });

                _bus.Subscribe<ChangeMeasureSchemaCompletedMqEvent>(_subscriptionId, evt =>
                {
                    var entry = StationStatuses.SingleOrDefault(x => x.StationIndex == evt.StationIndex);
                    if (entry != null)
                        entry.CurrentMeasureSchemaName = evt.MeasureSchemaName;
                });

                _bus.Subscribe<ChangeMeasureSchemaErrorMqEvent>(_subscriptionId, evt =>
                {
                    var entry = StationStatuses.SingleOrDefault(x => x.StationIndex == evt.StationIndex);
                    if (entry != null)
                        entry.CurrentMeasureSchemaName = evt.ErrorMessage;
                });

                


                /* TODO
               *
               *  var response2 = await _bus.RequestAsync<ReadParameterValuesMqRequest, ReadParameterValuesMqResponse>(
                    new ReadParameterValuesMqRequest());

                foreach (var mqEvent in response2.ChangedMqEvents)
                {
                    var entry = _alarmViewModels.SingleOrDefault(x => x.Name == mqEvent.ParameterMetadata.Name);
                    if (entry != null)
                        entry.Value = mqEvent.Value;
                }
*/


                System.Windows.Threading.Dispatcher.Run();
            }));

            thread.SetApartmentState(ApartmentState.MTA);
            thread.IsBackground = true;
            thread.Start();
        }

        public Config Config => ConfigProvider.Config;

        public ObservableCollection<WorkpieceResult> LastestWorkpieceResults { get; set; } =
            new ObservableCollection<WorkpieceResult>();

        private void Init()
        {
            InitializeCounter();
            if (Config.MonitorScreen_UniformGridRows == 0)
                Config.MonitorScreen_UniformGridRows = 2;

            if (Config.MonitorScreen_UniformGridColumns == 0)
                Config.MonitorScreen_UniformGridColumns = 5;

            for (int i = 0; i < (Config.StationsCount + 1); i++)
            {
                if (i > 0)
                {
                    var mr = new StationResult
                    {
                        StationIndex = i,
                        StationName = "测量工位" + i
                    };
                    CurrentStationResults.Add(mr);
                }

                var ss = new StationStatusViewModel()
                {
                    StationIndex = i,
                    StationName = "测量工位" + i
                };
                StationStatuses.Add(ss);
            }

            var paraSchemaPath = _assemblyDirectoryPath + "\\ParameterMetadataSchema.xaml";
            if (File.Exists(paraSchemaPath))
            {
                _parameterMetadataSchema = paraSchemaPath.DeserializeFromXamlFile<ParameterMetadataSchema>();

                var pevms = _parameterMetadataSchema.ParameterMetadatas.Select(x =>
                {
                    var parameterEntryViewModel = new ParameterEntryViewModel();
                    parameterEntryViewModel.InjectFrom(x);
                    return parameterEntryViewModel;
                });

                _alarmViewModels.AddRange(pevms);
            }


            RobotResetCommand = new DelegateCommand<string>(
                index =>
                    _bus.RequestAsync<RobotResetCommandMqRequest, object>(
                        new RobotResetCommandMqRequest(int.Parse(index))));

            RobotStartCommand = new DelegateCommand<string>(
                index =>
                    _bus.RequestAsync<RobotStartCommandMqRequest, object>(
                        new RobotStartCommandMqRequest(int.Parse(index))));

            RobotStopCommand = new DelegateCommand<string>(
                index =>
                    _bus.RequestAsync<RobotStopCommandMqRequest, object>(new RobotStopCommandMqRequest(int.Parse(index))));

            StationStartCommand = new DelegateCommand<string>(
                index =>
                    _bus.RequestAsync<StationStartCommandMqRequest, object>(
                        new StationStartCommandMqRequest(int.Parse(index))));

            StationResetCommand = new DelegateCommand<string>(
                index =>
                    _bus.RequestAsync<StationResetCommandMqRequest, object>(
                        new StationResetCommandMqRequest(int.Parse(index))));

            SetWorkpieceLocatingReadyCommand = new DelegateCommand<string>(
                index =>
                    _bus.RequestAsync<SetWorkpieceLocatingReadyPlcEventMqRequest, object>(
                        new SetWorkpieceLocatingReadyPlcEventMqRequest(int.Parse(index))));

            ResetWorkpieceLocatingReadyCommand = new DelegateCommand<string>(
                index =>
                    _bus.RequestAsync<ResetWorkpieceLocatingReadyPlcEventMqRequest, object>(
                        new ResetWorkpieceLocatingReadyPlcEventMqRequest(int.Parse(index))));

            SetAllMeasureDataProcessedCommand = new DelegateCommand<string>(
                index =>
                    _bus.RequestAsync<SetAllMeasureDataProcessedAppEventMqRequest, object>(
                        new SetAllMeasureDataProcessedAppEventMqRequest(int.Parse(index))));

            StartMeasureStationServiceCommand = new DelegateCommand<StationStatusViewModel>(
                x =>
                    _bus.PublishAsync(
                        new StartMeasureStationServiceMqEvent()
                        {
                            StationIndex = x.StationIndex,
                            ConfigIndex = 0,
                        }, cfg => cfg.WithExpires(5000)));

            StopMeasureStationServiceCommand = new DelegateCommand<StationStatusViewModel>(
                x =>
                    _bus.PublishAsync(
                        new StopMeasureStationServiceMqEvent()
                        {
                            StationIndex = x.StationIndex,
                        }, cfg => cfg.WithExpires(5000)));

            ShutdownComputerCommand = new DelegateCommand<StationStatusViewModel>(
                x =>
                    _bus.PublishAsync(
                        new ShutdownComputerMqEvent()
                        {
                            StationIndex = x.StationIndex,
                        }, cfg => cfg.WithExpires(5000)));

            ChangeMeasureSchemaCommand1 = new DelegateCommand<StationStatusViewModel>(
                x =>
                    _bus.PublishAsync(
                        new StartMeasureStationServiceMqEvent()
                        {
                            StationIndex = x.StationIndex,
                            ConfigIndex = 1,
                        }, cfg => cfg.WithExpires(5000)));

            ChangeMeasureSchemaCommand2 = new DelegateCommand<StationStatusViewModel>(
                x =>
                    _bus.PublishAsync(
                        new StartMeasureStationServiceMqEvent()
                        {
                            StationIndex = x.StationIndex,
                            ConfigIndex = 2,
                        }, cfg => cfg.WithExpires(5000)));

            ChangeMeasureSchemaCommand3 = new DelegateCommand<StationStatusViewModel>(
                x =>
                    _bus.PublishAsync(
                        new StartMeasureStationServiceMqEvent()
                        {
                            StationIndex = x.StationIndex,
                            ConfigIndex = 3,
                        }, cfg => cfg.WithExpires(5000)));

            ChangeMeasureSchemaCommand4 = new DelegateCommand<StationStatusViewModel>(
                x =>
                    _bus.PublishAsync(
                        new StartMeasureStationServiceMqEvent()
                        {
                            StationIndex = x.StationIndex,
                            ConfigIndex = 4,
                        }, cfg => cfg.WithExpires(5000)));

            ChangeMeasureSchemaCommand5 = new DelegateCommand<StationStatusViewModel>(
                x =>
                    _bus.PublishAsync(
                        new StartMeasureStationServiceMqEvent()
                        {
                            StationIndex = x.StationIndex,
                            ConfigIndex = 5,
                        }, cfg => cfg.WithExpires(5000)));

            ChangeMeasureSchemaCommand6 = new DelegateCommand<StationStatusViewModel>(
                x =>
                    _bus.PublishAsync(
                        new StartMeasureStationServiceMqEvent()
                        {
                            StationIndex = x.StationIndex,
                            ConfigIndex = 6,
                        }, cfg => cfg.WithExpires(5000)));

            ChangeMeasureSchemaCommand7 = new DelegateCommand<StationStatusViewModel>(
                x =>
                    _bus.PublishAsync(
                        new StartMeasureStationServiceMqEvent()
                        {
                            StationIndex = x.StationIndex,
                            ConfigIndex = 7,
                        }, cfg => cfg.WithExpires(5000)));

            ChangeMeasureSchemaCommand8 = new DelegateCommand<StationStatusViewModel>(
                x =>
                    _bus.PublishAsync(
                        new StartMeasureStationServiceMqEvent()
                        {
                            StationIndex = x.StationIndex,
                            ConfigIndex = 8,
                        }, cfg => cfg.WithExpires(5000)));

            ChangeMeasureSchemaCommand9 = new DelegateCommand<StationStatusViewModel>(
                x =>
                    _bus.PublishAsync(
                        new StartMeasureStationServiceMqEvent()
                        {
                            StationIndex = x.StationIndex,
                            ConfigIndex = 9,
                        }, cfg => cfg.WithExpires(5000)));

            ChangeMeasureSchemaCommand10 = new DelegateCommand<StationStatusViewModel>(
                x =>
                    _bus.PublishAsync(
                        new StartMeasureStationServiceMqEvent()
                        {
                            StationIndex = x.StationIndex,
                            ConfigIndex = 10,
                        }, cfg => cfg.WithExpires(5000)));

            Observable
             .Interval(TimeSpan.FromSeconds(1))
             .ObserveOnDispatcher()
             .Subscribe(x =>
             {
                 foreach (var stationStatusViewModel in StationStatuses)
                 {
                     // station timeout
                     var timeSpan = DateTime.Now - stationStatusViewModel.StationWatchdogDateTime;
                     if (timeSpan.TotalSeconds > 5)
                     {
                         stationStatusViewModel.StationWatchdogTimeoutDescription = "Offline";
                         stationStatusViewModel.CurrentConfigName = "";
                     }
                     else
                     {
                         stationStatusViewModel.StationWatchdogTimeoutDescription = "OK";
                     }

                     // station timeout
                     var launcherTimeSpan = DateTime.Now - stationStatusViewModel.LauncherWatchdogDateTime;
                     if (launcherTimeSpan.TotalSeconds > 5)
                     {
                         stationStatusViewModel.LauncherWatchdogTimeoutDescription = "Offline";
                     }
                     else
                     {
                         stationStatusViewModel.LauncherWatchdogTimeoutDescription = "OK";
                     }
                 }
             });
        }

        public ICollectionView QueryWorkpieceResultsCollectionView { get; set; }

        public ObservableCollection<StationResult> AllMeasureResults { get; set; } =
            new ObservableCollection<StationResult>();

        public ObservableCollection<StationResult> CurrentStationResults { get; set; } =
            new ObservableCollection<StationResult>();

        public ObservableCollection<StationStatusViewModel> StationStatuses { get; set; } =
            new ObservableCollection<StationStatusViewModel>();

        public ObservableCollection<WorkpieceResult> QueryWorkpieceResults { get; set; } =
            new ObservableCollection<WorkpieceResult>();

        public WorkpieceResult QueryWorkpieceResult
        {
            get { return _queryWorkpieceResult; }
            set
            {
                if (Equals(value, _queryWorkpieceResult)) return;
                _queryWorkpieceResult = value;
                OnPropertyChanged();
            }
        }

        public WorkpieceResult SelectedWorkpieceResult
        {
            get { return _selectedWorkpieceResult; }
            set
            {
                if (Equals(value, _selectedWorkpieceResult)) return;
                _selectedWorkpieceResult = value;
                OnPropertyChanged();
            }
        }

        public WorkpieceResult DownloadWorkpieceResult
        {
            get { return _downloadWorkpieceResult; }
            set
            {
                if (Equals(value, _downloadWorkpieceResult)) return;
                _downloadWorkpieceResult = value;
                OnPropertyChanged();
            }
        }

        public int ScreenIndex
        {
            get { return _screenIndex; }
            set
            {
                if (Equals(value, _screenIndex)) return;
                _screenIndex = value;
                OnPropertyChanged();
            }
        }

        public async void QueryInitWorkCountResults()
        {
            try
            {
                var response =
                        await _bus.RequestAsync<QueryPlcWorkDataCountRequest, QueryPlcWorkDataCountResponse>(
                            new QueryPlcWorkDataCountRequest());

                await Application.Current.Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        PlcWorkDataCount.JobCount = response.JobCount;
                        PlcWorkDataCount.TotalCount = response.TotalCount;
                        PlcWorkDataCount.IsNgCount = response.IsNgCount;
                        PlcWorkDataCount.IsOkCount = response.IsOkCount;
                        PlcWorkDataCount.OkPrecent = Math.Round((double)response.IsOkCount / (double)(response.IsOkCount + response.IsNgCount) * 100, 2);
                    }));

            }
            catch (Exception e)
            {
                MessageBox.Show($"{nameof(QueryAllWorkpieceResults)} timeout, " + e.Message);
            }
        }

        public async void QueryAllWorkpieceResults()
        {
            try
            {
                await Application.Current.Dispatcher.BeginInvoke(
                    new Action(() => QueryWorkpieceResults.Clear()));

                var totalCountResponse = _bus.Request<QueryTotalCountOfWorkpieceResultsRequest,
                    QueryTotalCountOfWorkpieceResultsResponse>(new QueryTotalCountOfWorkpieceResultsRequest());
                var totalCount = totalCountResponse.TotalCount;

                if (totalCount <= Config.Query_MaxPageSize)
                {
                    var response = await _bus.RequestAsync<QueryAllWorkpieceResultsRequest, QueryAllWorkpieceResultsResponse>(
                    new QueryAllWorkpieceResultsRequest());

                    await Application.Current.Dispatcher.BeginInvoke(
                        new Action(() => QueryWorkpieceResults.AddRange(response.WorkpieceResults)));
                }
                else
                {
                    MessageBox.Show("totalCount > Config.Query_MaxPageSize");
                    return;

                    var pageCount = totalCount / Config.Query_MaxPageSize + 1;
                    for (int i = 0; i < pageCount; i++)
                    {

                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"{nameof(QueryAllWorkpieceResults)} timeout, " + e.Message);
            }
        }

        public void CreateSampleMeasureResult()
        {
            QueryWorkpieceResults.Clear();

            for (int i = 0; i < 5; i++)
            {
                var workpieceResult = SampleGenerator.CreateWorkpieceResult();
                workpieceResult.Tag = i + 1;
                QueryWorkpieceResults.Add(workpieceResult);
            }

            DownloadWorkpieceResult = SampleGenerator.CreateWorkpieceResult();
        }

        public void StartMqButton()
        {
            InitEasyNetQ();            
        }

        public void SwitchScreenTo(int index)
        {
            ScreenIndex = index;
        }

        public async void QueryWorkpieceResultByTag(int tag)
        {
            try
            {
                await Application.Current.Dispatcher.BeginInvoke(
                    new Action(() => QueryWorkpieceResults.Clear()));

                var result = await _bus.RequestAsync<QueryWorkpieceResultMqRequest,
                    QueryWorkpieceResultMqResponse>(
                new QueryWorkpieceResultMqRequest()
                {
                    ClientGuid = _clientGuid,
                    WorkpieceTag = tag,
                });

                await Application.Current.Dispatcher.BeginInvoke(
                    new Action(() => QueryWorkpieceResults.Add(result.WorkpieceResult)));
            }
            catch (Exception e)
            {
                MessageBox.Show("QueryWorkpieceResultByTag timeout");
            }
        }

        public async void GetWorkpieceResultsByTake(int tag, int count)
        {
            try
            {
                await Application.Current.Dispatcher.BeginInvoke(
                    new Action(() => QueryWorkpieceResults.Clear()));

                var response =
                    await _bus.RequestAsync<QueryWorkpieceResultsByTakeRequest, QueryWorkpieceResultsByTakeResponse>(
                        new QueryWorkpieceResultsByTakeRequest()
                        {
                            StartIndex = tag,
                            Count = count,
                        });

                await Application.Current.Dispatcher.BeginInvoke(
                    new Action(() => QueryWorkpieceResults.AddRange(response.WorkpieceResults)));
            }
            catch (Exception e)
            {
                MessageBox.Show($"{nameof(GetWorkpieceResultsByTake)} timeout");
            }
        }

        public async void GetWorkpieceResultsByTakeLast(int count)
        {
            try
            {
                await Application.Current.Dispatcher.BeginInvoke(
                    new Action(() => QueryWorkpieceResults.Clear()));

                var lastResponse = await _bus.RequestAsync<QueryLastWorkpieceResultRequest,
                    QueryLastWorkpieceResultResponse>(new QueryLastWorkpieceResultRequest());

                var startIndex = lastResponse.WorkpieceResult.Tag;

                var response = await _bus.RequestAsync<QueryWorkpieceResultsByTakeLastRequest,
                    QueryWorkpieceResultsByTakeLastResponse>(new QueryWorkpieceResultsByTakeLastRequest()
                    {
                        StartIndex = startIndex,
                        Count = count,
                    });

                await Application.Current.Dispatcher.BeginInvoke(
                    new Action(() => QueryWorkpieceResults.AddRange(response.WorkpieceResults)));
            }
            catch (Exception e)
            {
                MessageBox.Show($"{nameof(GetWorkpieceResultsByTake)} timeout");
            }
        }

        public async void GetWorkpieceResultsByBetween(int beginTag, int endTag)
        {
            try
            {
                await Application.Current.Dispatcher.BeginInvoke(
                    new Action(() => QueryWorkpieceResults.Clear()));

                var response =
                    await _bus.RequestAsync<QueryWorkpieceResultsByBetweenRequest,
                        QueryWorkpieceResultsByBetweenResponse>
                        (
                            new QueryWorkpieceResultsByBetweenRequest()
                            {
                                BeginWpcTag = beginTag,
                                EndWpcTag = endTag,
                            });

                await Application.Current.Dispatcher.BeginInvoke(
                    new Action(() => QueryWorkpieceResults.AddRange(response.WorkpieceResults)));
            }
            catch (Exception e)
            {
                MessageBox.Show($"{nameof(GetWorkpieceResultsByBetween)} timeout");
            }
        }

        public bool ExportWorkpieceResults(string dir)
        {
            var wpcs = QueryWorkpieceResults.ToList();

            if (!Directory.Exists(dir))
            {
                MessageBox.Show("Directory is not exist");
                return false;
            }

            var now = DateTime.Now;
            var reportDir = dir + $"\\" + now.ToString("_yyyy-MM-dd_HH.mm.ss");

            Directory.CreateDirectory(reportDir);

            for (int i = 0; i < wpcs.Count; i++)
            {
                var wpc = wpcs[i];
                wpc.SerializeToXamlFile(reportDir + "\\" + wpc.Tag + "_" + i.ToString("D4") + ".xaml");
            }

            return true;
        }

        public bool ExportWorkpieceResultsToCsv(string dir)
        {
            var wpcs = QueryWorkpieceResults.ToList();

            if (!Directory.Exists(dir))
            {
                MessageBox.Show("Directory is not exist");
                return false;
            }

            //wpcs.Add(@"C:\Users\Administrator\Desktop\2017.11.6\2017.11.6\100\760128_0000.xaml".DeserializeFromXamlFile<WorkpieceResult>());

            for (int i = 0; i < wpcs.Count; i++)
            {
                var wpc = wpcs[i];

                M1787Report.ExportM1787Csv(wpc, dir);
            }

            return true;
        }

        public async void ResetJobCount()
        {
            await _bus.RequestAsync<ResetJobCountRequest, ResetJobCountResponse>
                (
                    new ResetJobCountRequest()
                );

            await Application.Current.Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        PlcWorkDataCount.JobCount = 0;
                        PlcWorkDataCount.IsNgCount = 0;
                        PlcWorkDataCount.IsOkCount = 0;
                        PlcWorkDataCount.OkPrecent = 0;
                    }));

           
        }

        public async void ResetTotalCount()
        {
            await _bus.RequestAsync<ResetTotalCountRequest, ResetTotalCountResponse>
                (
                    new ResetTotalCountRequest()
                );

            await Application.Current.Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        PlcWorkDataCount.JobCount = 0;
                        PlcWorkDataCount.IsNgCount = 0;
                        PlcWorkDataCount.IsOkCount = 0;
                        PlcWorkDataCount.OkPrecent = 0;
                        PlcWorkDataCount.TotalCount = 0;
                    }));
            
        }

        public async void SaveTotalCount()
        {
            var excelFileName = _assemblyDirectoryPath + "\\Counters.xaml";

            await Application.Current.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    _totalstationcounts.SerializeToXamlFile(excelFileName);
                }));
        }

        public DelegateCommand<string> RobotResetCommand { get; set; }
        public DelegateCommand<string> RobotStartCommand { get; set; }
        public DelegateCommand<string> RobotStopCommand { get; set; }
        public DelegateCommand<string> StationStartCommand { get; set; }
        public DelegateCommand<string> StationResetCommand { get; set; }
        public DelegateCommand<string> SetWorkpieceLocatingReadyCommand { get; set; }
        public DelegateCommand<string> ResetWorkpieceLocatingReadyCommand { get; set; }
        public DelegateCommand<string> SetAllMeasureDataProcessedCommand { get; set; }

        public DelegateCommand<StationStatusViewModel> StartMeasureStationServiceCommand { get; set; }
        public DelegateCommand<StationStatusViewModel> StopMeasureStationServiceCommand { get; set; }
        public DelegateCommand<StationStatusViewModel> ShutdownComputerCommand { get; set; }
        public DelegateCommand<StationStatusViewModel> RestartComputerCommand { get; set; }
        public DelegateCommand<StationStatusViewModel> ChangeMeasureSchemaCommand1 { get; set; }
        public DelegateCommand<StationStatusViewModel> ChangeMeasureSchemaCommand2 { get; set; }
        public DelegateCommand<StationStatusViewModel> ChangeMeasureSchemaCommand3 { get; set; }
        public DelegateCommand<StationStatusViewModel> ChangeMeasureSchemaCommand4 { get; set; }
        public DelegateCommand<StationStatusViewModel> ChangeMeasureSchemaCommand5 { get; set; }
        public DelegateCommand<StationStatusViewModel> ChangeMeasureSchemaCommand6 { get; set; }
        public DelegateCommand<StationStatusViewModel> ChangeMeasureSchemaCommand7 { get; set; }
        public DelegateCommand<StationStatusViewModel> ChangeMeasureSchemaCommand8 { get; set; }
        public DelegateCommand<StationStatusViewModel> ChangeMeasureSchemaCommand9 { get; set; }
        public DelegateCommand<StationStatusViewModel> ChangeMeasureSchemaCommand10 { get; set; }


        public string ConfigName0 => Config.ConfigName0;
        public string ConfigName1 => Config.ConfigName1;
        public string ConfigName2 => Config.ConfigName2;
        public string ConfigName3 => Config.ConfigName3;
        public string ConfigName4 => Config.ConfigName4;
        public string ConfigName5 => Config.ConfigName5;
        public string ConfigName6 => Config.ConfigName6;
        public string ConfigName7 => Config.ConfigName7;
        public string ConfigName8 => Config.ConfigName8;
        public string ConfigName9 => Config.ConfigName9;
        public string ConfigName10 => Config.ConfigName10;

        public async void SendIndexesOfPlateOfDownloadRobot(int indexOfOkPlateOfDownloadRobot, int indexOfNgPlateOfDownloadRobot)
        {
            var client = new AsyncTcpClient();

            try
            {
                await client.ConnectAsync(Config.DownloadRobot_Host, Config.DownloadRobot_Port);
            }
            catch (Exception)
            {
                MessageBox.Show($"{nameof(SendIndexesOfPlateOfDownloadRobot)}: ConnectAsync() Error.\n" +
                                $"TCP通讯连接失败, 请检查IP和端口.\n" +
                                $"机器人IP:{Config.DownloadRobot_Host}, 机器人端口:{Config.DownloadRobot_Port}");
                client.Dispose();
                return;
            }

            var msg = $"1,{indexOfOkPlateOfDownloadRobot},{indexOfNgPlateOfDownloadRobot}";
            byte[] bytes = Encoding.ASCII.GetBytes(msg);

            try
            {
                await client.SendAsync(bytes);
            }
            catch (Exception)
            {
                MessageBox.Show($"{nameof(SendIndexesOfPlateOfDownloadRobot)}: SendAsync() Error.\n" +
                                $"TCP数据发送失败,请检查IP和端口号.\n" +
                                $"机器人IP:{Config.DownloadRobot_Host}, 机器人端口:{Config.DownloadRobot_Port}");
                client.Dispose();
                return;
            }

            Thread.Sleep(500);

            await client.CloseAsync();
            client.Dispose();

            MessageBox.Show($"向下料机器人发送成功.\n" +
                            $"机器人IP:{Config.DownloadRobot_Host}, 机器人端口:{Config.DownloadRobot_Port}\n" +
                            $"OK盘序号:{indexOfOkPlateOfDownloadRobot}, NG盘序号:{indexOfNgPlateOfDownloadRobot}\n" +
                            $"发送数据:1,{indexOfOkPlateOfDownloadRobot},{indexOfNgPlateOfDownloadRobot}");

            Console.WriteLine($"{nameof(SendIndexesOfPlateOfDownloadRobot)}:{msg}");
        }
    }

    public class StationCount
    {
        public int stationindex { get; set; }
        public int okcount { get; set; }
        public int ngcount { get; set; }
        public int totalcount { get; set; }

    }
    public class TotalStationCount
    {
        public List<StationCount> StationCounts { get; set; } = new List<StationCount>();
        public int totalokcount { get; set; }
        public int totalngcount { get; set; }
        public int totalcount { get; set; }

    }

}