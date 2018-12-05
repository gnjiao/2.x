using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class CommonTcpSrvWorkpieceInPositionCommandController : ICommandController
    {
        public void Initialize()
        {
        }

        public void Command(MeasureEvent measureEvent)
        {
            CommonTcpSrvInitializer.Singleton.WorkInPositionCommand(measureEvent);
        }
        
    }
}
