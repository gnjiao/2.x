using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Hdc.Mv.Inspection.Halcon.SampleApp
{
    public static class Ex
    {
        public static IList<MeasurementInfoViewModel> GetMeasurementInfos(this
            IEnumerable<DistanceBetweenPointsResult> results, IRelativeCoordinate relativeCoordinate)
        {
            List<MeasurementInfoViewModel> viewModels = new List<MeasurementInfoViewModel>();

            var resultList = results.ToList();

            for (int i = 0; i < resultList.Count; i++)
            {
                var result = resultList[i];
                if (result.HasError)
                {
                    var measurement2 = new MeasurementInfoViewModel {HasError = true};
                    viewModels.Add(measurement2);
                    continue;
                }
                ;

                var line = new Line(result.Point1, result.Point2);
                var measurement = line.GetMeasurementInfo(relativeCoordinate,
                    result.Definition.PixelCellSideLengthInMillimeter);
                measurement.Index = i;
                measurement.GroupName = result.Definition.GroupName;
                measurement.TypeCode = 100 + i;
                measurement.Name = result.Definition.Name;
                measurement.DisplayName = result.Definition.DisplayName;
                measurement.ExpectValue = result.Definition.ExpectValue;
                viewModels.Add(measurement);
            }

            return viewModels;
        }

        public static IList<MeasurementInfoViewModel> GetMeasurementInfos(this
            CircleSearchingResultCollection results, IRelativeCoordinate relativeCoordinate)
        {
            List<MeasurementInfoViewModel> viewModels = new List<MeasurementInfoViewModel>();

            var resultList = results.ToList();

            for (int i = 0; i < resultList.Count; i++)
            {
                var result = resultList[i];
                if (result.HasError)
                {
                    var measurement2 = new MeasurementInfoViewModel {HasError = true};
                    viewModels.Add(measurement2);
                    continue;
                }
                ;

                viewModels.Add(new MeasurementInfoViewModel
                {
                    Index = i,
                    GroupName = result.Definition.GroupName,
                    TypeCode = 100 + i,
                    Name = result.Definition.Name + "-DiameterInWorld",
                    DisplayName = result.Definition.DisplayName,
                    ExpectValue = result.Definition.Diameter_ExpectValue,
                    Value = result.Diameter,
                    ValueActualValue = result.DiameterInWorld,
                });

                viewModels.Add(new MeasurementInfoViewModel
                {
                    Index = i,
                    GroupName = result.Definition.GroupName,
                    TypeCode = 100 + i,
                    Name = result.Definition.Name + "-RelativeCenterXInWorld",
                    DisplayName = result.Definition.DisplayName,
                    ExpectValue = result.Definition.BaselineX,
                    Value = result.RelativeCenterX,
                    ValueActualValue = result.RelativeCenterXInWorld,
                });

                viewModels.Add(new MeasurementInfoViewModel
                {
                    Index = i,
                    GroupName = result.Definition.GroupName,
                    TypeCode = 100 + i,
                    Name = result.Definition.Name + "-RelativeCenterYInWorld",
                    DisplayName = result.Definition.DisplayName,
                    ExpectValue = result.Definition.BaselineY,
                    Value = result.RelativeCenterY,
                    ValueActualValue = result.RelativeCenterYInWorld,
                });
            }

            return viewModels;
        }


        public static MeasurementInfoViewModel GetMeasurementInfo(this Line line, IRelativeCoordinate coordinate,
            double pixelCellSideLengthInMillimeter)
        {
            Vector relativeP1 = coordinate.GetRelativeVector(line.GetPoint1().ToVector());
            Vector relativeP2 = coordinate.GetRelativeVector(line.GetPoint2().ToVector());

            var measurement = new MeasurementInfoViewModel
            {
                StartPointX = line.GetPoint1().X,
                StartPointY = line.GetPoint1().Y,
                EndPointX = line.GetPoint2().X,
                EndPointY = line.GetPoint2().Y,
                Value = line.GetLength(),
                ValueActualValue = line.GetLength().ToMillimeterFromPixel(pixelCellSideLengthInMillimeter),
                StartPointXActualValue = relativeP1.X.ToMillimeterFromPixel(pixelCellSideLengthInMillimeter),
                StartPointYActualValue = relativeP1.Y.ToMillimeterFromPixel(pixelCellSideLengthInMillimeter),
                EndPointXActualValue = relativeP2.X.ToMillimeterFromPixel(pixelCellSideLengthInMillimeter),
                EndPointYActualValue = relativeP2.Y.ToMillimeterFromPixel(pixelCellSideLengthInMillimeter),
            };

            return measurement;
        }


        public static IList<MeasurementInfoViewModel> GetMeasurementInfos(
            this IEnumerable<DataCodeSearchingResult> results,
            IRelativeCoordinate coordinate)
        {
            List<MeasurementInfoViewModel> measurementInfoViewModels = new List<MeasurementInfoViewModel>();

            foreach (var result in results)
            {
                var info = result.GetMeasurementInfo(coordinate);
                measurementInfoViewModels.Add(info);
            }

            return measurementInfoViewModels;
        }

        public static MeasurementInfoViewModel GetMeasurementInfo(this DataCodeSearchingResult result,
            IRelativeCoordinate coordinate)
        {
            var measurement = new MeasurementInfoViewModel
            {
                DisplayName = result.DecodeString,
                //                Value = result.DecodeString,
                //                ValueActualValue = result.GetLength().ToMillimeterFromPixel(pixelCellSideLengthInMillimeter),
                //                StartPointXActualValue = relativeP1.X.ToMillimeterFromPixel(pixelCellSideLengthInMillimeter),
                //                StartPointYActualValue = relativeP1.Y.ToMillimeterFromPixel(pixelCellSideLengthInMillimeter),
                //                EndPointXActualValue = relativeP2.X.ToMillimeterFromPixel(pixelCellSideLengthInMillimeter),
                //                EndPointYActualValue = relativeP2.Y.ToMillimeterFromPixel(pixelCellSideLengthInMillimeter),
            };

            return measurement;
        }


        public static IList<MeasurementInfoViewModel> GetMeasurementInfos(
            this IEnumerable<PointOfXldAndRadialLineResult> results,
            IRelativeCoordinate coordinate)
        {
            List<MeasurementInfoViewModel> measurementInfoViewModels = new List<MeasurementInfoViewModel>();

            foreach (var result in results)
            {
                var info = result.GetMeasurementInfo(coordinate);
                measurementInfoViewModels.Add(info);
            }

            return measurementInfoViewModels;
        }

        public static IList<MeasurementInfoViewModel> GetMeasurementInfos(
            this IEnumerable<DistanceBetweenPointsOfXldAndRadialLineResult> results,
            IRelativeCoordinate coordinate)
        {
            List<MeasurementInfoViewModel> measurementInfoViewModels = new List<MeasurementInfoViewModel>();

            foreach (var result in results)
            {
                var info = result.GetMeasurementInfo(coordinate);
                measurementInfoViewModels.Add(info);
            }

            return measurementInfoViewModels;
        }

        public static MeasurementInfoViewModel GetMeasurementInfo(this PointOfXldAndRadialLineResult result,
            IRelativeCoordinate coordinate)
        {
            var measurement = new MeasurementInfoViewModel
            {
                GroupName = result.Definition.GroupName,
                DisplayName = result.Definition.DisplayName,
                Value = result.DistanceInWorld,
                ValueActualValue = result.DistanceInWorld,
                StartPointXActualValue = result.ActualOriginX,
                StartPointYActualValue = result.ActualOriginY,
                EndPointXActualValue = result.X,
                EndPointYActualValue = result.Y,
                ExpectValue = result.Definition.StandardValue,
                
            };

            return measurement;
        }

        public static MeasurementInfoViewModel GetMeasurementInfo(this DistanceBetweenPointsOfXldAndRadialLineResult result,
            IRelativeCoordinate coordinate)
        {
            var measurement = new MeasurementInfoViewModel
            {
                GroupName = result.Definition.GroupName,
                DisplayName = result.Definition.DisplayName,
                Value = result.DistanceInWorld,
                ValueActualValue = result.DistanceInWorld,
                StartPointXActualValue = result.X1,
                StartPointYActualValue = result.Y1,
                EndPointXActualValue = result.X2,
                EndPointYActualValue = result.Y2,
                ExpectValue = result.Definition.StandardValue,
                
            };

            return measurement;
        }

        public static IList<MeasurementInfoViewModel> GetMeasurementInfos(
            this IEnumerable<DistanceBetweenTwoPointsResult> results,
            IRelativeCoordinate coordinate)
        {
            List<MeasurementInfoViewModel> measurementInfoViewModels = new List<MeasurementInfoViewModel>();

            foreach (var result in results)
            {
                measurementInfoViewModels.Add(new MeasurementInfoViewModel
                {
                    GroupName = result.Definition.GroupName,
                    DisplayName = result.Definition.DisplayName + "[Distance]",
                    Value = result.Distance,
                    ValueActualValue = result.DistanceInWorld,
                    ExpectValue = result.Definition.StandardDistance,
                    StartPointXActualValue = result.StartPointXPath,
                    StartPointYActualValue = result.StartPointYPath,
                    EndPointXActualValue = result.EndPointXPath,
                    EndPointYActualValue = result.EndPointYPath
                });

                measurementInfoViewModels.Add(new MeasurementInfoViewModel
                {
                    GroupName = result.Definition.GroupName,
                    DisplayName = result.Definition.DisplayName + "[DistanceInXAxis]",
                    Value = result.DistanceInXAxis,
                    ValueActualValue = result.DistanceInXAxisInWorld,
                    ExpectValue = result.Definition.StandardDistanceInXAxis,
                    StartPointXActualValue = result.StartPointXPath,
                    StartPointYActualValue = result.StartPointYPath,
                    EndPointXActualValue = result.EndPointXPath,
                    EndPointYActualValue = result.EndPointYPath
                });

                measurementInfoViewModels.Add(new MeasurementInfoViewModel
                {
                    GroupName = result.Definition.GroupName,
                    DisplayName = result.Definition.DisplayName + "[DistanceInYAxis]",
                    Value = result.DistanceInYAxis,
                    ValueActualValue = result.DistanceInYAxisInWorld,
                    ExpectValue = result.Definition.StandardDistanceInYAxis,
                    StartPointXActualValue = result.StartPointXPath,
                    StartPointYActualValue = result.StartPointYPath,
                    EndPointXActualValue = result.EndPointXPath,
                    EndPointYActualValue = result.EndPointYPath
                });
            }

            return measurementInfoViewModels;
        }

        public static IList<MeasurementInfoViewModel> GetMeasurementInfos(
           this IEnumerable<StepFromGrayValueResult> results,
           IRelativeCoordinate coordinate)
        {
            List<MeasurementInfoViewModel> measurementInfoViewModels = new List<MeasurementInfoViewModel>();

            foreach (var result in results)
            {
                var info = result.GetMeasurementInfo(coordinate);
                measurementInfoViewModels.Add(info);
            }

            return measurementInfoViewModels;
        }

        public static MeasurementInfoViewModel GetMeasurementInfo(this StepFromGrayValueResult result,
            IRelativeCoordinate coordinate)
        {
            var measurement = new MeasurementInfoViewModel
            {
                GroupName = result.Definition.GroupName,
                DisplayName = result.Definition.DisplayName,
                Value = result.StepValueInGrayValue,
                ValueActualValue = result.StepValueInGrayValue,
                ExpectValue = result.Definition.StandardValue
            };

            return measurement;
        }
    }
}