using System.Collections.ObjectModel;
using Hdc.Measuring;
using Hdc.Measuring.VinsML;

namespace Vins.ML.MeasureStationService
{
    public class MeasureSchemaFactory
    {
        public static MeasureSchema GetLKMeasureSchema()
        {
            var measureSchema = new MeasureSchema
            {
                StationIndex = 2,
                Initializers = new Collection<IInitializer>()
                {
                    new AD7230Initializer(),
                    new OpcInitializer()
                    {
                        OpcXi_ServerUrl = "http://192.168.100.100/XiTOCO",
                        OpcXi_UserName = "hdc",
                        OpcXi_Password = "hdcrd.com",
                    },
                    new RobotResetCompleatedPluginInitializer(),
                },
                WorkpieceInPositionEventController =
                    new OpcBooleanEventController("WorkpieceLocatingReadyPlcEvent"),
                SensorInPositionEventController = new Ad7230MeasureRequestEventController(),
                StationCompletedCommandController =
                    new OpcCommandController("AllMeasureDataProcessedAppEvent"),
                SensorInPositionCommandController = new Ad7230MeasureCompletedForRobotCommandController(),
                WorkpieceTagService = new OpcWorkpieceTagService("TotalCount")
            };

            for (int i = 0; i < 40; i++)
            {
                var md = new MeasureDefinition
                {
                    Index = i,
                    DeviceName = "Point-" + i.ToString("00"),
                    Device = new LkMeasureDevice(),
                    Triggers = null,
                };
                measureSchema.MeasureDefinitions.Add(md);
            }

            for (int i = 0; i < 40/2; i++)
            {
                var mc = new CalculateDefinition()
                {
                    Index = i,
                    Name = "Step: " + i.ToString("D2") + " to" + (i + 1).ToString("D2"),
                    CalculateOperation = new StepCalculateOperation()
                    {
                        Operation1 = new GetMeasureOutputCalculateOperation() {MeasureResultIndex = i*2, MeasureOutputIndex = 0},
                        Operation2 = new GetMeasureOutputCalculateOperation() {MeasureResultIndex = i*2 + 1, MeasureOutputIndex = 0},
                    }
                };
                measureSchema.CalculateDefinitions.Add(mc);
            }

            return measureSchema;
        }
    }
}