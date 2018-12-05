using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class SetVarFromMeasureOutputUsingRcApiMeasureTrigger : IMeasureTrigger
    {
        public TriggerType TriggerType { get; set; }

        public void Action(IMeasureDevice measureDevice, int pointIndex, MeasureResult measureResult)
        {
            var output = measureResult.Outputs[MeasureOutputIndex];

            RcApiService.Singletone.SetVar(VarName, output.Value);
        }

        public string VarName { get; set; }

        public int MeasureOutputIndex { get; set; }
    }
}