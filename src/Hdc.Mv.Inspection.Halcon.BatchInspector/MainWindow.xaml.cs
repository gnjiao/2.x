using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using HalconDotNet;
using Hdc.Collections.Generic;
using Hdc.Collections.ObjectModel;
using Hdc.Linq;
using Hdc.Mv.Inspection.Halcon.BatchInspector.Properties;
using Hdc.Serialization;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace Hdc.Mv.Inspection.Halcon.BatchInspector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string configFileName = "BatchInspector.Config.xaml";

        public MainWindow()
        {
            InitializeComponent();

            Closing += (sender, e) => Save();

            DataContext = this;

            Closing += MainWindow_Closing;

            if (!File.Exists(configFileName))
            {
                var cfg = new BatchInspectorConfig { InspectionSchemaDirName = "InspectionSchema_Forward" };
                for (int i = 0; i < 10; i++)
                {
                    cfg.Directories.Add(new DirectoryViewModel());
                }
                cfg.SerializeToXamlFile(configFileName);
            }

            Load();

        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        public ObservableCollection<DirectoryViewModel> Directories { get; set; }

        private void Save()
        {
            var c = new BatchInspectorConfig();
            c.Directories.AddRange(Directories);
            c.InspectionSchemaDirName = InspectionSchemaTextBox.Text;
            c.InspectionReportFileName = InspectionReportTextBox.Text;

            c.SerializeToXamlFile(configFileName);
        }

        private void Load()
        {
            var config = configFileName.DeserializeFromXamlFile<BatchInspectorConfig>();

            if (Directories == null)
                Directories = new BindableCollection<DirectoryViewModel>();

            Directories.Clear();
            Directories.AddRange(config.Directories);

            InspectionSchemaTextBox.Text = config.InspectionSchemaDirName;
            InspectionReportTextBox.Text = config.InspectionReportFileName;
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            Save();

            var ret = MessageBox.Show("Are you sure to start? ", "Warning", MessageBoxButton.YesNo);
            if (ret != MessageBoxResult.Yes)
                return;



            List<string> workedDir = new List<string>();

            List<Task> tasks = new List<Task>();

            foreach (var directory in Directories)
            {
                if (string.IsNullOrEmpty(directory.DirectoryPath))
                {
                    //MessageBox.Show("ImageDir is null: " + directory.DirectoryPath);
                    continue;
                }

                workedDir.Add(directory.DirectoryPath);

                // Init
//                InspectionSchema schema = null;

                try
                {
                    if (!Directory.Exists(InspectionSchemaTextBox.Text))
                    {
                        MessageBox.Show("InspectionSchema Dir is not exist!");
                        return;
                    }

                    //                schema = InspectionSchemaTextBox.Text.LoadFromFile();
                    //schema = InspectionSchemaExtensions.LoadFromFile(InspectionSchemaTextBox.Text);
//                    schema = InspectionController.GetInspectionSchema();
                }
                catch (Exception)
                {
                    MessageBox.Show("InspectionSchema loading error!");
                    return;
                }

                var directoryPath = directory.DirectoryPath;
                var schemaDir = InspectionSchemaTextBox.Text;

                var task = new Task(() =>
                {
                    Run(directoryPath, schemaDir);
                });
                tasks.Add(task);
                task.Start();
            }

            Task.WaitAll(tasks.ToArray());

            string dirs = string.Empty;
            foreach (var dir in workedDir)
            {
                dirs += dir + "\n";
            }

            MessageBox.Show("Directories exported:\n\n" + dirs);
        }

        public void Run(string imageDir, string schemaDir)
        {
            IList<string> fileNames = new List<string>();
            try
            {
                var strings = Directory.GetFiles(imageDir).ToList();

                var imageFileNames = strings.Where(x =>
                {
                    if (x.EndsWith(".bmp"))
                        return true;

                    if (x.EndsWith(".tif"))
                        return true;

                    if (x.EndsWith(".jpg"))
                        return true;

                    return false;
                }).ToList();

                fileNames.AddRange(imageFileNames);

                var subDirNames = Directory.GetDirectories(imageDir);
                foreach (var subDirName in subDirNames)
                {
                    var substrings = Directory.GetFiles(subDirName).ToList();
                    var imageFileNames2 = substrings.Where(x =>
                    {
                        if (x.EndsWith(".bmp"))
                            return true;

                        if (x.EndsWith(".tif"))
                            return true;

                        if (x.EndsWith(".jpg"))
                            return true;

                        return false;
                    }).ToList();

                    fileNames.AddRange(imageFileNames2);
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Image directory cannot load files! " + imageDir);
                return;
            }


            // Inspect
            IList<InspectionResult> inspectionResults = new List<InspectionResult>();

            IList<HImage> images = new List<HImage>();

            foreach (var fileName in fileNames)
            {
                string name = fileName;

                var fn = (string)name;
                Debug.WriteLine("Task.Started: " + fn);

                images.Add(new HImage(name));
            }

//            IList<Task> tasks = new List<Task>();

            foreach (var imageInfo in images)
            {
                var imageInfo2 = imageInfo;
//                var task = new Task(
//                    (x) =>
//                    {
                        HImage imageInfo3 = (HImage)imageInfo2;


                        var inspector = new HalconInspectionSchemaInspector()
                        {
                            InspectionSchemaDir = schemaDir,
                        };
                        var result = inspector.Inspect(imageInfo3);
                        inspectionResults.Add(result);
                        

                        //                var targetTask = new SearchingTask();
                        //                foreach (var csd in schema.CircleSearchingDefinitions)
                        //                {
                        //                    var relativeVector = new Vector(csd.BaselineX*1000.0/16.0, csd.BaselineY*1000.0/16.0);
                        //                    var originalVector = coord.GetOriginalVector(relativeVector);
                        //                    csd.CenterX = originalVector.X;
                        //                    csd.CenterY = originalVector.Y;
                        //                }
                        //                targetTask.CircleDefinitions.AddRange(schema.CircleSearchingDefinitions);
                        //
                        //
                        //                var targetResult = inspector.Search(imageInfo, targetTask);
                        //
                        //                targetResult.CircleSearchingResults.UpdateRelativeCoordinate(coord);

//                    }, imageInfo);
//                tasks.Add(task);
//                task.Start();

            }

//            Task.WaitAll(tasks.ToArray());

            var coordinateResultGroups = inspectionResults.Select(x => x.CoordinateCircles).ToList();
            List<CircleSearchingResultCollection> objectsResultGroups = inspectionResults.Select(x => x.Circles).ToList();

            foreach (var task in objectsResultGroups)
            {
                foreach (var t in task)
                {
                    Debug.WriteLine("objectsResultGroups Circle " + t.Index + " X: " + t.Definition.CenterX);
                    Debug.WriteLine("objectsResultGroups Circle " + t.Index + " Y: " + t.Definition.CenterY);
                }
            }

            DateTime dateTime = DateTime.Now;
            string reportDir = "_reports" + dateTime.ToString("_yyyy-MM-dd_HH.mm.ss");
            var exportDir = Path.Combine(imageDir, reportDir);
            Directory.CreateDirectory(exportDir);

            // SaveToCSV
            ReportManager.SaveToCSV(coordinateResultGroups, exportDir, "Coordinate");
            ReportManager.SaveToCSV(objectsResultGroups, exportDir, "Objects");
            Debug.WriteLine("SaveToCSV() end");


            var distGroups = inspectionResults.Select(x => x.DistanceBetweenPointsResults).ToList();
            ReportManager.SaveCsvGroupByEdge(distGroups, exportDir, "Edges");
            Debug.WriteLine("SaveCsvGroupByEdge() end");


            List<RegionDefectResult> defectsGroups = inspectionResults.SelectMany(x => x.RegionDefectResults).ToList();
            var dgs = defectsGroups.Select(x => x.DefectResults).ToList();
            ReportManager.SaveDefectResultsToCsvGroupByWorkpiece(dgs, exportDir, "Defects");
            Debug.WriteLine("SaveDefectResultsToCsvGroupByWorkpiece() end");

            // SaveToXaml
            ReportManager.SaveToXaml(coordinateResultGroups, exportDir, "Coordinate");
            ReportManager.SaveToXaml(objectsResultGroups, exportDir, "Objects");
            ReportManager.SaveToXaml(inspectionResults, exportDir, "All");
            Debug.WriteLine("SaveToXaml() end");


            var cs = CoordinateCircleCalculator.Calculate(inspectionResults);


            //            var allResultGroup = new List<CircleSearchingResultCollection>();
            //            allResultGroup.AddRange(coordinateResultGroups);
            //            allResultGroup.AddRange(objectsResultGroups);
            //            ReportManager.SaveToCSV(allResultGroup, exportDir, "All");
            //            ReportManager.SaveToXaml(allResultGroup, exportDir, "All");
        }

        private void SelectImageDirectoryButton_OnClick(object sender, RoutedEventArgs e)
        {
            var x = new FolderBrowserDialog();
            x.ShowDialog();
        }

        private void SelectInspectionSchemaFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                FileName = "InspectionSchema",
                Filter = "*.xaml|XAML",
                DefaultExt = ".xaml"
            };
            DialogResult ok = dialog.ShowDialog();
            if (ok != System.Windows.Forms.DialogResult.OK)
                return;

            var fileName = dialog.FileName;

            InspectionSchemaTextBox.Text = fileName;

            Save();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Save();

            MessageBox.Show(configFileName + " has saved.");
        }

        private void CalculateButton_OnClick(object sender, RoutedEventArgs e)
        {
            Save();

            var fileName = InspectionReportTextBox.Text;

            var inspectionReport = fileName.DeserializeFromXamlFile<InspectionReport>();

            var cs = CoordinateCircleCalculator.Calculate(inspectionReport.Results);

            var dt = DateTime.Now.ToString("_yyyy-MM-dd_HH.mm.ss");

            var newFileName = fileName + dt + ".csv";
            using (var csvWriterContainer = new CsvWriterContainer(newFileName))
            {
                var writer = csvWriterContainer.CsvWriter;

                writer.WriteField("Name.");
                writer.WriteField("X");
                writer.WriteField("Y");
                writer.WriteField("Angle");
                writer.WriteField("Length");
                writer.WriteField("|");
                writer.WriteField("X mm");
                writer.WriteField("Y mm");
                writer.WriteField("Angle");
                writer.NextRecord();

                foreach (ReferenceCircleInfo c in cs)
                {
                    writer.WriteField(c.Name);
                    writer.WriteField(c.Offset.X);
                    writer.WriteField(c.Offset.Y);
                    writer.WriteField(c.Angle);
                    writer.WriteField(c.Length);
                    writer.WriteField("|");
                    writer.WriteField(c.Offset.X.ToNumbericStringInMillimeterFromPixel(16));
                    writer.WriteField(c.Offset.Y.ToNumbericStringInMillimeterFromPixel(16));
                    writer.WriteField(c.Angle);
                    writer.NextRecord();
                }
            }

            MessageBox.Show(newFileName + " has exported.");
        }
    }
}