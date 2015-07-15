// PhilBebb
using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RemoteImplementations.Networking {

    public delegate void ClientConnectedEventHandle(object sender, ClientConnectedEventArgs e);

    public class NetworkStateMachine {

        public ClientConnectedEventHandle ClientConnectedEvent;

        private TcpListener _listener = null;
        private IList<NetworkStateData> clientStates = new List<NetworkStateData>();
        private Thread _receiverThread = null;

        public NetworkStateMachine(TcpListener listener) {
            _listener = listener;
        }

        public void Start() {
            if(_receiverThread == null) {
                lock(this) {
                    if(_receiverThread == null) {
                        _receiverThread = new Thread(new ParameterizedThreadStart(ListenForClient));
                        _receiverThread.Start(true);
                    }
                }
            }
        }

        public void Stop() {
            if(_receiverThread != null) {
                lock(this) {
                    if(_receiverThread != null) {
                        _receiverThread.Abort();
                        _receiverThread = null;
                    }
                }
            }
        }

        private void ListenForClient(object o) {
            ListenForClient((bool)o);
        }

        private void ListenForClient(bool loopForever = false) {
            do {
                try {
                    var tcpClient = _listener.AcceptTcpClient();
                    Task.Factory.StartNew(() => StartConnection(tcpClient));

                } catch(Exception ex) {
                    Helpers.Helper.Log(string.Format("Exception : {0}", ex.Message));
                }
            } while (loopForever);
        }

        private void StartConnection(TcpClient tcpClient) {
            string clientInfo = tcpClient.Client.RemoteEndPoint.ToString();
            Helpers.Helper.Log(string.Format("Got connection request from {0}", clientInfo));

            if(!tcpClient.Connected) {
                return;
            }

            //if client is not in the processing state then start
            //else do what? possibly abort and maybe make it wait

            //get the current client state
            var state = clientStates.FirstOrDefault(cs => cs.Client.Equals(tcpClient));

            if(state != null) {
                //already in the state machine, not a new connection
                return;
            }

            state.NetworkMessageReceived += HandleConnection;
            state.Start();

        }

        private void HandleConnection(object sender, NetworkStateDataReceivedEventArgs message) {
            if(message == null || message.Client == null || string.IsNullOrWhiteSpace(message.Message)) {
                return;
            }
        
            switch(message.Client.State) {

                case NetworkStateEnum.Connecting:
                    HandleConnecting(message.Client);
                    break;
                
                case NetworkStateEnum.Connected:
                    HandleConnected(message.Client);
                    break;

                default:
                    //remove client?
                    HandleAborting(message.Client);
                    break;
            }

        }

        private void HandleConnecting(NetworkStateData client) {
            
        }

        private void HandleConnected(NetworkStateData client) {

            //should sne auth challenge now not accept, but for now lets just accept it

            string name = client.Client.Client.RemoteEndPoint.ToString(); //needs more client
            RemoteNetworkedClient remote = new RemoteNetworkedClient(client.Client, name);
            if(ClientConnectedEvent != null) {
                ClientConnectedEvent(this, new ClientConnectedEventArgs(remote));
            }
        }

        private void HandleAborting(NetworkStateData client) {
            client.Abort();
        }
    }

    public class ClientConnectedEventArgs : EventArgs {

        public ClientConnectedEventArgs(RemoteNetworkedClient client) {
            Client = client;
        }

        public RemoteNetworkedClient Client { get; private set; }
    }
}