using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using HalconDotNet;
using Hdc.Collections.Generic;
using Hdc.Mv.Halcon;
using Hdc.Mv.ImageAcquisition.Halcon;
using Hdc.Mv.Inspection;
using Hdc.Mv.Mvvm;
using Hdc.Reactive.Linq;
using Hdc.Serialization;
using Microsoft.Win32;

namespace Hdc.Mv.RobotVision.HandEyeCalibrator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isCameraInitialized;
        private HalconInspectionSchemaInspector _calibInspector;
        private HalconInspectionSchemaInspector _oriInspector;
        private HalconInspectionSchemaInspector _refInspector;
        private readonly IGrabService _grabService = new GrabService();

        public ObservableCollection<CircleIndicatorViewModel> DefinitionsCircleIndicators { get; set; } =
            new ObservableCollection<CircleIndicatorViewModel>();

        public ObservableCollection<CircleIndicatorViewModel> ResultsCircleIndicators { get; set; } =
            new ObservableCollection<CircleIndicatorViewModel>();

        public ObservableCollection<RobotPoint> RobotPoints { get; set; } = new ObservableCollection<RobotPoint>();

        private readonly IRobotService _robotService = new RcRobotService();
        private bool _robotServiceInitialized;
//        private int _autoCalibIndex;
        private bool _autoCalibRunning;
        private HImage _image;
        private readonly object UpdateImageLocker = new object();


        public Config Config => ConfigProvider.Config;

        public MainWindow()
        {
            InitializeComponent();

            AutoCalibButton.IsEnabled = true;
            GrabButton.IsEnabled = false;
            SnapButton.IsEnabled = false;
            GetRobotHereButton.IsEnabled = true;
            GetOriToolInBaseButton.IsEnabled = true;
            GetRefToolInBaseButton.IsEnabled = false;
            GoOriToolInBaseButton.IsEnabled = true;
            GoRefToolInBaseButton.IsEnabled = false;
            StartContinuosGrabButton.IsEnabled = false;
            StopContinuosGrabButton.IsEnabled = false;
            InspectButton.IsEnabled = false;

            Closing += MainWindow_Closing;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _grabService.Uninitialize();
        }

        private async void GrabButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isCameraInitialized)
                InitCamera();

