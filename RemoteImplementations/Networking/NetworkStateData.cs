//// PhilBebb
//using System;
//using System.Net.Sockets;
//using System.Threading;
//using System.IO;
//
//namespace RemoteImplementations.Networking {
//    public delegate void NetworkMessageReceivedEventHandle(object sender, NetworkStateDataReceivedEventArgs e);
//    public delegate void HeartbeatMessageReceivedEventHandle(object sender, HeartbeatReceivedEventArgs e);
//
//    public class NetworkStateData {
//        
//        private const string HeartbeatString = "heartbeat";
//
//        private static bool IsHeartbeat(string message) {
//            return string.Equals(HeartbeatString, message);
//        }
//
//        public NetworkMessageReceivedEventHandle NetworkMessageReceived;
//        public HeartbeatMessageReceivedEventHandle HeartbeatReceived;
//
//        private Thread _listenerThead = null;
//        private DateTime _lastHeartbeatTime = DateTime.MinValue;
//
//        public NetworkStateData(TcpClient tcpClient) {
//            Client = tcpClient;
//        }
//
//        public TcpClient Client {
//            get;
//            private set;
//        }
//
//        public NetworkStateEnum State {
//            get;
//            set;
//        }
//
//        public DateTime LastHeatbeatTime {
//            get { return _lastHeartbeatTime; }
//        }
//
//        private void RecordHeartbeat() {
//            _lastHeartbeatTime = DateTime.UtcNow;
//            if(HeartbeatReceived != null) {
//                HeartbeatReceived(this, new HeartbeatReceivedEventArgs(this, _lastHeartbeatTime));
//            }
//        }
//
//        public bool HasHeartbeatedSince(DateTime since) {
//            return _lastHeartbeatTime > since;
//        }
//
//        public bool HasHeartbeatedIn(TimeSpan time) {
//            return  _lastHeartbeatTime > DateTime.UtcNow.Add(time.Add(time.Negate()));
//        }
//
//        public void Start() {
//
//            State = NetworkStateEnum.Connecting;
//
//            if(_listenerThead != null) {
//                if(_listenerThead.IsAlive) {
//                    _listenerThead.Abort();
//                }
//            }
//
//            _listenerThead = new Thread(new System.Threading.ThreadStart(ThreadStart));
//            _listenerThead.Start();
//        }
//
//        private void ThreadStart() {
//            while (Client.Connected) {
//                string data = string.Empty;
//
//                try {
//                    using(var reader = new StreamReader(Client.GetStream())) {
//                        data = reader.ReadToEnd();
//
//                        if(IsHeartbeat(data)) {
//                            RecordHeartbeat();
//                            continue;
//                        }
//
//                        if(NetworkMessageReceived != null) {
//                            NetworkMessageReceived(this, new NetworkStateDataReceivedEventArgs(this, data));                        
//                        }    
//
//                    }
//                } catch(Exception ex) {
//                    Helpers.Helper.Log(ex);
//                }
//            }
//        }
//
//        public void SendToClient(string message) {
//            try {
//                using(var writer = new StreamWriter(Client.GetStream())) {
//                    writer.Write(message);
//                    writer.Flush();
//                }
//                    
//            } catch(Exception ex) {
//                Helpers.Helper.Log(ex.Message);
//            } 
//
//        }
//
//        public void Abort() {
//            try {
//                Client.Close();
//            } catch(Exception ex) {
//                Helpers.Helper.Log(ex.Message);
//            }
//        }
//    }
//
//    public class NetworkStateDataReceivedEventArgs : EventArgs {
//
//        public NetworkStateDataReceivedEventArgs(NetworkStateData client, string message) {
//            Client = Client;
//            Message = Message;
//        }
//
//        public string Message {
//            get;
//            private set;
//        }
//
//        public NetworkStateData Client {
//            get;
//            private set;
//        }
//    }
//
//    public class HeartbeatReceivedEventArgs : EventArgs {
//        public HeartbeatReceivedEventArgs(NetworkStateData client) : this(client, DateTime.UtcNow) {
//        }
//
//        public HeartbeatReceivedEventArgs(NetworkStateData client, DateTime time) {
//            Client = client;
//            Time = time;
//        }
//
//        public DateTime Time {
//            get;
//            private set;
//        }
//
//        public NetworkStateData Client {
//            get;
//            private set;
//        }
//    }
//}
//
