using System;
using System.Collections.Generic;
using Hdc.FA.KeyenceLasers.LJG;

namespace Hdc.Measuring
{
    [Serializable]
    public class LJGStartStorageCommandController : ICommandController
    {
        public void Initialize()
        {
        }

        public void Command(MeasureEvent measureEvent)
        {
            int ret = -1;

            ret = LJIF.LJIF_StopStorage();
//            if (ret != 0)
//                throw new InvalidOperationException("LJGStartStorageCommandController.LJIF_StopStorage error");

            ret = LJIF.LJIF_ClearStorageData();
//            if (ret != 0)
//                throw new InvalidOperationException("LJGStartStorageCommandController.LJIF_ClearStorageData error");

            ret = LJIF.LJIF_StartStorage();
            if (ret != 0)
                throw new InvalidOperationException("LJGStartStorageCommandController.LJIF_StartStorage error");
        }

    }
}