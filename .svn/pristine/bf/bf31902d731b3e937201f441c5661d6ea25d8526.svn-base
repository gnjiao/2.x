using System;
using System.Linq;
using Hdc.Mv.RobotVision;
using RCAPINet;
using Hdc.Measuring;

namespace Hdc.Measuring
{
    [Serializable]
    public class XySetVarFromMeasureOutputUsingRcApiMeasureTrigger : IMeasureTrigger
    {
        public TriggerType TriggerType { get; set; }

        public void Action(IMeasureDevice measureDevice, int pointIndex, MeasureResult measureResult)
        {
            var xOutput = measureResult.Outputs[XMeasureOutputIndex];
            var yOutput = measureResult.Outputs[YMeasureOutputIndex];

            var robotPointSchema = XyRcApiInitializer.Singleton.RobotPointSchemas
                .Single(x=>x.StationIndex == RobotPointSchemaStationIndex);

            foreach (var robotPoint in robotPointSchema.RobotPoints)
            {
                if (robotPoint.Index == RobotPointIndex)
                {
                    robotPoint.BaseX = xOutput.Value + XOffSet;
                    robotPoint.BaseY = yOutput.Value + YOffSet;
                    robotPoint.BaseZ = 0 + ZOffSet;
                }
            }
        }

        public int XMeasureOutputIndex { get; set; }
        public int YMeasureOutputIndex { get; set; }

        public int RobotPointSchemaStationIndex { get; set; }
        public int RobotPointIndex { get; set; }

        public float XOffSet { get; set; }
        public float YOffSet { get; set; }
        public float ZOffSet { get; set; } 
    }
}