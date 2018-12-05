using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EasyNetQ;
using Hdc;
using Hdc.Boot;
using Hdc.Collections.Generic;
using Hdc.Measuring;
using Hdc.Mercury;
using Hdc.Mercury.Communication;
using Hdc.Mercury.Communication.OPC.Xi;
using Hdc.Mvvm.Resources;
using Hdc.Patterns;
using Hdc.Reactive.Linq;
using Hdc.Toolkit;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Vins.ML.Domain;
using Vins.ML.MeasureStationMonitor.Annotations;
using IEventBus = Hdc.Patterns.IEventBus;

namespace Vins.ML.MeasureStationMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly IUnityContainer _container = new UnityContainer();
        private IServiceLocator _serviceLocator;
        private StationResult _currentStationResult;
        private int _stationIndex;
        private BootstrapperRunner _bootstrapperRunner;

        private WorkpieceResult _currentWorkpieceResult;
        private Guid _clientGuid = Guid.NewGuid();
        private IBus _bus;
        private string _notifyMessage;

        public int StationIndex
        {
            get { return _stationIndex; }
            set
            {
                if (value == _stationIndex) return;
                _stationIndex = value;
                OnPropertyChanged();
            }
        }

        public Config Config => ConfigProvider.Config;

        public MainWindow()
        {
            DrawingBrushServiceLocator.Loader = new XamlDrawingBrushLoader();

            InitializeComponent();

            InitContainer();

            StationIndex = ConfigProvider.Config.StationIndex;

            this.Closed += MainWindow_Closed;
        }


        private void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        private void InitContainer()
        {
            _container.RegisterInstance<IServiceLocator>(new UnityServiceLocator(_container));
            _container.RegisterType<IEventBus, RxEventBus>(new ContainerControlledLifetimeManager());

            _serviceLocator = _container.Resolve<IServiceLocator>();
        }

        private void InitEasyNetQ()
        {
            var thread = new Thread((async () =>
            {
                _bus = EasyNetQEx.CreateBusAndWaitForConnnected(
                    ConfigProvider.Config.EasyNetQ_Host,
                    ConfigProvider.Config.EasyNetQ_VirtualHost,
                    ConfigProvider.Config.EasyNetQ_Username,
                    ConfigProvider.Config.EasyNetQ_Password,
                    5000,
                    isConnected => UpdateMessageBar(
                        $"{DateTime.Now}: MQ Server {isConnected}"));

                _bus.SubscribeAsync<TotalCountChangedMqEvent>(_clientGuid.ToString(),  request =>
                {
                    return Task.Run(async () =>
                    {
                        if (request.TotalCount == 0)
                            return;

                        Debug.WriteLine("TotalCountChangedMqEvent raised. at " + DateTime.Now);

                        var downloadWpcTag = request.TotalCount - ConfigProvider.Config.StationPositionOffset;

                        Debug.WriteLine("TotalCountChangedMqEvent QueryWorkpieceResultMqRequest begin. Tag=" +
                                        downloadWpcTag + ", at " + DateTime.Now);
                        await UpdateWorkpieceResult(downloadWpcTag);
                    });
                });

                _bus.SubscribeAsync<StationCompletedMqEvent>(_clientGuid.ToString(), evt =>
                {
                   return Task.Run(() =>
                    {
                        Debug.WriteLine(
                        $"MeasureCompletedMqEvent raised, StationIndex={evt.StationResult.StationIndex}, at " +
                        DateTime.Now);

                        if (evt.StationResult.StationIndex != StationIndex)
                            return;

                        Debug.WriteLine($"MeasureCompletedMqEvent raised, StationIndex equals, at " + DateTime.Now);

                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            MonitorStationMeasureResults.Add(evt.StationResult);
                            CurrentStationResult = evt.StationResult;
                        }));
                    });
                    
                });

                var response2 = await _bus.RequestAsync<QueryTotalCountRequest, QueryTotalCountResponse>(
                    new QueryTotalCountRequest());
                await UpdateWorkpieceResult(response2.TotalCount - ConfigProvider.Config.StationPositionOffset);


                System.Windows.Threading.Dispatcher.Run();
            }));

            thread.SetApartmentState(ApartmentState.MTA);
            thread.IsBackground = true;
            thread.Start();
        }

        private void UpdateMessageBar(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action<string>((x) =>
            {
                NotifyMessage = x;
            }), message);
        }

        private async Task UpdateWorkpieceResult(int downloadWpcTag)
        {
            var response =
                await _bus.RequestAsync<QueryWorkpieceResultMqRequest, QueryWorkpieceResultMqResponse>(
                    new QueryWorkpieceResultMqRequest()
                    {
                        ClientGuid = _clientGuid,
                        WorkpieceTag = downloadWpcTag
                    });

            Debug.WriteLine("TotalCountChangedMqEvent QueryWorkpieceResultMqRequest end. at " + DateTime.Now);

            Application.Current.Dispatcher.BeginInvoke(
                new Action(() => { CurrentWorkpieceResult = response.WorkpieceResult.DeepClone(); }));
        }


        public StationResult CurrentStationResult
        {
            get { return _currentStationResult; }
            set
            {
                if (Equals(value, _currentStationResult)) return;
                _currentStationResult = value;
                OnPropertyChanged();
            }
        }

        public WorkpieceResult CurrentWorkpieceResult
        {
            get { return _currentWorkpieceResult; }
            set
            {
                if (Equals(value, _currentWorkpieceResult)) return;
                _currentWorkpieceResult = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<StationResult> MonitorStationMeasureResults { get; set; } =
            new ObservableCollection<StationResult>();

        private void ChangeMonitorStationIndexButton_OnClick(object sender, RoutedEventArgs e)
        {
            StationIndex = int.Parse(ChangeMonitorStationIndexTextBox.Text);

            MonitorStationMeasureResults.Clear();

            //            var results = _repository.QueryMeasureResultsByStationIndex(_monitorStationIndex);
            //            foreach (var measureResult in results)
            //            {
            //                MonitorStationMeasureResults.Add(measureResult);
            //            }

            MessageBox.Show("工位序号设定成功");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CreateSampleMeasureResultButton_OnClick(object sender, RoutedEventArgs e)
        {
            var mr = SampleGenerator.CreateMeasureResult();
            CurrentStationResult = mr;

            var wr = SampleGenerator.CreateWorkpieceResult();
            CurrentWorkpieceResult = wr;
        }

        private void RobotStartCommandButton_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.RequestAsync<RobotStartCommandMqRequest, object>(new RobotStartCommandMqRequest(Config.StationIndex));
        }

        private void RobotStopCommandButton_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.RequestAsync<RobotStopCommandMqRequest, object>(new RobotStopCommandMqRequest(Config.StationIndex));
        }

        private void RobotResetCommandButton_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.RequestAsync<RobotResetCommandMqRequest, object>(new RobotResetCommandMqRequest(Config.StationIndex));
        }

        private void StationStartCommandButton_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.RequestAsync<StationStartCommandMqRequest, object>(new StationStartCommandMqRequest(Config.StationIndex));
        }

        private void StationResetCommandButton_OnClick(object sender, RoutedEventArgs e)
        {
            _bus.RequestAsync<StationResetCommandMqRequest, object>(new StationResetCommandMqRequest(Config.StationIndex));
        }

        private void StartMqButton_OnClick(object sender, RoutedEventArgs e)
        {
            InitEasyNetQ();

            MqStatusTextBox.Text = "OK";
            StartMqButton.IsEnabled = false;
        }

        private void SwitchToScreen0Button_OnClick(object sender, RoutedEventArgs e)
        {
            ScreenContainerTabControl.SelectedIndex = 0;
        }

        private void SwitchToScreen1Button_OnClick(object sender, RoutedEventArgs e)
        {
            ScreenContainerTabControl.SelectedIndex = 1;
        }

        private void SwitchToScreen2Button_OnClick(object sender, RoutedEventArgs e)
        {
            ScreenContainerTabControl.SelectedIndex = 2;
        }

        public string NotifyMessage
        {
            get { return _notifyMessage; }
            set
            {
                if (value == _notifyMessage) return;
                _notifyMessage = value;
                OnPropertyChanged();
            }
        }
    }
}