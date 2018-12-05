using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class EsponTcpSrvWorkpieceInPositionCommandController : ICommandController
    {
        public void Initialize()
        {
        }

        public void Command(MeasureEvent measureEvent)
        {
            EsponTcpSrvInitializer.Singleton.WorkInPositionCommand(measureEvent);
        }
        
    }
}