//            var image = await Task.Run(() => _grabService.Grab());
            var image = _grabService.Grab();
            UpdateImage(image);
        }

        private void InitCameraButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isCameraInitialized)
            {
                MessageBox.Show("It has Initialized");
                return;
            }

            InitCamera();
        }

        private void InitCamera()
        {
            if (_isCameraInitialized)
                return;

            InitCameraButton.IsEnabled = false;

            _grabService.InitializedEvent
                .ObserveOnDispatcher()
                .Subscribe(
                    x =>
                    {
                        _isCameraInitialized = true;
                        GrabButton.IsEnabled = true;
                        SnapButton.IsEnabled = true;
                        StartContinuosGrabButton.IsEnabled = true;
                        StopContinuosGrabButton.IsEnabled = true;
                        InspectButton.IsEnabled = true;

                        if (Config.OpenFramegrabber_Name == "GigEVision")
                        {
                            ExposureTimeTextBlock.Text = _grabService.GetParamerter("ExposureTime")?.ToString();
                        }
                    });
            _grabService.Initialize();


            _calibInspector = new HalconInspectionSchemaInspector()
            {
                InspectionSchemaDir = Config.Mv_CalibInspectionSchemaDirctory,
            };
            _calibInspector.UpdateInspectionSchemaFromDir();

            _oriInspector = new HalconInspectionSchemaInspector()
            {
                InspectionSchemaDir = Config.Mv_OriInspectionSchemaDirctory,
            };
            _oriInspector.UpdateInspectionSchemaFromDir();

            _refInspector = new HalconInspectionSchemaInspector()
            {
                InspectionSchemaDir = Config.Mv_RefInspectionSchemaDirctory,
            };
            _refInspector.UpdateInspectionSchemaFromDir();
        }

        public void Show_CircleSearchingDefinitions(IEnumerable<CircleSearchingDefinition> circleSearchingDefinitions)
        {
            var olds = DefinitionsCircleIndicators.ToList();

            foreach (var circleSearchingDefinition in circleSearchingDefinitions)
            {
                var innerCircle = new CircleIndicatorViewModel()
                {
                    CenterX = circleSearchingDefinition.CenterX,
                    CenterY = circleSearchingDefinition.CenterY,
                    Radius = circleSearchingDefinition.InnerRadius,
                };
                olds.Add(innerCircle);
                var outerCircle = new CircleIndicatorViewModel()
                {
                    CenterX = circleSearchingDefinition.CenterX,
                    CenterY = circleSearchingDefinition.CenterY,
                    Radius = circleSearchingDefinition.OuterRadius,
                };
                olds.Add(outerCircle);
            }

            DefinitionsCircleIndicators.Clear();
            DefinitionsCircleIndicators.AddRange(olds);
        }

        private void EnableGrabContinueButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void DisableGrabContinueButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void LoadSchemaButton_Click(object sender, RoutedEventArgs e)
        {
            _calibInspector = new HalconInspectionSchemaInspector()
            {
                InspectionSchemaDir = Config.Mv_CalibInspectionSchemaDirctory,
            };
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
//            return;
//            var image2 = _halconFrameGrabber.HFramegrabber.GrabImage();
//
//            return;
//
//            HalconViewer.ClearWindow();
//            HalconViewer.Image?.Dispose();
//
//            var image = _halconFrameGrabber.HFramegrabber.GrabImage();
//            HalconViewer.Image = image;
//
//            if (_calibInspector == null) return;
//            var result = _calibInspector.Inspect(image);
//
//            DefinitionsCircleIndicators.Clear();
//            ResultsCircleIndicators.Clear();
//
//            Show_CircleSearchingDefinitions(result.GetCircleSearchingDefinitions());
//            Show_CircleSearchingResults(result.Circles);
        }

        public void Show_CircleSearchingResults(IList<CircleSearchingResult> CircleSearchingResults)
        {
            var css = ResultsCircleIndicators.ToList();

            foreach (var result in CircleSearchingResults)
            {
                var ci = new CircleIndicatorViewModel()
                {
                    CenterX = result.Circle.CenterX,
                    CenterY = result.Circle.CenterY,
                    Radius = result.Circle.Radius,
                };
                if (result.IsNotFound)
                {
                    //                    ci.Stroke = Brushes.Red;}
                }

                css.Add(ci);
            }

            ResultsCircleIndicators.Clear();
            ResultsCircleIndicators.AddRange(css);
        }

        private void ChangeExposureTimeButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var time = Int32.Parse(ExposureTimeTextBox.Text);
                _grabService.SetParamerter("ExposureTime", time);
                var newValue = _grabService.GetParamerter("ExposureTime");
                ExposureTimeTextBlock.Text = newValue.ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("ExposureTime invalid");
            }
        }

        private void AutoCalibButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_robotServiceInitialized)
            {
                MessageBox.Show("robotService is not Initialized");
                return;
            }
            _autoCalibRunning = true;
            AutoCalibButton.IsEnabled = false;
            RobotPoints.Clear();
            _robotService.StartAutoCalib();
        }

        private void InitRobotServiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_robotServiceInitialized)
                return;

            InitRobotService();

            InitRobotServiceButton.IsEnabled = false;
        }

        public void InitRobotService()
        {
            if (_robotServiceInitialized)
                return;

            _robotService.RobotPointUpdatedEvent.Subscribe(
                x =>
                {
                    if (!_autoCalibRunning)
                    {
                        Debug.WriteLine("Error, RobotPointUpdatedEvent when _autoCalibRunning = false. at " +
                                        DateTime.Now);
                        return;
                    }

                    if (x.Index == -1)
                    {
                        _autoCalibRunning = false;

                        Application.Current.Dispatcher.BeginInvoke(
                            new Action(() => { AutoCalibButton.IsEnabled = true; }));

                        return;
                    }

                    AutoResetEvent resetEvent = new AutoResetEvent(false);

                    Application.Current.Dispatcher.BeginInvoke(
                        new Action(() =>
                        {
                            if (_isCameraInitialized)
                            {
                                var image = _grabService.Grab();
                                var result = Inspect(image);

                                if (result != null)
                                {
                                    x.CamX = result.Circles[0].Circle.CenterX;
                                    x.CamY = result.Circles[0].Circle.CenterY;
                                }
                            }

                            RobotPoints.Add(x);

                            resetEvent.Set();
                        }));

                    resetEvent.WaitOne(3000);
                });

            _robotService.SessionChangedEvent
                .ObserveOnDispatcher()
                .Subscribe(
                    x =>
                    {
                        AutoCalibButton.IsEnabled = x;
                        GetRobotHereButton.IsEnabled = x;
                        GetOriToolInBaseButton.IsEnabled = x;
                        GetRefToolInBaseButton.IsEnabled = x;
                        GoOriToolInBaseButton.IsEnabled = x;
                        GoRefToolInBaseButton.IsEnabled = x;
                    });

            _robotService.Init();
            _robotServiceInitialized = true;
        }

        private async void GrabAndInspectButton_Click(object sender, RoutedEventArgs e)
        {
            var image = await Task.Run(() => _grabService.Grab());
//            var image = await Task.Run(() => _grabService.GrabAsync());
//            var image = _grabService.Grab();
//            var image = _grabService.GrabAsync();

            UpdateImage(image);

            var result = await Task.Run(() => _calibInspector.Inspect(image));
            UpdateResult(result);

//            Application.Current.Dispatcher.BeginInvoke(
//                new Action(() => { UpdateResult(result); }));
        }

        public void UpdateResult(InspectionResult result)
        {
//            HalconViewer.ZoomFit();
//            HalconViewer.ClearWindow();
//            HalconViewer.Image?.Dispose();
//            HalconViewer.Image = _image;

            DefinitionsCircleIndicators.Clear();
            ResultsCircleIndicators.Clear();

            Show_CircleSearchingDefinitions(result.GetCircleSearchingDefinitions());
            Show_CircleSearchingResults(result.Circles);

            XTextBox.Text = result.Circles[0].Circle.CenterX.ToString("0000.000");
            YTextBox.Text = result.Circles[0].Circle.CenterY.ToString("0000.000");
        }

        public InspectionResult Inspect(HImage hImage)
        {
            Debug.WriteLine("MainWindow.Inspect() begin, at " + DateTime.Now);

            if (_calibInspector == null)
                return null;

            Debug.WriteLine("MainWindow.Inspect(): _calibInspector.Inspect begin, at " + DateTime.Now);
            InspectionResult result = _calibInspector.Inspect(hImage);
            Debug.WriteLine("MainWindow.Inspect(): _calibInspector.Inspect end, at " + DateTime.Now);

            Application.Current.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    DefinitionsCircleIndicators.Clear();
                    ResultsCircleIndicators.Clear();

//                    HalconViewer.ClearWindow();
//                    HalconViewer.Image?.Dispose();
//                    HalconViewer.Image = hImage;

                    Show_CircleSearchingDefinitions(result.GetCircleSearchingDefinitions());
                    Show_CircleSearchingResults(result.Circles);

                    XTextBox.Text = result.Circles[0].Circle.CenterX.ToString("0000.000");
                    YTextBox.Text = result.Circles[0].Circle.CenterY.ToString("0000.000");
                }));

            Debug.WriteLine("MainWindow.Inspect() end, at " + DateTime.Now);
            return result;
        }

        private void ZoomFitButton_Click(object sender, RoutedEventArgs e)
        {
            HalconViewer.ZoomFit();
        }

        private void SaveImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog {Filter = "*.tif|*.tif"};
            var ret = dialog.ShowDialog();
            if (ret != true) return;

            var fileName = dialog.FileName;
            var image = _grabService.Grab();
            image.WriteImageOfTiffLzw(fileName);
        }

        private bool isStarted;

        private async void StartContinuosGrabButton_Click(object sender, RoutedEventArgs e)
        {
            isStarted = true;

            while (isStarted)
            {
                var image = _grabService.Grab();
                UpdateImage(image);

                var result2 = await Task.Run(() => Inspect(image));
                UpdateResult(result2);

                await Task.Delay(2000);
            }
        }

        private void StopContinuosGrabButton_Click(object sender, RoutedEventArgs e)
        {
            isStarted = false;
        }

        private async void GetRobotHereButton_Click(object sender, RoutedEventArgs e)
        {
            GetRobotHereButton.IsEnabled = false;

            RobotHereXTextBox.Text = "waiting...";
            RobotHereYTextBox.Text = "waiting...";
            RobotHereZTextBox.Text = "waiting...";
            RobotHereUTextBox.Text = "waiting...";

            var herePoint = await Task.Run(() => _robotService.GetHere());

            RobotHereXTextBox.Text = herePoint.BaseX.ToString("000.000");
            RobotHereYTextBox.Text = herePoint.BaseY.ToString("000.000");
            RobotHereZTextBox.Text = herePoint.BaseZ.ToString("000.000");
            RobotHereUTextBox.Text = herePoint.BaseU.ToString("000.000");

            GetRobotHereButton.IsEnabled = true;
        }

        private async void GetOriToolInBaseButton_Click(object sender, RoutedEventArgs e)
        {
            GetOriToolInBaseButton.IsEnabled = false;

            OriToolInBaseXTextBox.Text = "waiting...";
            OriToolInBaseYTextBox.Text = "waiting...";
            OriToolInBaseZTextBox.Text = "waiting...";
            OriToolInBaseUTextBox.Text = "waiting...";

            var point = await Task.Run(() => _robotService.GetOriToolInBase());

            OriToolInBaseVector = new Vector(point.BaseX, point.BaseY);

            OriToolInBaseXTextBox.Text = point.BaseX.ToString("000.000");
            OriToolInBaseYTextBox.Text = point.BaseY.ToString("000.000");
            OriToolInBaseZTextBox.Text = point.BaseZ.ToString("000.000");
            OriToolInBaseUTextBox.Text = point.BaseU.ToString("000.000");

            GetOriToolInBaseButton.IsEnabled = true;
        }

        private async void GetRefToolInBaseButton_Click(object sender, RoutedEventArgs e)
        {
            GetRefToolInBaseButton.IsEnabled = false;

            RefToolInBaseXTextBox.Text = "waiting...";
            RefToolInBaseYTextBox.Text = "waiting...";
            RefToolInBaseZTextBox.Text = "waiting...";
            RefToolInBaseUTextBox.Text = "waiting...";

            var point = await Task.Run(() => _robotService.GetRefToolInBase());

            RefToolInBaseVector = new Vector(point.BaseX, point.BaseY);

            RefToolInBaseXTextBox.Text = point.BaseX.ToString("000.000");
            RefToolInBaseYTextBox.Text = point.BaseY.ToString("000.000");
            RefToolInBaseZTextBox.Text = point.BaseZ.ToString("000.000");
            RefToolInBaseUTextBox.Text = point.BaseU.ToString("000.000");

            GetRefToolInBaseButton.IsEnabled = true;
        }

        private async void GoOriToolInBaseButton_Click(object sender, RoutedEventArgs e)
        {
            GoOriToolInBaseButton.IsEnabled = false;
            await Task.Run(() => _robotService.GoOriToolInBase());
            GoOriToolInBaseButton.IsEnabled = true;
        }

        private async void GoRefToolInBaseButton_Click(object sender, RoutedEventArgs e)
        {
            GoRefToolInBaseButton.IsEnabled = false;
            await Task.Run(() => _robotService.GoRefToolInBase());
            GoRefToolInBaseButton.IsEnabled = true;
        }

        private async void InspectCurrentInCamButton_Click(object sender, RoutedEventArgs e)
        {
            var image = await Task.Run(() => _grabService.Grab());
            var result = await Task.Run(() => _calibInspector.Inspect(image));

            DefinitionsCircleIndicators.Clear();
            ResultsCircleIndicators.Clear();

            HalconViewer.ClearWindow();
            HalconViewer.Image?.Dispose();
            HalconViewer.Image = image;

            Show_CircleSearchingDefinitions(result.GetCircleSearchingDefinitions());
            Show_CircleSearchingResults(result.Circles);

            XTextBox.Text = result.Circles[0].Circle.CenterX.ToString("0000.000");
            YTextBox.Text = result.Circles[0].Circle.CenterY.ToString("0000.000");
        }

        private async void InspectOriInCamButton_Click(object sender, RoutedEventArgs e)
        {
            var image = await Task.Run(() => _grabService.Grab());
            var result = await Task.Run(() => _oriInspector.Inspect(image));

            DefinitionsCircleIndicators.Clear();
            ResultsCircleIndicators.Clear();

            HalconViewer.ClearWindow();
            HalconViewer.Image?.Dispose();
            HalconViewer.Image = image;

            Show_CircleSearchingDefinitions(result.GetCircleSearchingDefinitions());
            Show_CircleSearchingResults(result.Circles);

            var centerX = result.Circles[0].Circle.CenterX;
            var centerY = result.Circles[0].Circle.CenterY;
            OriInCamVector = new Vector(centerX, centerY);
            OriInCamXTextBox.Text = centerX.ToString("0000.000");
            OriInCamYTextBox.Text = centerY.ToString("0000.000");
        }

        private Vector OriInCamVector;
        private Vector RefInCamVector;
        private Vector OriInBaseVector;
        private Vector RefInBaseVector;
        private Vector OriToolInBaseVector;
        private Vector RefToolInBaseVector;

        private async void InspectRefInCamButton_Click(object sender, RoutedEventArgs e)
        {
            var image = await Task.Run(() => _grabService.Grab());
            var result = await Task.Run(() => _refInspector.Inspect(image));

            DefinitionsCircleIndicators.Clear();
            ResultsCircleIndicators.Clear();

            HalconViewer.ClearWindow();
            HalconViewer.Image?.Dispose();
            HalconViewer.Image = image;

            Show_CircleSearchingDefinitions(result.GetCircleSearchingDefinitions());
            Show_CircleSearchingResults(result.Circles);


            var centerX = result.Circles[0].Circle.CenterX;
            var centerY = result.Circles[0].Circle.CenterY;
            RefInCamVector = new Vector(centerX, centerY);

            RefInCamXTextBox.Text = centerX.ToString("0000.000");
            RefInCamYTextBox.Text = centerY.ToString("0000.000");
        }

        private void ImportRobotPointsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog() {Filter = "*.xaml|*.xaml"};
            var ret = dialog.ShowDialog();
            if (ret != true) return;

            var fileName = dialog.FileName;

            var definition = fileName.DeserializeFromXamlFile<HandEyeCalibrationSchema>();
            RobotPoints.Clear();
            RobotPoints.AddRange(definition.RobotPoints);

            MessageBox.Show("Import OK: " + fileName);
        }

        private void ExportRobotPointsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "*.xaml|*.xaml",
                FileName = "HandEyeCalibrationSchema.xaml"
            };
            var ret = dialog.ShowDialog();
            if (ret != true) return;

            var fileName = dialog.FileName;

            var def = GetHandEyeCalibrationDefinition();

            def.SerializeToXamlFile(fileName);

            MessageBox.Show("Export OK: " + fileName);
        }

        private HandEyeCalibrationSchema GetHandEyeCalibrationDefinition()
        {
            var def = new HandEyeCalibrationSchema();
            def.RobotPoints.AddRange(RobotPoints);
            def.OriInCamVector = OriInCamVector;
            def.RefInCamVector = RefInCamVector;
            def.OriToolInBaseVector = OriToolInBaseVector;
            def.RefToolInBaseVector = RefToolInBaseVector;
            def.OriInBaseVector = OriInBaseVector;
            def.RefInBaseVector = RefInBaseVector;
            def.CenterInBaseVector = (RefInBaseVector + OriInBaseVector)/2.0;
            return def;
        }

        private void CalculateOriInBaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (RobotPoints.Count == 0)
            {
                MessageBox.Show("RobotPoints is null");
                return;
            }

