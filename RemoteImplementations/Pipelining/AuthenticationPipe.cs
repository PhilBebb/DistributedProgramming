// PhilBebb
using System;

namespace RemoteImplementations.Pipelining {
    public class AuthenticationPipe : Pipe {

        private static bool Authenticate(string message) {
            //super secure auth here!
            return message.ToUpperInvariant().Contains("OPEN");
        }

        public override void HandleRequest(System.Net.Sockets.TcpClient tcpClient) {

            bool handled = false;

            if(SuccessFullAuthPipe != null) {
                var message = Internal.InternalHelper.ReadJson(tcpClient.GetStream());

                if(Authenticate(message)) {
                    handled = true;

                    Internal.InternalHelper.SendMessage("success", tcpClient.GetStream());

                    FireEvents(tcpClient);
                    SuccessFullAuthPipe.HandleRequest(tcpClient);
                }
            }

            if(!handled) {
                Internal.InternalHelper.SendMessage("failed, send hello again", tcpClient.GetStream());               
                base.HandleRequest(tcpClient);
            }

        }

        public Pipe SuccessFullAuthPipe { get; set; }
    }
}

