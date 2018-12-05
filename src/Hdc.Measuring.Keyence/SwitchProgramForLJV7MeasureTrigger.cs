using System;
using Hdc.FA.KeyenceLasers.LJV7;

namespace Hdc.Measuring
{
    [Serializable]
    public class SwitchProgramForLJV7MeasureTrigger : IMeasureTrigger
    {
        public int TargetIndex { get; set; }
        public TriggerType TriggerType { get; set; }

        public void Action(IMeasureDevice measureDevice, int pointIndex, MeasureResult measureResult)
        {
            var ret = NativeMethods.LJV7IF_ChangeActiveProgram(DeviceId, (byte) ProgramNo);
            if (ret != 0)
                throw new InvalidOperationException("LJV7IF_ChangeActiveProgram error, ProgramNO: " + ProgramNo);
        }

        public int DeviceId { get; set; } = 0;
        public int ProgramNo { get; set; }
    }
}