//            if (OriToolInBaseVector.Length < 0.0001)
//            {
//                MessageBox.Show("OriToolInBaseVector is null");
//                return;
//            }

            var def = GetHandEyeCalibrationDefinition();

//            var converter = new HHomConverter(
//                def.GetCameraVectors(),
//                def.GetRobotVectors(),
//                def.OriToolInBaseVector);

            var converter = new HHomConverter(
                def.GetCameraVectors(),
                def.GetRobotVectors(),
                new Vector(58.089, 316.247));

//            var oriInBase = converter.ConvertToBase(def.OriInCamVector, def.OriToolInBaseVector);
//            OriInBaseVector = oriInBase;

            var oriInBase = converter.ConvertToBase(new Vector(1667.00558196767, 911.400507038986), new Vector(59.088, 306.243));
            OriInBaseVector = oriInBase;

            OriInBaseXTextBox.Text = oriInBase.X.ToString("0000.000");
            OriInBaseYTextBox.Text = oriInBase.Y.ToString("0000.000");
        }

        private void CalculateRefInBaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (RobotPoints.Count == 0)
            {
                MessageBox.Show("RobotPoints is null");
                return;
            }

//            if (RefToolInBaseVector.Length < 0.0001)
//            {
//                MessageBox.Show("RefToolInBaseVector is null");
//                return;
//            }

            var def = GetHandEyeCalibrationDefinition();

//            var converter = new HHomConverter(
//                def.GetCameraVectors(),
//                def.GetRobotVectors(),
//                def.OriToolInBaseVector);

            var converter = new HHomConverter(
                def.GetCameraVectors(),
                def.GetRobotVectors(),
                new Vector(58.089, 316.247));

//            var refInBase = converter.ConvertToBase(def.RefInCamVector, def.RefToolInBaseVector);
//            RefInBaseVector = refInBase;

            var refInBase = converter.ConvertToBase(new Vector(1635.62220578697, 911.310903190418), new Vector(-40.811, 352.248));
            RefInBaseVector = refInBase;

            RefInBaseXTextBox.Text = refInBase.X.ToString("0000.000");
            RefInBaseYTextBox.Text = refInBase.Y.ToString("0000.000");
        }

        private void InspectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_image != null)
            {
                var result = Inspect(_image);
            }
        }

        private HImage UpdateImage(HImage image)
        {
            lock (UpdateImageLocker)
            {
                HalconViewer.ClearWindow();
                _image?.Dispose();
                _image = image;

                Application.Current.Dispatcher.BeginInvoke(new Action(() => { HalconViewer.Image = image; }));

                return _image;
            }
        }

    }
}