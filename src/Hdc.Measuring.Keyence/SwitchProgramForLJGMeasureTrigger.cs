using System;
using Hdc.FA.KeyenceLasers.LJG;

namespace Hdc.Measuring
{
    [Serializable]
    public class SwitchProgramForLJGMeasureTrigger : IMeasureTrigger
    {
        private const int SUCCESS = 0;
        private bool _isUsed;

        public TriggerType TriggerType { get; set; }

        public void Action(IMeasureDevice measureDevice, int pointIndex, MeasureResult measureResult)
        {
            if (OnlyOnce && _isUsed) return;

            _isUsed = true;

            int rc = LJIF.LJIF_ChangeProgram((LJIF.LJIF_SETTINGTARGET) ProgramNo);

            if (rc == SUCCESS)
            {
            }
            else
            {
                throw new InvalidOperationException("LJIF_ChangeProgram terminated abnormally, ProgramNO: " + ProgramNo);
            }
        }

        public int ProgramNo { get; set; }

        public bool OnlyOnce { get; set; }
    }
}