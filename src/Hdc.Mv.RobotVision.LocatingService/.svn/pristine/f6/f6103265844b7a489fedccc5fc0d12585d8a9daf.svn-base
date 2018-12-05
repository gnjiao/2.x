using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace Hdc.Mv.LocatingService
{
    public class TerminatorProtocolServer : AppServer
    {
        public TerminatorProtocolServer()
                        : base(new TerminatorReceiveFilterFactory("#"))
//            : base(new TerminatorReceiveFilterFactory("\r\n"))
        {
        }
    }
}