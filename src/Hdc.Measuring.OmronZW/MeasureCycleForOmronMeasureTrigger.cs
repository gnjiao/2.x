using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omron.Cxap.Modules.DisplacementSensorSDK;

namespace Hdc.Measuring.OmronZW
{
    [Serializable]
    public class MeasureCycleForOmronMeasureTrigger : IMeasureTrigger
    {
        private const int SUCCESS = 0;
        public const int UNIT_NO_PRESERVE = 20;
        public const int DATA_NO_PRESERVE = 0;

        public TriggerType TriggerType { get; set; }

        public void Action(IMeasureDevice measureDevice, int pointIndex, MeasureResult measureResult)
        {
            int rc = OmronZw2MeasureDevice.DsComm.SetBankData(UNIT_NO_PRESERVE, DATA_NO_PRESERVE, this.MeasureCycle);

            if (rc == SUCCESS)
            {
            }
            else
            {
                throw new InvalidOperationException("Omron MeasureCycle is : " + MeasureCycle);
            }
        }

        public int MeasureCycle { get; set; }
    }

}
