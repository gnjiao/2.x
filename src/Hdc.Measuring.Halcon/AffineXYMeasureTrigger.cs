using System;
using System.IO;
using System.Windows;
using Hdc.Mv.Halcon;
using Hdc.Mv.RobotVision;
using Hdc.Reflection;
using Hdc.Serialization;

namespace Hdc.Measuring
{
    [Serializable]
    public class AffineXYMeasureTrigger: IMeasureTrigger
    {
        private HandEyeCalibrationSchema _calibrationSchema;
        private HHomConverter _converter;

        public TriggerType TriggerType { get; set; }

        public AffineXYMeasureTrigger()
        {
        }

        public void Action(IMeasureDevice measureDevice, int pointIndex, MeasureResult measureResult)
        {

            var dir = typeof(AffineXYMeasureTrigger).Assembly.GetAssemblyDirectoryPath();
            var calibrationSchemaFileName = Path.Combine(dir, CalibrationSchemaFileName);
            _calibrationSchema = calibrationSchemaFileName.DeserializeFromXamlFile<HandEyeCalibrationSchema>();

            // display CalibrationSchema
            Console.WriteLine($"--CalibrationSchema.CalibToolInBaseVector = {_calibrationSchema.CalibToolInBaseVector}");
            Console.WriteLine($"--CalibrationSchema.OriToolInBaseVector = {_calibrationSchema.OriToolInBaseVector}");
            Console.WriteLine($"--CalibrationSchema.RefToolInBaseVector = {_calibrationSchema.RefToolInBaseVector}");
            Console.WriteLine($"--CalibrationSchema.OriInCamVector = {_calibrationSchema.OriInCamVector}");
            Console.WriteLine($"--CalibrationSchema.RefInCamVector = {_calibrationSchema.RefInCamVector}");
            Console.WriteLine($"--CalibrationSchema.OriInBaseVector = {_calibrationSchema.OriInBaseVector}");
            Console.WriteLine($"--CalibrationSchema.RefInBaseVector = {_calibrationSchema.RefInBaseVector}");
            Console.WriteLine($"--CalibrationSchema.CenterInBaseVector = {_calibrationSchema.CenterInBaseVector}");

            //
            _converter = new HHomConverter(_calibrationSchema);

            var x = measureResult.Outputs[MeasureOutputIndexOfObjInCamX].Value;
            var y = measureResult.Outputs[MeasureOutputIndexOfObjInCamY].Value;
            var targetOriInCamVector = new Vector(x,y);

            var targetOriInBase = _converter.ConvertToBase(targetOriInCamVector,
                _calibrationSchema.OriToolInBaseVector);

            Console.WriteLine($">>targetOriInBase = {targetOriInBase}");

            measureResult.Outputs.Add(new MeasureOutput()
            {
                Value = targetOriInBase.X,
                Message = "targetOriInBaseX",
            });

            measureResult.Outputs.Add(new MeasureOutput()
            {
                Value = targetOriInBase.Y,
                Message = "targetOriInBaseY",
            });
        }

        public int MeasureOutputIndexOfObjInCamX { get; set; }

        public int MeasureOutputIndexOfObjInCamY { get; set; }

        public string CalibrationSchemaFileName { get; set; }
    }
}