// PhilBebb
using System;

namespace RemoteImplementations.Pipelining {
    public class ClientConnectingPipe : Pipe {

        public ClientConnectingPipe() {
        }

        private string HelloMessage {
            get { return string.Format("Hello Client, the time is {0}\nPlease Auth yourself", DateTime.UtcNow.ToString()); }
        }

        /// <summary>
        /// Sends a hello to the client
        /// </summary>
        /// <param name="tcpClient">Tcp client.</param>
        public override void HandleRequest(System.Net.Sockets.TcpClient tcpClient) {
            Internal.InternalHelper.SendMessage(HelloMessage, tcpClient.GetStream());
            base.HandleRequest(tcpClient);
        }
    }
}

