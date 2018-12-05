using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using HalconDotNet;
using Hdc.Collections.ObjectModel;
using Hdc.Mv.Halcon;
using Hdc.Mv.Halcon.DefectDetection;
using Hdc.Mv.Halcon.Mvvm;
using Hdc.Mv.Inspection.Halcon.SampleApp.Annotations;
using Hdc.Mv.Mvvm;
using Hdc.Serialization;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;

// ReSharper disable InconsistentNaming

namespace Hdc.Mv.Inspection.Halcon.SampleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public BindableCollection<RegionIndicatorViewModel> RegionIndicators { get; set; } = new BindableCollection<RegionIndicatorViewModel>();
        public BindableCollection<RegionIndicatorViewModel> RegionIndicators_Edges { get; set; } = new BindableCollection<RegionIndicatorViewModel>();
        public BindableCollection<RegionViewModel> Regions { get; set; } = new BindableCollection<RegionViewModel>();
        public BindableCollection<RoiRectangleViewModel> RoiRectangleViewModels_CoordinateEdges { get; set; } = new BindableCollection<RoiRectangleViewModel>();
        public BindableCollection<RoiRectangleViewModel> RoiRectangleViewModels_DataCodeSearching { get; set; } = new BindableCollection<RoiRectangleViewModel>();
        public BindableCollection<RoiRectangleViewModel> RoiRectangleViewModels_CoordinateExtractors_IntersectionCoordinateExtactors { get; set; } = new BindableCollection<RoiRectangleViewModel>();
        public BindableCollection<RectangleIndicatorViewModel> ObjectIndicators { get; set; } = new BindableCollection<RectangleIndicatorViewModel>();
        public BindableCollection<LineIndicatorViewModel> LineIndicators_Edges { get; set; } = new BindableCollection<LineIndicatorViewModel>();
        public BindableCollection<LineIndicatorViewModel> LineIndicators_EdgesDistances { get; set; } = new BindableCollection<LineIndicatorViewModel>();
        public BindableCollection<LineIndicatorViewModel> LineIndicators_CoordinateEdges { get; set; } = new BindableCollection<LineIndicatorViewModel>();
        public BindableCollection<LineIndicatorViewModel> LineIndicators_ReferenceLines { get; set; } = new BindableCollection<LineIndicatorViewModel>();
        public BindableCollection<LineIndicatorViewModel> LineIndicators_ReferenceRadialLine { get; set; } = new BindableCollection<LineIndicatorViewModel>();
        public BindableCollection<LineIndicatorViewModel> LineIndicators_PointOfXldAndRadialLines { get; set; } = new BindableCollection<LineIndicatorViewModel>();
        public BindableCollection<LineIndicatorViewModel> LineIndicators_DistanceBetweenPointsOfXldAndRadialLines { get; set; } = new BindableCollection<LineIndicatorViewModel>();
        public BindableCollection<LineIndicatorViewModel> LineIndicators_CircleDiameter { get; set; } = new BindableCollection<LineIndicatorViewModel>();
        public BindableCollection<LineIndicatorViewModel> LineIndicators_RegionTargetResults { get; set; } = new BindableCollection<LineIndicatorViewModel>();
        public BindableCollection<LineIndicatorViewModel> LineIndicators_DistanceBetweenLinesResults { get; set; } = new BindableCollection<LineIndicatorViewModel>();
        public BindableCollection<LineIndicatorViewModel> LineIndicators_DistanceBetweenPointsResults { get; set; } = new BindableCollection<LineIndicatorViewModel>();
        public BindableCollection<LineIndicatorViewModel> LineIndicators_CoordinateExtractors_IntersectionCoordinateExtactors { get; set; } = new BindableCollection<LineIndicatorViewModel>();
        public BindableCollection<CircleIndicatorViewModel> DefinitionsCircleIndicators { get; set; } = new BindableCollection<CircleIndicatorViewModel>();
        public BindableCollection<CircleIndicatorViewModel> ResultsCircleIndicators { get; set; } = new BindableCollection<CircleIndicatorViewModel>();
        //        public BindableCollection<DefectInfo> DefectInfos { get; set; }
        public BindableCollection<DefectResult> DefectResults { get; set; } = new BindableCollection<DefectResult>();
        public BindableCollection<DefectViewModel> Defects { get; set; } = new BindableCollection<DefectViewModel>();
        public BindableCollection<DefectViewModel> HighlightDefects { get; set; } = new BindableCollection<DefectViewModel>();
        public BindableCollection<DefectViewModel> SurfaceDefects { get; set; } = new BindableCollection<DefectViewModel>();
        public BindableCollection<XldIndicatorViewModel> XldIndicators { get; set; } = new BindableCollection<XldIndicatorViewModel>();
        public BindableCollection<CircleIndicatorViewModel> ReferencePointUsingCircleIndicators { get; set; } = new BindableCollection<CircleIndicatorViewModel>();
        public BindableCollection<LineIndicatorViewModel> LineIndicators_DistanceBetweenTwoPointsResults { get; set; } = new BindableCollection<LineIndicatorViewModel>();

        private InspectionController InspectionController;
        //        private InspectionSchema _inspectionSchema;
        private string _testImageFilePath;
        private string InspectionSchemaDir;
        private InspectionResult inspectionResult;

        public BindableCollection<MeasurementInfoViewModel> MeasurementInfos { get; set; } = new BindableCollection<MeasurementInfoViewModel>();
        //        public ICollectionView MeasurementInfosCollectionView { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _halconMvSchema = "HalconMvSchema.xaml".DeserializeFromXamlFile<HalconMvSchema>();
            //            var inspector = _halconMvSchema.FrameGrabberSchemas.First().Inspector;

            InspectionSchemaDir = "InspectionSchema_Forward";
            InspectionSchemaDirTextBlock.Text = InspectionSchemaDir;

            this.DataContext = this;
            this.Closing += MainWindow_Closing;

            InspectionController = new InspectionController();

            //Refresh();

            SelectDefectInfoCommand = new DelegateCommand<DefectResult>(
                x =>
                {
                    if (x == null)
                        return;

                    //                    Defects.Clear();
                    //                    Defects.Add(x.ToViewModel());

                    //                    foreach (var result in inspectionResult.RegionDefectResults.SelectMany(y => y.DefectResults))
                    DefectResults.ForEach(y =>
                    {
                        y.DisplayHighlight = false;
                        y.DisplayEnabled = false;
                    });

                    //                    x.DisplayEnabled = true;
                    x.DisplayHighlight = true;

                    ShowInspectionResult(hImage, inspectionResult);
                    HalconViewer.ZoomToRectangle(x.X, x.Y, x.Width, x.Height, 0.1);
                });

            OpenFileCommand = new DelegateCommand(
                () =>
                {
                    string fileName;
                    if (UseOpenFileDialog(out fileName)) return;

                    _testImageFilePath = fileName;

                    if (_halconMvSchema.FrameGrabberSchemas.Count > 1)
                    {

                    }

                    InspectionSchemaDir = DefaultInspectionSchemaDir;

                    Refresh();
                });

            CtorOpenFileCommands();
        }

        private void OpenFileXCommandInner(int index)
        {
            string fileName;
            if (UseOpenFileDialog(out fileName)) return;

            _testImageFilePath = fileName;

            var halconFrameGrabberSchema = _halconMvSchema.FrameGrabberSchemas.SingleOrDefault(x => x.Index == index);

            if (halconFrameGrabberSchema == null)
            { 
                MessageBox.Show("_halconMvSchema.FrameGrabberSchemas is not exist");
                return;
            }

            var inspector = halconFrameGrabberSchema.Inspector as HalconInspectionSchemaInspector;
            InspectionSchemaDir = inspector.InspectionSchemaDir;

            Refresh();
        }

        private static bool UseOpenFileDialog(out string fileName)
        {
            fileName = null;
            var dialog = new OpenFileDialog()
            {
                Filter = "Image File(*.tif,*.bmp,*.jpg,*.png)|*.tif;*.bmp;*.jpg;*.png",
            };
            var ret = dialog.ShowDialog();
            if (ret != true)
            {
//                MessageBox.Show("OpenFileDialog error");
                return true;
            }

            fileName = dialog.FileName;
            return false;
        }

        private void Refresh()
        {
            if (!File.Exists(_testImageFilePath))
            {
                MessageBox.Show("Image file not exist");
                return;
                //                throw new HalconInspectorException("Image file not exist");
            }

            HalconInspectionSchemaInspector halconInspector;


            halconInspector = new HalconInspectionSchemaInspector
            {
                InspectionSchemaDir = InspectionSchemaDir
            };

/*            if (InspectionSchemaDir.IsNotNullOrEmpty() && (InspectionSchemaDir == DefaultInspectionSchemaDir))
            {
                halconInspector = new HalconInspectionSchemaInspector
                {
                    InspectionSchemaDir = DefaultInspectionSchemaDir
                };
            }
            else
            {
                var halconInspectors = _halconMvSchema.FrameGrabberSchemas
                        .Select(x => x.Inspector as HalconInspectionSchemaInspector)
                        .Where(x => x.InspectionSchemaDir == InspectionSchemaDir)
                        .ToList();

                if (!halconInspectors.Any())
                {
                    MessageBox.Show($"InspectionSchemaDir for Inspector is not found: {InspectionSchemaDir}");
                    return;
                }

                halconInspector = halconInspectors.First();
            }*/


//            switch (InspectionSchemaDir)
//            {
//                case "InspectionSchema_Forward":
//                    halconInspector = _halconMvSchema.FrameGrabberSchemas[0].Inspector as HalconInspectionSchemaInspector;
//                    break;
//                case "InspectionSchema_Backward":
//                    halconInspector = _halconMvSchema.FrameGrabberSchemas[1].Inspector as HalconInspectionSchemaInspector;
//                    break;
//            }

            hImage?.Dispose();
            inspectionResult?.Dispose();

            hImage = new HImage();
            hImage.ReadImage(_testImageFilePath);

            halconInspector.UpdateInspectionSchemaFromDir();
            inspectionResult = halconInspector.Inspect(hImage);

            /*            for (int i = 0; i < 10000; i++)
                        {
                            inspectionResult = halconInspector.Inspect(hImage);
                            if (i%20 == 0)
                            {
                                Thread.Sleep(3000);
                                Debug.WriteLine("Inspect " + i + " times, begin");
                                Thread.Sleep(3000);
                                GC.Collect();
                                GC.Collect();
                                Debug.WriteLine("Inspect " + i + " times, collected");
                                Thread.Sleep(3000);
                                Debug.WriteLine("Inspect " + i + " times, end");
                            }
                        }*/


            //
            var regionDefectResultsList = inspectionResult.RegionDefectResults.ToList();
            //            var defectResults = regionDefectResultsList.SelectMany(x => x.DefectResults).ToList();

            ShowDefectList(regionDefectResultsList);

            ShowMeasurementInfosList();

            ShowInspectionResult(hImage, inspectionResult);
        }

        private void ShowMeasurementInfosList()
        {
            MeasurementInfos.Clear();
            var DistanceBetweenPointsResultMeasurementInfos = inspectionResult.DistanceBetweenPointsResults
                .GetMeasurementInfos(inspectionResult.Coordinate);
            MeasurementInfos.AddRange(DistanceBetweenPointsResultMeasurementInfos);

            var circleMeasurementInfos = inspectionResult.Circles
                .GetMeasurementInfos(inspectionResult.Coordinate);
            MeasurementInfos.AddRange(circleMeasurementInfos);

            var dataCodeMeasurementInfos = inspectionResult.DataCodeSearchingResults
                .GetMeasurementInfos(inspectionResult.Coordinate);
            MeasurementInfos.AddRange(dataCodeMeasurementInfos);

            var PointOfXldAndRadialLineMeasurementInfos = inspectionResult.PointOfXldAndRadialLineResults
                ?.GetMeasurementInfos(inspectionResult.Coordinate);
            if (PointOfXldAndRadialLineMeasurementInfos != null)
                MeasurementInfos.AddRange(PointOfXldAndRadialLineMeasurementInfos);

            var DistanceBetweenPointsOfXldAndRadialLineMeasurementInfos = inspectionResult.DistanceBetweenPointsOfXldAndRadialLineResults
                ?.GetMeasurementInfos(inspectionResult.Coordinate);
            if (DistanceBetweenPointsOfXldAndRadialLineMeasurementInfos != null)
                MeasurementInfos.AddRange(DistanceBetweenPointsOfXldAndRadialLineMeasurementInfos);

            var DistanceBetweenTwoPointsResultsMeasurementInfos = inspectionResult.DistanceBetweenTwoPointsResults
                ?.GetMeasurementInfos(inspectionResult.Coordinate);

            if (DistanceBetweenTwoPointsResultsMeasurementInfos != null)
                MeasurementInfos.AddRange(DistanceBetweenTwoPointsResultsMeasurementInfos);

            var StepFromGrayValueMeasurementInfos = inspectionResult.StepFromGrayValueResults?.GetMeasurementInfos(inspectionResult.Coordinate);

            if (StepFromGrayValueMeasurementInfos != null)
                MeasurementInfos.AddRange(StepFromGrayValueMeasurementInfos);
            //            MeasurementInfosCollectionView.Filter = null;
            //            MeasurementInfosCollectionView.Refresh();
        }

        private void ShowDefectList(List<RegionDefectResult> regionDefectResultsList)
        {
            var blobs = regionDefectResultsList
                .Where(x => x.DefectRegion != null)
                .Select(x => x.DefectRegion);

            var unionBlobs = blobs.Union();
            var connectionBlobs = unionBlobs.Connection().ToList();

            var defectResults = connectionBlobs.Select(x => x.ToDefectResult());

            DefectResults.Clear();
            foreach(var rr in regionDefectResultsList)
            {
                DefectResult dr = new DefectResult();
                dr.defectArea = rr.DefectArea;
                dr.Location = rr.Location;
                DefectResults.Add(dr);
            }

            // DefectResults.AddRange(defectResults);
        }

        private HImage hImage;

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            InspectionController.Dispose();
        }

        private void ShowInspectionResult(HImage image, InspectionResult result)
        {
            Regions.Clear();
            RegionIndicators.Clear();
            RegionIndicators_Edges.Clear();
            RoiRectangleViewModels_CoordinateEdges.Clear();
            RoiRectangleViewModels_DataCodeSearching.Clear();
            RoiRectangleViewModels_CoordinateExtractors_IntersectionCoordinateExtactors.Clear();

            LineIndicators_Edges.Clear();
            LineIndicators_EdgesDistances.Clear();
            LineIndicators_CoordinateEdges.Clear();
            LineIndicators_ReferenceLines.Clear();
            LineIndicators_ReferenceRadialLine.Clear();
            LineIndicators_PointOfXldAndRadialLines.Clear();
            LineIndicators_DistanceBetweenPointsOfXldAndRadialLines.Clear();
            LineIndicators_CircleDiameter.Clear();
            LineIndicators_RegionTargetResults.Clear();
            LineIndicators_DistanceBetweenLinesResults.Clear();
            LineIndicators_DistanceBetweenPointsResults.Clear();
            LineIndicators_DistanceBetweenTwoPointsResults.Clear();
            LineIndicators_CoordinateExtractors_IntersectionCoordinateExtactors.Clear();

            DefinitionsCircleIndicators.Clear();
            ResultsCircleIndicators.Clear();
            ObjectIndicators.Clear();
            XldIndicators.Clear();

            ReferencePointUsingCircleIndicators.Clear();

            HalconViewer.ClearWindow();
            HalconViewer.Image = image;


            // CoordinateCircles
            Show_CircleSearchingDefinitions(
                result.GetCoordinateCircleSearchingDefinitions());
            Show_CircleSearchingResults(result.CoordinateCircles);

            // CoordinateEdges
            Show_EdgeSearchingDefinitions(result.GetCoordinateEdges());
            Show_CoordinateEdges(result.CoordinateEdges);

            // CoordinateRegions
            Show_RegionSearchingResult(result.CoordinateRegions);

            // Edges
            Show_EdgeSearchingDefinitions(result.GetEdgeSearchingDefinitions());
            Show_Edges(result.Edges);

            // Circles
            Show_CircleSearchingDefinitions(result.GetCircleSearchingDefinitions());
            Show_CircleSearchingResults(result.Circles);
            Show_ReferencePointUsingCircleIndicators(result.ReferencePointDefinitions);

            // DistanceBetweenPoints
            Show_DistanceBetweenPointsResults(result.DistanceBetweenPointsResults);
            Show_DistanceBetweenTwoPointsResults(result.DistanceBetweenTwoPointsResults);

            // Defects
            Show_DefectResults(result.RegionDefectResults);

            // Parts
            Show_PartSearchingDefinitions(result.GetPartSearchingDefinitions());
            Show_PartSearchingResults(result.Parts);

            // RegionTargets
            Show_RegionTargetDefinitions(result.GetRegionTargetDefinitions());
            Show_RegionTargetResults(result.RegionTargets);

            // DataCodeSearching
            Show_DataCodeSearchingDefinitions(result.GetDataCodeSearchingDefinitions());

            // DataCodeSearching
            Show_XldSearchingResults(result.XldSearchingResults);

            // Show_ReferenceLines
            Show_ReferenceLines(result.ReferenceLineDefinitions);

            // Show_ReferenceLines
            Show_ReferenceRadialLineDefinitions(result.ReferenceRadialLineDefinitions);

            // Show_ReferenceLines
            Show_PointOfXldAndRadialLineResults(result.PointOfXldAndRadialLineResults);

            // Show_DistanceBetweenPointsOfXldAndRadialLineResults
            Show_DistanceBetweenPointsOfXldAndRadialLineResults(result.DistanceBetweenPointsOfXldAndRadialLineResults);

            // Show_DistanceBetweenPointsOfXldAndRadialLineResults
            Show_InspectionSchema_CoordinateExtractors(result.InspectionSchema);
        }

        private void Show_ReferencePointUsingCircleIndicators(IEnumerable<PointDefinition> referencePointDefinitions)
        {
            var cis = new List<CircleIndicatorViewModel>();
            foreach (var circleSearchingDefinition in referencePointDefinitions)
            {
                var innerCircle = new CircleIndicatorViewModel()
                {
                    CenterX = circleSearchingDefinition.ActualX,
                    CenterY = circleSearchingDefinition.ActualY,
                    Radius = 5,
                };
                cis.Add(innerCircle);
            }
            ReferencePointUsingCircleIndicators.AddRange(cis);
        }

        private void Show_InspectionSchema_CoordinateExtractors(InspectionSchema inspectionSchema)
        {
            List<RoiRectangleViewModel> roiRectangleViewModels = new List<RoiRectangleViewModel>();
            List<LineIndicatorViewModel> lineIndicatorViewModels = new List<LineIndicatorViewModel>();

            foreach (var coordinateExtactor in inspectionSchema.CoordinateExtactors)
            {
                var IntersectionCoordinateExtactor = coordinateExtactor as IntersectionCoordinateExtactor;
                if (IntersectionCoordinateExtactor == null) continue;

                var LineInCoordinateExtractor1 =
                    IntersectionCoordinateExtactor.PrimaryLineExtractor as LineInCoordinateExtractor;

                var LineInCoordinateExtractor2 =
                    IntersectionCoordinateExtactor.SecondaryLineExtractor as LineInCoordinateExtractor;

                // ROI 1
                var roi1 = new RoiRectangleViewModel()
                {
                    StartX = LineInCoordinateExtractor1.RoiActualLine.X1,
                    StartY = LineInCoordinateExtractor1.RoiActualLine.Y1,
                    EndX = LineInCoordinateExtractor1.RoiActualLine.X2,
                    EndY = LineInCoordinateExtractor1.RoiActualLine.Y2,
                    HalfWidth = LineInCoordinateExtractor1.RoiHalfWidth,
                };
                roiRectangleViewModels.Add(roi1);

                // ROI 1
                var roi2 = new RoiRectangleViewModel()
                {
                    StartX = LineInCoordinateExtractor2.RoiActualLine.X1,
                    StartY = LineInCoordinateExtractor2.RoiActualLine.Y1,
                    EndX = LineInCoordinateExtractor2.RoiActualLine.X2,
                    EndY = LineInCoordinateExtractor2.RoiActualLine.Y2,
                    HalfWidth = LineInCoordinateExtractor2.RoiHalfWidth,
                };
                roiRectangleViewModels.Add(roi2);

                // Line 1
                var distanceLineIndicator = new LineIndicatorViewModel
                {
                    StartPointX = IntersectionCoordinateExtactor.PrimaryLine.X1,
                    StartPointY = IntersectionCoordinateExtactor.PrimaryLine.Y1,
                    EndPointX = IntersectionCoordinateExtactor.PrimaryLine.X2,
                    EndPointY = IntersectionCoordinateExtactor.PrimaryLine.Y2,
                };
                lineIndicatorViewModels.Add(distanceLineIndicator);

                // Line 2
                var distanceLineIndicator2 = new LineIndicatorViewModel
                {
                    StartPointX = IntersectionCoordinateExtactor.SecondaryLine.X1,
                    StartPointY = IntersectionCoordinateExtactor.SecondaryLine.Y1,
                    EndPointX = IntersectionCoordinateExtactor.SecondaryLine.X2,
                    EndPointY = IntersectionCoordinateExtactor.SecondaryLine.Y2,
                };
                lineIndicatorViewModels.Add(distanceLineIndicator2);
            }

            RoiRectangleViewModels_CoordinateExtractors_IntersectionCoordinateExtactors.AddRange(roiRectangleViewModels);
            LineIndicators_CoordinateExtractors_IntersectionCoordinateExtactors.AddRange(lineIndicatorViewModels);
        }

        private void Show_PointOfXldAndRadialLineResults(IEnumerable<PointOfXldAndRadialLineResult> results)
        {
            //            var isNotifying = LineIndicators.IsNotifying;
            //            LineIndicators.IsNotifying = false;
            var LineIndicatorViewModels = new List<LineIndicatorViewModel>();
            foreach (var result in results)
            {
                var distanceLineIndicator = new LineIndicatorViewModel
                {
                    StartPointX = result.X + 10,
                    StartPointY = result.Y + 10,
                    EndPointX = result.X - 10,
                    EndPointY = result.Y - 10,
                };
                LineIndicatorViewModels.Add(distanceLineIndicator);

                var distanceLineIndicatorY = new LineIndicatorViewModel
                {
                    StartPointX = result.X + 10,
                    StartPointY = result.Y - 10,
                    EndPointX = result.X - 10,
                    EndPointY = result.Y + 10,
                };
                LineIndicatorViewModels.Add(distanceLineIndicatorY);
            }

            LineIndicators_PointOfXldAndRadialLines.AddRange(LineIndicatorViewModels);
            //            LineIndicators_Edges.IsNotifying = isNotifying;
            //            LineIndicators_Edges.Refresh();
        }

        private void Show_DistanceBetweenPointsOfXldAndRadialLineResults(IEnumerable<DistanceBetweenPointsOfXldAndRadialLineResult> results)
        {
            var LineIndicatorViewModels = new List<LineIndicatorViewModel>();
            foreach (var result in results)
            {
                var distanceLineIndicator = new LineIndicatorViewModel
                {
                    StartPointX = result.X1,
                    StartPointY = result.Y1,
                    EndPointX = result.X2,
                    EndPointY = result.Y2,
                };
                LineIndicatorViewModels.Add(distanceLineIndicator);
            }

            LineIndicators_DistanceBetweenPointsOfXldAndRadialLines.AddRange(LineIndicatorViewModels);
        }

        private void Show_XldSearchingResults(IEnumerable<XldSearchingResult> xldSearchingResults)
        {
            var indicators = new List<XldIndicatorViewModel>();
            foreach (var result in xldSearchingResults)
            {
                var indicatorViewModel = new XldIndicatorViewModel()
                {
                    Xld = result.Xld,
                };
                indicators.Add(indicatorViewModel);
            }
            XldIndicators.AddRange(indicators);
        }

        public void Show_DistanceBetweenLinesResults(
            IEnumerable<DistanceBetweenLinesResult> distanceBetweenLinesResults)
        {
            var isNotifying = LineIndicators_DistanceBetweenLinesResults.IsNotifying;
            LineIndicators_DistanceBetweenLinesResults.IsNotifying = false;

            foreach (var result in distanceBetweenLinesResults)
            {
                var distanceLineIndicator = new LineIndicatorViewModel
                {
                    StartPointX = result.FootPoint1.X,
                    StartPointY = result.FootPoint1.Y,
                    EndPointX = result.FootPoint2.X,
                    EndPointY = result.FootPoint2.Y,
                };
                LineIndicators_DistanceBetweenLinesResults.Add(distanceLineIndicator);
            }
            LineIndicators_DistanceBetweenLinesResults.IsNotifying = isNotifying;
            LineIndicators_DistanceBetweenLinesResults.Refresh();
        }

        
        public void Show_DistanceBetweenTwoPointsResults(
            IEnumerable<DistanceBetweenTwoPointsResult> pointsResultCollection)
        {
            var isNotifying = LineIndicators_DistanceBetweenTwoPointsResults.IsNotifying;
            LineIndicators_DistanceBetweenTwoPointsResults.IsNotifying = false;
            
            foreach (var result in pointsResultCollection)
            {
                var distanceLineIndicator = new LineIndicatorViewModel
                {
                    StartPointX = result.StartPointXPath,
                    StartPointY = result.StartPointYPath,
                    EndPointX = result.EndPointXPath,
                    EndPointY = result.EndPointYPath
                };
                LineIndicators_DistanceBetweenTwoPointsResults.Add(distanceLineIndicator);
            }
            LineIndicators_DistanceBetweenTwoPointsResults.IsNotifying = isNotifying;
            LineIndicators_DistanceBetweenTwoPointsResults.Refresh();            
        }

        public void Show_DistanceBetweenPointsResults(
            DistanceBetweenPointsResultCollection pointsResultCollection)
        {
            var isNotifying = LineIndicators_DistanceBetweenPointsResults.IsNotifying;
            LineIndicators_DistanceBetweenPointsResults.IsNotifying = false;

            var isNotifying2 = LineIndicators_EdgesDistances.IsNotifying;
            LineIndicators_EdgesDistances.IsNotifying = false;

            foreach (var result in pointsResultCollection)
            {
                var distanceLineIndicator = new LineIndicatorViewModel
                {
                    StartPointX = result.Point1.X,
                    StartPointY = result.Point1.Y,
                    EndPointX = result.Point2.X,
                    EndPointY = result.Point2.Y,
                };
                LineIndicators_DistanceBetweenPointsResults.Add(distanceLineIndicator);
                LineIndicators_EdgesDistances.Add(distanceLineIndicator);
            }
            LineIndicators_DistanceBetweenPointsResults.IsNotifying = isNotifying;
            LineIndicators_DistanceBetweenPointsResults.Refresh();

            LineIndicators_EdgesDistances.IsNotifying = isNotifying2;
            LineIndicators_EdgesDistances.Refresh();
        }

        public void Show_CircleSearchingResults(IList<CircleSearchingResult> CircleSearchingResults, Brush brush = null)
        {
            if (brush == null)
                brush = Brushes.Lime;

            var isNotifying = LineIndicators_CircleDiameter.IsNotifying;
            LineIndicators_CircleDiameter.IsNotifying = false;
            var cis = new List<CircleIndicatorViewModel>();
            foreach (var result in CircleSearchingResults)
            {
                var ci = new CircleIndicatorViewModel()
                {
                    CenterX = result.Circle?.CenterX ?? 0,
                    CenterY = result.Circle?.CenterY ?? 0,
                    Radius = result.Circle?.Radius ?? 0,
                };
                if (result.IsNotFound)
                {
                    //                    ci.Stroke = Brushes.Red;}
                }

                cis.Add(ci);

                if (result.Definition.Diameter_DisplayEnabled)
                {
                    if (result.Circle == null) return;
                    var line = result.Circle.GetLine(45);

                    var lineIndicator = new LineIndicatorViewModel
                    {
                        StartPointX = line.X1,
                        StartPointY = line.Y1,
                        EndPointX = line.X2,
                        EndPointY = line.Y2,
                    };
                    LineIndicators_CircleDiameter.Add(lineIndicator);
                }
            }
            LineIndicators_CircleDiameter.IsNotifying = isNotifying;
            LineIndicators_CircleDiameter.Refresh();
            ResultsCircleIndicators.AddRange(cis);

            if (CircleSearchingResults.Count >= 2)
            {
                var Circle1CenterX = CircleSearchingResults[0].Circle.CenterX;
                var Circle1CenterY = CircleSearchingResults[0].Circle.CenterY;
                var vector1 = new Vector(Circle1CenterX, Circle1CenterY);


                var isNotifying2 = LineIndicators_CircleDiameter.IsNotifying;
                LineIndicators_CircleDiameter.IsNotifying = false;
                for (int i = 1; i < CircleSearchingResults.Count; i++)
                {
                    var Circle2CenterX = CircleSearchingResults[i].Circle.CenterX;
                    var Circle2CenterY = CircleSearchingResults[i].Circle.CenterY;

                    var vector2 = new Vector(Circle2CenterX, Circle2CenterY);

                    var diff = vector1 - vector2;
                    var distance = diff.Length;
                    var DistanceBetweenC1C2 = distance;


                    var distanceLineIndicator = new LineIndicatorViewModel
                    {
                        StartPointX = Circle1CenterX,
                        StartPointY = Circle1CenterY,
                        EndPointX = Circle2CenterX,
                        EndPointY = Circle2CenterY,
                    };
                    LineIndicators_CircleDiameter.Add(distanceLineIndicator);
                }
                LineIndicators_CircleDiameter.IsNotifying = isNotifying2;
                LineIndicators_CircleDiameter.Refresh();
            }
        }

        public void Show_CircleSearchingDefinitions(IEnumerable<CircleSearchingDefinition> circleSearchingDefinitions)
        {
            var cis = new List<CircleIndicatorViewModel>();
            foreach (var circleSearchingDefinition in circleSearchingDefinitions)
            {
                var innerCircle = new CircleIndicatorViewModel()
                {
                    CenterX = circleSearchingDefinition.CenterX,
                    CenterY = circleSearchingDefinition.CenterY,
                    Radius = circleSearchingDefinition.InnerRadius,
                };
                cis.Add(innerCircle);

                var outerCircle = new CircleIndicatorViewModel()
                {
                    CenterX = circleSearchingDefinition.CenterX,
                    CenterY = circleSearchingDefinition.CenterY,
                    Radius = circleSearchingDefinition.OuterRadius,
                };
                cis.Add(outerCircle);
            }
            DefinitionsCircleIndicators.AddRange(cis);
        }

        public void Show_EdgeSearchingDefinitions(IEnumerable<EdgeSearchingDefinition> edgeSearchingDefinitions)
        {
            List<RoiRectangleViewModel> roiRectangleViewModels = new List<RoiRectangleViewModel>();
            List<RegionIndicatorViewModel> regionIndicatorViewModels = new List<RegionIndicatorViewModel>();

            foreach (var ed in edgeSearchingDefinitions)
            {
                var regionIndicator = new RegionIndicatorViewModel
                {
                    StartPointX = ed.StartX,
                    StartPointY = ed.StartY,
                    EndPointX = ed.EndX,
                    EndPointY = ed.EndY,
                    RegionWidth = ed.ROIWidth,
                    Stroke = Brushes.Orange,
                    StrokeThickness = 4,
                    IsHidden = false,
                };
                regionIndicatorViewModels.Add(regionIndicator);

                var vm = new RoiRectangleViewModel()
                {
                    StartX = ed.StartX,
                    StartY = ed.StartY,
                    EndX = ed.EndX,
                    EndY = ed.EndY,
                    HalfWidth = ed.ROIWidth,
                };
                roiRectangleViewModels.Add(vm);
            }

            RegionIndicators_Edges.AddRange(regionIndicatorViewModels);
            RoiRectangleViewModels_CoordinateEdges.AddRange(roiRectangleViewModels);
        }

        public void Show_DataCodeSearchingDefinitions(IEnumerable<DataCodeSearchingDefinition> edgeSearchingDefinitions)
        {
            List<RoiRectangleViewModel> roiRectangleViewModels = new List<RoiRectangleViewModel>();

            foreach (var ed in edgeSearchingDefinitions)
            {
                var regionIndicator = new RegionIndicatorViewModel
                {
                    StartPointX = ed.StartX,
                    StartPointY = ed.StartY,
                    EndPointX = ed.EndX,
                    EndPointY = ed.EndY,
                    RegionWidth = ed.ROIHalfWidth,
                    Stroke = Brushes.Orange,
                    StrokeThickness = 4,
                    IsHidden = false,
                };
                RegionIndicators.Add(regionIndicator);

                var vm = new RoiRectangleViewModel()
                {
                    StartX = ed.StartX,
                    StartY = ed.StartY,
                    EndX = ed.EndX,
                    EndY = ed.EndY,
                    HalfWidth = ed.ROIHalfWidth,
                };
                roiRectangleViewModels.Add(vm);
            }

            RoiRectangleViewModels_DataCodeSearching.AddRange(roiRectangleViewModels);
        }

        public void Show_PartSearchingDefinitions(IEnumerable<PartSearchingDefinition> partSearchingDefinitions)
        {
            foreach (var ed in partSearchingDefinitions)
            {
                var regionIndicator = new RegionIndicatorViewModel
                {
                    StartPointX = ed.RoiLine.X1,
                    StartPointY = ed.RoiLine.Y1,
                    EndPointX = ed.RoiLine.X2,
                    EndPointY = ed.RoiLine.Y2,
                    RegionWidth = ed.RoiHalfWidth,
                    Stroke = Brushes.Orange,
                    StrokeThickness = 4,
                    IsHidden = false,
                };
                RegionIndicators.Add(regionIndicator);

                var regionIndicator2 = new RegionIndicatorViewModel
                {
                    StartPointX = ed.AreaLine.X1,
                    StartPointY = ed.AreaLine.Y1,
                    EndPointX = ed.AreaLine.X2,
                    EndPointY = ed.AreaLine.Y2,
                    RegionWidth = ed.AreaHalfWidth,
                    Stroke = Brushes.Orange,
                    StrokeThickness = 4,
                    IsHidden = false,
                };
                RegionIndicators.Add(regionIndicator2);
            }
        }

        public void Show_Edges(IEnumerable<EdgeSearchingResult> edgeSearchingResults)
        {
            List<LineIndicatorViewModel> lineIndicators = new List<LineIndicatorViewModel>();

            foreach (var edgeSearchingResult in edgeSearchingResults)
            {
                if (edgeSearchingResult.HasError) continue;

                var lineIndicator = new LineIndicatorViewModel
                {
                    StartPointX = edgeSearchingResult.EdgeLine.X1,
                    StartPointY = edgeSearchingResult.EdgeLine.Y1,
                    EndPointX = edgeSearchingResult.EdgeLine.X2,
                    EndPointY = edgeSearchingResult.EdgeLine.Y2,
                };
                lineIndicators.Add(lineIndicator);
            }

            LineIndicators_Edges.AddRange(lineIndicators);
        }

        public void Show_ReferenceLines(IEnumerable<LineDefinition> definitions)
        {
            List<LineIndicatorViewModel> lineIndicators = new List<LineIndicatorViewModel>();

            foreach (var edgeSearchingResult in definitions)
            {
                var lineIndicator = new LineIndicatorViewModel
                {
                    StartPointX = edgeSearchingResult.ActualLine.X1,
                    StartPointY = edgeSearchingResult.ActualLine.Y1,
                    EndPointX = edgeSearchingResult.ActualLine.X2,
                    EndPointY = edgeSearchingResult.ActualLine.Y2,
                };
                lineIndicators.Add(lineIndicator);
            }

            LineIndicators_ReferenceLines.AddRange(lineIndicators);
        }

        public void Show_ReferenceRadialLineDefinitions(IEnumerable<RadialLineDefinition> definitions)
        {
            List<LineIndicatorViewModel> _lineIndicators = new List<LineIndicatorViewModel>();

            foreach (var def in definitions)
            {
                var radiusVector = new Vector(def.ActualRadius, 0);
                var rotateVector = radiusVector.Rotate(def.Angle);
                var startVector = new Vector(def.ActualOriginX, def.ActualOriginY);
                var endVector = startVector + rotateVector;

                var lineIndicator = new LineIndicatorViewModel
                {
                    StartPointX = startVector.X,
                    StartPointY = startVector.Y,
                    EndPointX = endVector.X,
                    EndPointY = endVector.Y,
                };
                _lineIndicators.Add(lineIndicator);
            }

            LineIndicators_ReferenceLines.AddRange(_lineIndicators);
        }

        public void Show_CoordinateEdges(IEnumerable<EdgeSearchingResult> edgeSearchingResults)
        {
            List<LineIndicatorViewModel> _lineIndicators = new List<LineIndicatorViewModel>();

            foreach (var edgeSearchingResult in edgeSearchingResults)
            {
                if (edgeSearchingResult.HasError) continue;

                var lineIndicator = new LineIndicatorViewModel
                {
                    StartPointX = edgeSearchingResult.EdgeLine.X1,
                    StartPointY = edgeSearchingResult.EdgeLine.Y1,
                    EndPointX = edgeSearchingResult.EdgeLine.X2,
                    EndPointY = edgeSearchingResult.EdgeLine.Y2,
                };
                _lineIndicators.Add(lineIndicator);
            }

            LineIndicators_CoordinateEdges.AddRange(_lineIndicators);
        }

        public void Show_PartSearchingResults(IEnumerable<PartSearchingResult> partSearchingResults)
        {
            foreach (var edgeSearchingResult in partSearchingResults)
            {
                if (edgeSearchingResult.HasError) continue;

                var regionIndicator2 = new RegionIndicatorViewModel
                {
                    StartPointX = edgeSearchingResult.PartLine.X1,
                    StartPointY = edgeSearchingResult.PartLine.Y1,
                    EndPointX = edgeSearchingResult.PartLine.X2,
                    EndPointY = edgeSearchingResult.PartLine.Y2,
                    RegionWidth = edgeSearchingResult.PartHalfWidth,
                    Stroke = Brushes.Lime,
                    StrokeThickness = 2,
                    IsHidden = false,
                };
                RegionIndicators.Add(regionIndicator2);
            }
        }

        public void Show_RegionTargetDefinitions(IEnumerable<RegionTargetDefinition> definitions)
        {
            foreach (var ed in definitions)
            {
                var regionIndicator = new RegionIndicatorViewModel
                {
                    StartPointX = ed.RoiActualLine.X1,
                    StartPointY = ed.RoiActualLine.Y1,
                    EndPointX = ed.RoiActualLine.X2,
                    EndPointY = ed.RoiActualLine.Y2,
                    RegionWidth = ed.RoiHalfWidth,
                    Stroke = Brushes.Orange,
                    StrokeThickness = 4,
                    IsHidden = false,
                };
                RegionIndicators.Add(regionIndicator);
            }
        }

        public void Show_RegionTargetResults(IEnumerable<RegionTargetResult> results)
        {
            var isNotifying2 = LineIndicators_RegionTargetResults.IsNotifying;
            LineIndicators_RegionTargetResults.IsNotifying = false;
            foreach (var result in results)
            {
                if (result.HasError) continue;

                var rect2 = result.TargetRegion.GetSmallestHRectangle2();
                var roiRect = rect2.GetRoiRectangle();

                if (result.Definition.Rect2Len1Line_DisplayEnabled)
                {
                    var line = roiRect.GetWidthLine();

                    var lineIndicator = new LineIndicatorViewModel
                    {
                        StartPointX = line.X1,
                        StartPointY = line.Y1,
                        EndPointX = line.X2,
                        EndPointY = line.Y2,
                    };
                    LineIndicators_RegionTargetResults.Add(lineIndicator);
                }

                if (result.Definition.Rect2Len2Line_DisplayEnabled)
                {
                    var line = roiRect.GetLine();

                    var lineIndicator = new LineIndicatorViewModel
                    {
                        StartPointX = line.X1,
                        StartPointY = line.Y1,
                        EndPointX = line.X2,
                        EndPointY = line.Y2,
                    };
                    LineIndicators_RegionTargetResults.Add(lineIndicator);
                }

                var regionIndicator2 = new RegionIndicatorViewModel
                {
                    StartPointX = roiRect.StartX,
                    StartPointY = roiRect.StartY,
                    EndPointX = roiRect.EndX,
                    EndPointY = roiRect.EndY,
                    RegionWidth = roiRect.ROIWidth,
                    Stroke = Brushes.Lime,
                    StrokeThickness = 2,
                    IsHidden = false,
                };
                RegionIndicators.Add(regionIndicator2);

                var regionVm = new RegionViewModel
                {
                    Region = result.TargetRegion,
                };
                Regions.Add(regionVm);
            }
            LineIndicators_RegionTargetResults.IsNotifying = isNotifying2;
            LineIndicators_RegionTargetResults.Refresh();
        }

        public void Show_RegionSearchingResult(IEnumerable<RegionSearchingResult> results)
        {
            var regionIndicatorViewModels = new List<RegionIndicatorViewModel>();
            var regions = new List<RegionViewModel>();

            foreach (var result in results)
            {
                if (result.HasError) continue;

                var regionIndicator2 = new RegionIndicatorViewModel
                {
                    StartPointX = result.Definition.RoiActualLine.X1,
                    StartPointY = result.Definition.RoiActualLine.Y1,
                    EndPointX = result.Definition.RoiActualLine.X2,
                    EndPointY = result.Definition.RoiActualLine.Y2,
                    RegionWidth = result.Definition.RoiHalfWidth,
                    Stroke = Brushes.Lime,
                    StrokeThickness = 2,
                    IsHidden = false,
                };
                regionIndicatorViewModels.Add(regionIndicator2);

                var regionVm = new RegionViewModel
                {
                    Region = result.Region,
                };
                regions.Add(regionVm);
            }

            RegionIndicators.AddRange(regionIndicatorViewModels);
            Regions.AddRange(regions);
        }

        public void Show_DefectResults(IEnumerable<RegionDefectResult> regionDefectResults)
        {

            //
            /*            var surfaceDefectResults = inspectionResult.RegionDefectResults
                            .Where(x => x.DefectRegion != null)
                            .Select(x =>
                            {
                                var count2 = x.DefectRegion.CountObj();
                                var region = x.DefectRegion;
                                if (count2 > 1)
                                    region = region.Union1();

                                return new DefectViewModel(region);
                            })
                            .ToList();*/
            //
            var regions = inspectionResult.RegionDefectResults
                .Where(x => x.DefectRegion != null)
                .Select(x => x.DefectRegion)
                .ToList();

            var union = regions.Union();
            var dvm = new DefectViewModel(union);

            SurfaceDefects.Clear();
            SurfaceDefects.Add(dvm);
            //            SurfaceDefects.AddRange(surfaceDefectResults);

            //
            var highlightDefectViewModels = DefectResults
                .Where(x => x.DisplayHighlight)
                .Select(x => x.ToViewModel());

            HighlightDefects.Clear();
            HighlightDefects.AddRange(highlightDefectViewModels);
        }

        private void SaveImageButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                DefaultExt = ".tif",
                FileName = "ExportImage_" + DateTime.Now.ToString("yyyy-MM-dd__HH-mm-ss"),
                Filter = ".tif|TIFF"
            };
            var r = dialog.ShowDialog();
            if (r == true)
            {
                //                IndicatorViewer.BitmapSource.SaveToTiff(dialog.FileName);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            //            IndicatorViewer.Refresh();

            Refresh();
        }

        private void ZoomFitButton_OnClick(object sender, RoutedEventArgs e)
        {
            //            IndicatorViewer.ZoomFit();
            HalconViewer.ZoomFit();
        }

        private void ZoomActualButton_OnClick(object sender, RoutedEventArgs e)
        {
            //            IndicatorViewer.ZoomActual();
            HalconViewer.ZoomActual();
        }

        private void ZoomHalfButton_OnClick(object sender, RoutedEventArgs e)
        {
            //            IndicatorViewer.Scale = 0.5;
        }

        private void ZoomQuarterButton_OnClick(object sender, RoutedEventArgs e)
        {
            //            IndicatorViewer.Scale = 0.25;
        }

        private void ExportReportButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = "InspectionReports_" + DateTime.Now.ToString("_yyyy-MM-dd_HH.mm.ss");
            dialog.Filter = "*.csv|CSV";
            dialog.DefaultExt = ".csv";
            var ok = dialog.ShowDialog();
            if (ok != true)
                return;

            var fileName = dialog.FileName;
        }

        private void ZoomInButton_OnClick(object sender, RoutedEventArgs e)
        {
            //            IndicatorViewer.ZoomIn();
            HalconViewer.ZoomIn();
        }

        private void ZoomOutButton_OnClick(object sender, RoutedEventArgs e)
        {
            //            IndicatorViewer.ZoomOut();
            HalconViewer.ZoomOut();
        }

        private void ZoomActualToCenterButton_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void ClearWindowButton_OnClick(object sender, RoutedEventArgs e)
        {
            counter2 = 0;
            HalconViewer.ClearWindow();
        }

        public DelegateCommand OpenFileCommand { get; set; }

        public DelegateCommand<DefectResult> SelectDefectInfoCommand { get; set; }
        public DelegateCommand<string> SelectFileNameCommand { get; set; }

        private void OpenFileCommandButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileCommand.Execute();
        }

        private void ShowAllDefectsButton_OnClick(object sender, RoutedEventArgs e)
        {
            DefectsListView.SelectedItem = null;
            //            DefectIndicators.Clear();
            //            Defects.Clear();

            /*            var RectangleIndicatorViewModels = new List<RectangleIndicatorViewModel>();

                        foreach (var dr in DefectResults)
                        {
                            var diVM = dr.ToViewModel();
            //                diVM.Stroke = Brushes.Lime;
            //                diVM.StrokeThickness = 2;

                            RectangleIndicatorViewModels.Add(diVM);
                        }
            //            DefectIndicators.AddRange(RectangleIndicatorViewModels);*/

            Defects.Clear();
            var defectViewModels = DefectResults.Select(x => x.ToViewModel());
            Defects.AddRange(defectViewModels);
        }

        private void SwitchForwadAndBackwardButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (InspectionSchemaDir == "InspectionSchema_Forward")
            {
                InspectionSchemaDir = "InspectionSchema_Backward";
            }
            else if (InspectionSchemaDir == "InspectionSchema_Backward")
            {
                InspectionSchemaDir = "InspectionSchema_Forward";
            }
            InspectionSchemaDirTextBlock.Text = InspectionSchemaDir;
        }

        private int counter = 1;

        private void TestButton_OnClick(object sender, RoutedEventArgs e)
        {
            /*            HalconViewer.HWindowControl.HalconWindow.SetColor("blue");
                        HalconViewer.HWindowControl.HalconWindow.SetDraw("margin");
                        HalconViewer.HWindowControl.HalconWindow.SetLineWidth(10);

                        var line1 = new HRegion();
                        var line2 = new HRegion();

                        line1.GenRegionLine(100, 200, 3000, 4000.0);
                        line2.GenRegionLine(4000, 5000, 200, 500.0);

            //            HalconViewer.HWindowControl.HalconWindow.DispObj(line1);
            //            HalconViewer.HWindowControl.HalconWindow.DispObj(line2);

            //            HalconViewer.HWindowControl.HalconWindow.DispRegion(line1);
            //            HalconViewer.HWindowControl.HalconWindow.DispRegion(line2);

            //            HalconViewer.HWindowControl.HalconWindow.DispLine(200,100,400,300.0);
            //            HalconViewer.HWindowControl.HalconWindow.DispLine(5000, 4000, 500, 200.0);

            //            HalconViewer.HWindowControl.HalconWindow.DispLine(5000, 4000, 500, 200.0);

                        var rect = new HRegion();
                        rect.GenRectangle1(100*counter, 200*counter, 300*counter, 400.0*counter);

                        HalconViewer.HWindowControl.HalconWindow.DispObj(rect);
                        counter++;*/


            HalconViewer.ClearWindow();
            int count = 0;

            foreach (var regionDefectResult in InspectionController.InspectionResult.RegionDefectResults)
            {
                if (regionDefectResult.DefectRegion == null) continue;

                if (count == counter2)
                {
                    HalconViewer.HWindowControl.HalconWindow.SetColor("blue");
                    HalconViewer.HWindowControl.HalconWindow.SetDraw("margin");
                    HalconViewer.HWindowControl.HalconWindow.SetLineWidth(4);
                    HalconViewer.HWindowControl.HalconWindow.DispObj(regionDefectResult.DefectRegion);

                    counter2++;
                    break;
                }
                count++;
            }
        }

        private void Test2Button_OnClick(object sender, RoutedEventArgs e)
        {
            HalconViewer.ClearWindow();

            foreach (var regionDefectResult in InspectionController.InspectionResult.RegionDefectResults)
            {
                if (regionDefectResult.DefectRegion == null) continue;

                HalconViewer.HWindowControl.HalconWindow.SetColor("cyan");
                HalconViewer.HWindowControl.HalconWindow.SetDraw("margin");
                HalconViewer.HWindowControl.HalconWindow.SetLineWidth(4);
                HalconViewer.HWindowControl.HalconWindow.DispObj(regionDefectResult.DefectRegion);
            }
        }

        private int counter2 = 0;
        private HalconMvSchema _halconMvSchema;
        private static readonly string DefaultInspectionSchemaDir = "InspectionSchemas";

        private void TrainButton_OnClick(object sender, RoutedEventArgs e)
        {
            TrainSvmSample.TrainingSvmSample();
            MessageBox.Show("training finish");
        }
    }
}

// ReSharper restore InconsistentNaming