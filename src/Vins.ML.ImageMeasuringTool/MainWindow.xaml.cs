using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Aspose.Cells;
using Hdc;
using Hdc.Collections.ObjectModel;
using Hdc.Controls;
using Hdc.IO;
using Hdc.Linq;
using Hdc.Measuring;
using Hdc.Measuring.Minitab;
using Hdc.Measuring.Reporting;
using Hdc.Measuring.VinsML;
using Hdc.Mv.Halcon;
using Hdc.Reflection;
using Hdc.Serialization;
using Vins.ML.ImageMeasuringTool.Annotations;
using MessageBox = System.Windows.MessageBox;

namespace Vins.ML.ImageMeasuringTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly string _assemblyDirectoryPath;
        private MeasureSchema _measureSchema;
        public MeasureSchema MeasureSchema => _measureSchema;
        private StationResult _currentStationResult;
        private LoadFromFilesMeasureService _measureService;
        private DateTime _bootDateTime = DateTime.Now;
        private XamlReportStationResultProcessor _xamlReportStationResultProcessor;

        public BindableCollection<ImageFileEntry> ImageFileNames { get; set; } =
            new BindableCollection<ImageFileEntry>();

        public MainWindow()
        {
            InitializeComponent();

            WpcCountPerRoundInStationStatisticTextBox.Text = "1";

            _assemblyDirectoryPath = typeof(MainWindow).Assembly.GetAssemblyDirectoryPath();

            //Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.MeasureDevices.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.ADLink.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.EasyNetQ.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Epson.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.GoDEX.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Halcon.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.JAI.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Keyence.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Opc.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Reporting.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Silicon.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.VinsML.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Minitab.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Mv.ImageAcquisition.Halcon.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.EsponTcpSrv.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.QueueScheduling.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.FocalSpec.dll");
            

            _xamlReportStationResultProcessor = new XamlReportStationResultProcessor
            {
                ReportDirectoryPath = "_Reports_Offline_Xaml"
            };

            _measureService = new LoadFromFilesMeasureService();

            _measureService.StationCompletedEvent.Subscribe(
                stationResult =>
                {
                    var currentStationResult = stationResult.DeepClone();
                    currentStationResult.StationIndex = _measureSchema.StationIndex;
                    currentStationResult.ClearCalculationOperations();

                    // import DataCode
                    foreach (var stationResultProcessor in _measureSchema.StationResultProcessors)
                    {
                        var fixTag = stationResultProcessor as GetFixtureDataCodeStationResultProcessor;
                        fixTag?.Process(currentStationResult);

                        var wpcTag = stationResultProcessor as GetWorkpieceDataCodeStationResultProcessor;
                        wpcTag?.Process(currentStationResult);
                    }

                    _xamlReportStationResultProcessor.Process(currentStationResult);

                    CurrentStationResult = currentStationResult;
                });

            ImageDirectoryTextBox.Text = "B:\\";
        }

        private void RunButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(ImageDirectoryTextBox.Text))
            {
                MessageBox.Show("Directory not exist!");
                return;
            }

            _measureService.ImageDirectoryName = ImageDirectoryTextBox.Text;

            _measureSchema = $"{_assemblyDirectoryPath}\\MeasureSchema.xaml"
                .DeserializeFromXamlFile<MeasureSchema>();
            _measureService.Initialize(_measureSchema);

            _measureService.TriggerWorkpieceLocatingReady();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private void RunAllButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(ImageDirectoryTextBox.Text))
            {
                MessageBox.Show("Directory not exist!");
                return;
            }

            _measureService.ImageDirectoryName = ImageDirectoryTextBox.Text;

            _xamlReportStationResultProcessor.ReportDirectoryPath = _measureService.ImageDirectoryName + $"\\" +
                                                                    DateTime.Now.ToString("_yyyy-MM-dd_HH.mm.ss");

            _measureSchema = $"{_assemblyDirectoryPath}\\MeasureSchema.xaml"
                .DeserializeFromXamlFile<MeasureSchema>();
            _measureService.Initialize(_measureSchema);

            _measureService.TriggerWorkpieceLocatingReadyAll();
        }

        private void RenameFiles_OnClick(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(ImageDirectoryTextBox.Text))
            {
                MessageBox.Show("Directory not exist!");
                return;
            }

            var allFileNames = Directory.GetFiles(ImageDirectoryTextBox.Text);
            var fileInfos = allFileNames.Select(x => new FileInfo(x)).ToList();
            var allFileCount = allFileNames.Length;
            var cycleCount = allFileCount / _measureSchema.MeasureDefinitions.Count;

            for (int i = 0; i < cycleCount; i++)
            {
                for (int j = 0; j < _measureSchema.MeasureDefinitions.Count; j++)
                {
                    var newFileName = $"WPC.{(i+1).ToString("D3")}_Photo.{j.ToString("D2")}";
                    var counter = i * _measureSchema.MeasureDefinitions.Count + j;
                    if (counter < allFileCount)
                    {
                        var destFileName = System.IO.Path.Combine(ImageDirectoryTextBox.Text,
                            newFileName + fileInfos[counter].Extension);
                        File.Move(allFileNames[counter], destFileName);
                    }
                }
            }
            ;
        }

        private void SelectDirectoryInStationStatisticButton_OnClick(object sender, RoutedEventArgs e)
        {
            var folderBrowser = new FolderBrowserDialog();
            var result = folderBrowser.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryPathInStationStatisticTextBox.Text = folderBrowser.SelectedPath;
            }
        }

        private void RunInStationStatisticButton_OnClick(object sender, RoutedEventArgs e)
        {
            int wpcCount;
            try
            {
                wpcCount = int.Parse(WpcCountPerRoundInStationStatisticTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("WPC Count Error");
                return;
            }

            var dir = DirectoryPathInStationStatisticTextBox.Text;
            if (!Directory.Exists(dir))
            {
                MessageBox.Show("Directory is not exist");
                return;
            }

            var files = Directory.GetFiles(dir);


            List<StationResult> stationResults = new List<StationResult>();
            foreach (var file in files)
            {
                try
                {
                    StationResult sr = file.DeserializeFromXamlFileWithCheckIfZipped<StationResult>();
                    stationResults.Add(sr);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            if (!stationResults.Any())
            {
                MessageBox.Show("Cannot find any stationResult!");
                return;
            }


            var isContinueous = ContinuesCheckBox.IsChecked.HasValue && ContinuesCheckBox.IsChecked.Value;

            var wpcGroups = stationResults.SpilitWpc(wpcCount, isContinueous);

            var now = DateTime.Now;
            var reportDir = dir + $"\\" + now.ToString("_yyyy-MM-dd_HH.mm.ss");

            Directory.CreateDirectory(reportDir);

            for (int i = 0; i < wpcGroups.Count; i++)
            {
                var @group = wpcGroups[i];
                //@group.SerializeToXamlFile(reportDir + "\\Group_" + i.ToString("D2") + ".xaml");
            }

            var workbook = wpcGroups.GetWorkbookByWpc();

            workbook.Save(reportDir + "\\Report_" + now.ToString("yyyy-MM-dd_HH.mm.ss") + ".xlsx");

            MessageBox.Show("Report successfully created.");
        }

        private void RefreshImageDir_OnClick(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(ImageDirectoryTextBox.Text))
            {
                MessageBox.Show("Directory not exist!");
                return;
            }

            try
            {

                _measureSchema = $"{_assemblyDirectoryPath}\\MeasureSchema.xaml"
                    .DeserializeFromXamlFile<MeasureSchema>();
                _measureService.Initialize(_measureSchema);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var allFileNames = Directory.GetFiles(ImageDirectoryTextBox.Text);
            var allFileCount = allFileNames.Length;
            var defCount = _measureSchema.MeasureDefinitions.Count;
            var cycleCount = allFileCount / defCount;

            ImageFileNames.Clear();


            for (int i = 0; i < cycleCount; i++)
            {
                for (int j = 0; j < defCount; j++)
                {
                    ImageFileNames.Add(new ImageFileEntry()
                    {
                        RoundIndex = i,
                        OffsetIndex = j,
                        FileName = allFileNames[i * defCount + j],
                    });
                }
                //                TriggerWorkpieceLocatingReady(i * _measureSchema.MeasureDefinitions.Count);
            }
            ;
        }

        private void CreateSampleWorkpieceResults_OnClick(object sender, RoutedEventArgs e)
        {
            var dirName = CreateSampleWorkpieceResultsDirTextBox.Text;

            if (!Directory.Exists(dirName))
            {
                MessageBox.Show("Directory not exist!");
                return;
            }


            var wpcCount = int.Parse(CreateSampleWorkpieceResultsWpcCountTextBox.Text);
            var stationCount = int.Parse(CreateSampleWorkpieceResultsStationCountTextBox.Text);

            var now = DateTime.Now;
            for (int i = 0; i < wpcCount; i++)
            {
                var wr = SampleGenerator.CreateWorkpieceResult(i, stationCount);
                wr.SerializeToXamlFile(dirName + "\\SampleWpcResult_" + now.ToString("yyyy-MM-dd_HH.mm.ss") + "_" +
                                       i.ToString("D4") +
                                       ".xaml");
            }

            MessageBox.Show("WpcResults have created at: " + dirName);
        }

        private void RunInInWorkpieceStatisticButton_OnClick(object sender, RoutedEventArgs e)
        {
            int wpcCount;
            try
            {
                wpcCount = int.Parse(WpcCountPerRoundInWorkpieceStatisticTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("WPC Count Error");
                return;
            }

            var dir = DirectoryPathInWorkpieceStatisticTextBox.Text;
            if (!Directory.Exists(dir))
            {
                MessageBox.Show("Directory is not exist");
                return;
            }

            var files = Directory.GetFiles(dir);


            List<WorkpieceResult> workpieceResults = new List<WorkpieceResult>();
            foreach (var file in files)
            {
                try
                {
                    WorkpieceResult sr = file.DeserializeFromXamlFileWithCheckIfZipped<WorkpieceResult>();
                    workpieceResults.Add(sr);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            if (!workpieceResults.Any())
            {
                MessageBox.Show("Cannot find any WorkpieceResult!");
                return;
            }


            if (wpcCount == 1)
            {
                Workbook workbook = new Workbook();
                workbook.Worksheets.Clear();

                Dictionary<string, List<string>> VNamesForMinitab = new Dictionary<string, List<string>>();
                var stationCount = workpieceResults.First().StationResults.Count;
                var stationCountOkworkpieceResults = workpieceResults.Where(
                    x => x.StationResults.Count == stationCount).ToList();
                var workpieceResultsCount = stationCountOkworkpieceResults.Count;
                List<StationResult> allStationResults = new List<StationResult>();

                for (int k = 0; k < stationCount; k++)
                {
                    var stationResults = stationCountOkworkpieceResults.Select(x => x.StationResults[k]).ToList();
                    var stationResult = stationResults.First();

                    var sheetName = "S" + stationResult.StationIndex.ToString("D2");
                    var ws = workbook.Worksheets.Add(sheetName);

                    stationResults.CreateWorksheetByWpc(ws);
                    allStationResults.AddRange(stationResults);

                    VNamesForMinitab.Add(sheetName,
                        stationResult.CalculateResults.Select(x => x.Definition.Name).ToList());

                    ws.AutoFitColumns();
                    //                    ws.Cells.SetColumnWidthPixel(0, 200);
                }

                List<StationResult> allStationResultsMerged = new List<StationResult>();

                for (int i = 0; i < workpieceResultsCount; i++)
                {
                    var sr = new StationResult
                    {
                        StationIndex = allStationResults[i ].StationIndex,
                        StationName = allStationResults[i ].StationName,
                        StationDescription = allStationResults[i ].StationDescription,
                        WorkpieceTag = allStationResults[i ].WorkpieceTag,
                        FixtureDataCode = allStationResults[i ].FixtureDataCode,
                        WorkpieceDataCode = allStationResults[i ].WorkpieceDataCode
                    };

                    for (int j = 0; j < stationCount; j++)
                    {
                        var calculateResults = allStationResults[j * workpieceResultsCount + i].CalculateResults;
                        sr.CalculateResults.AddRange(calculateResults);
                    }

                    allStationResultsMerged.Add(sr);
                }

                var wsOfAll = workbook.Worksheets.Insert(0, SheetType.Worksheet, "All");
                allStationResultsMerged.CreateWorksheetByWpc(wsOfAll);
                wsOfAll.AutoFitColumns();
                //                wsOfAll.Cells.SetColumnWidthPixel(0, 200);
                //                wsOfAll.Cells.SetColumnWidth(0, 10);


                var now = DateTime.Now;
                var reportDir = dir + $"\\" + now.ToString("_yyyy-MM-dd_HH.mm.ss");
                Directory.CreateDirectory(reportDir);

                var fileNameWithoutExt = "Report_" + now.ToString("yyyy-MM-dd_HH.mm.ss");
                var workBookFileName = reportDir + "\\" + fileNameWithoutExt + ".xlsx";

                workbook.Save(workBookFileName);

                //

                var newConfig = new MinitabCommandConfig()
                {
                    OutputMtbFileName = reportDir + "\\" + fileNameWithoutExt + ".mtb",
                    OutputMtbDirectory = reportDir,
                    WOPEN_FilePath = workBookFileName,
                    SHEET_Count = stationCount,
                    SHEET_VNAMES = 17,
                    SHEET_FIRST = 18,
                    SHEET_NROWS = workpieceResultsCount,
                };

                foreach (var v in VNamesForMinitab)
                {
                    foreach (var vName in v.Value)
                    {
                        var entry = new MinitabHistogramEntry
                        {
                            StationName = v.Key,
                            ColumnName = vName,
                            Format = "JPEG"
                        };

                        newConfig.HistogramEntries.Add(entry);
                    }
                }


                var command = MinitabOperations.GetCommand(newConfig);

                using (var fs = new FileStream(newConfig.OutputMtbFileName, FileMode.Create))
                {
                    using (var tw = new StreamWriter(fs))
                    {
                        tw.Write(command);
                    }
                }

                MessageBox.Show("Report successfully created.");
            }
            else
            {
                throw new NotImplementedException();

                var wpcGroups = workpieceResults.SpilitWpc(wpcCount);

                var now = DateTime.Now;
                var reportDir = dir + $"\\" + now.ToString("_yyyy-MM-dd_HH.mm.ss");

                Directory.CreateDirectory(reportDir);

                for (int i = 0; i < wpcGroups.Count; i++)
                {
                    var @group = wpcGroups[i];
                    //@group.SerializeToXamlFile(reportDir + "\\Group_" + i.ToString("D2") + ".xaml");
                }

                //                var workbook = wpcGroups.GetWorkbookByWpc();

                //                workbook.Save(reportDir + "\\Report_" + now.ToString("yyyy-MM-dd_HH.mm.ss") + ".xlsx");

                MessageBox.Show("Report successfully created.");
            }
        }

        private void SelectDirectoryInWorkpieceStatisticButton_OnClick(object sender, RoutedEventArgs e)
        {
            var folderBrowser = new FolderBrowserDialog();
            var result = folderBrowser.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryPathInWorkpieceStatisticTextBox.Text = folderBrowser.SelectedPath;
            }
        }

        private void SelectDirectoryInFixtureStatisticButton_OnClick(object sender, RoutedEventArgs e)
        {
            var folderBrowser = new FolderBrowserDialog();
            var result = folderBrowser.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryPathInFixtureStatisticTextBox.Text = folderBrowser.SelectedPath;
            }
        }

        private void SelectDirectoryInSeparateWorkpiece_OnClick(object sender, RoutedEventArgs e)
        {
            var folderBrowser = new FolderBrowserDialog();
            var result = folderBrowser.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryPathInSeparateWorkpieceStatisticPerDayTextBox.Text = folderBrowser.SelectedPath;
            }
        }

        private void RunInFixtureStatisticButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dir = DirectoryPathInFixtureStatisticTextBox.Text;
            if (!Directory.Exists(dir))
            {
                MessageBox.Show("Directory is not exist");
                return;
            }

            var files = Directory.GetFiles(dir);


            List<WorkpieceResult> workpieceResults = new List<WorkpieceResult>();
            foreach (var file in files)
            {
                try
                {
                    WorkpieceResult sr = file.DeserializeFromXamlFileWithCheckIfZipped<WorkpieceResult>();
                    workpieceResults.Add(sr);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            if (!workpieceResults.Any())
            {
                MessageBox.Show("Cannot find any WorkpieceResult!");
                return;
            }

            Workbook workbook = new Workbook();
            workbook.Worksheets.Clear();



            var groups = workpieceResults.OrderBy(x => x.WorkpieceDataCode).GroupBy(x => x.WorkpieceDataCode).ToList();
            for (int i = 0; i < groups.Count; i++)
            {
                var @group = groups[i];

                var ws = workbook.Worksheets.Add($"{@group.First().WorkpieceDataCode}");
                CreateWorksheetByWpcInFixtureStatistic(@group, ws);
                ws.AutoFitColumns();
            }


            var now = DateTime.Now;
            var reportDir = dir + $"\\" + now.ToString("_yyyy-MM-dd_HH.mm.ss");
            Directory.CreateDirectory(reportDir);

            var fileNameWithoutExt = "Report_" + now.ToString("yyyy-MM-dd_HH.mm.ss");
            var workBookFileName = reportDir + "\\" + fileNameWithoutExt + ".xlsx";

            workbook.Save(workBookFileName);
        }

        private void RunInSeparateWorkpieceButton_OnClick(object sender, RoutedEventArgs e)
        {
            int wpcCount;
            try
            {
                wpcCount = int.Parse(WpcCountPerRoundInSeparateWorkpieceStatisticTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("WPC Count Error");
                return;
            }

            var dir = DirectoryPathInSeparateWorkpieceStatisticPerDayTextBox.Text;
            if (!Directory.Exists(dir))
            {
                MessageBox.Show("Directory is not exist");
                return;
            }

            var files = Directory.GetFiles(dir);


            List<StationResult> statioresults = new List<StationResult>();
            foreach (var file in files)
            {
                try
                {
                    StationResult sr = file.DeserializeFromXamlFileWithCheckIfZipped<StationResult>();
                    statioresults.Add(sr);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            if (!statioresults.Any())
            {
                MessageBox.Show("Cannot find any WorkpieceResult!");
                return;
            }


            if (wpcCount == 1)
            {
                Workbook workbook = new Workbook();
                workbook.Worksheets.Clear();
                List<StationResult> allStationResults = new List<StationResult>();

                for (int k = 0; k < 9; k++)
                {
                    var singleStationResults = statioresults.Where(
                    x => x.StationIndex == k).ToList();
                    if (!singleStationResults.Any())
                        continue;
                    var sheetName = "S" + singleStationResults.First().StationIndex.ToString("D2");
                    var ws = workbook.Worksheets.Add(sheetName);

                    singleStationResults.CreateWorksheetByWpc(ws);
                    allStationResults.AddRange(singleStationResults);

                    ws.AutoFitColumns();
                }

                var now = DateTime.Now;
                var reportDir = dir + $"\\" + now.ToString("_yyyy-MM-dd_HH.mm.ss");
                Directory.CreateDirectory(reportDir);

                var fileNameWithoutExt = "Report_" + now.ToString("yyyy-MM-dd_HH.mm.ss");
                var workBookFileName = reportDir + "\\" + fileNameWithoutExt + ".xlsx";

                workbook.Save(workBookFileName);

                MessageBox.Show("Report successfully created.");
            }

        }

        private void CreateWorksheetByWpcInFixtureStatistic(IEnumerable<WorkpieceResult> @group, Worksheet ws)
        {
            var workpieceResults = @group as IList<WorkpieceResult> ?? @group.ToList();
            foreach (var workpieceResult in workpieceResults)
            {
                foreach (var stationResult in workpieceResult.StationResults)
                {
                    stationResult.WorkpieceDataCode = workpieceResult.WorkpieceDataCode;
                    stationResult.FixtureDataCode = workpieceResult.FixtureDataCode;
                }
            }

            var sortedWpcResults = workpieceResults.OrderBy(x => x.FixtureDataCode);

            var maxStationCount = sortedWpcResults.Max(x => x.StationResults.Count);
            var availableWpcResults = sortedWpcResults.Where(x => x.StationResults.Count == maxStationCount);

            var srs = availableWpcResults.SelectMany(x => x.StationResults).ToList();

            var merged = srs.DistinctBy(x => x.StationIndex);
            int lastColumnIndex = 1;
            foreach (var stationResult in merged)
            {
                var srs2 = srs.Where(x => x.StationIndex == stationResult.StationIndex).ToList();
                lastColumnIndex = srs2.CreateWorksheetByWpcWithColumnOffset(ws, lastColumnIndex);
            }


        }
    }
}