using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class XyRcApiWorkpieceInPositionCommandController: ICommandController
    {
        public void Initialize()
        {
        }

        public void Command(MeasureEvent measureEvent)
        {
            XyRcApiInitializer.Singleton.WorkInPositionCommand(StationIndex);
        }

        public int StationIndex { get; set; }
    }
}