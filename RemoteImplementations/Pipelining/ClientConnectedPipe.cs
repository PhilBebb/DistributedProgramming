// PhilBebb
using System;

namespace RemoteImplementations.Pipelining {
    public class ClientConnectedPipe : Pipe {
        
        public ClientConnectedPipe() {
        }

        //at some point this should do some logging
        public override void HandleRequest(System.Net.Sockets.TcpClient tcpClient) {
            base.HandleRequest(tcpClient);
        }
    }
}

