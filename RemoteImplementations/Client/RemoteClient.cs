using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Interfaces.Shared;
using SimpleImplementations;

namespace RemoteImplementations {

    public delegate void RequestReceivedEventHandle(object sender, RequestReceivedEventArgs e);
    public delegate void RequestProcessedEventHandle(object sender, RequestProcessedEventArgs e);

    public class RemoteClient {
        private TcpClient _client = null;
        private Thread _receiverThread = null;

        private void StartListenerThread() {
            if(_receiverThread == null) {
                lock(this) {
                    if(_receiverThread == null) {
                        _receiverThread = new Thread(new ThreadStart(ListenForServer));
                        _receiverThread.Start();
                    }
                }
            }
        }

        private void StopListenerThread() {
            if(_receiverThread != null) {
                lock(this) {
                    if(_receiverThread != null) {
                        _receiverThread.Abort();
                        _receiverThread = null;
                    }
                }
            }
        }

        public RemoteClient() {
            _client = new TcpClient();
        }

        public void Start(IPAddress address, int port) {
            Console.WriteLine("Starting");
            _client.Connect(address, port);
            Console.WriteLine("Connected");

            SendHelloAsync();

            ListenForServer();
        }

        public void StartThreaded(IPAddress address, int port) {
            Console.WriteLine("Starting");
            _client.ConnectAsync(address, port);
            Console.WriteLine("Connected");

            SendHelloAsync();

            StopListenerThread();
            StartListenerThread();

        }

        public void Stop() {
            try {
                _client.Close();
            } catch(Exception ex) {
                throw ex;
            }

            StopListenerThread();

        }

        public bool IsRunningThreadded() {
            if(_receiverThread != null) {
                lock(this) {
                    if(_receiverThread != null) {
                        return _receiverThread.ThreadState == System.Threading.ThreadState.Running;
                    }
                }
            }
            return false;
        }

        private async void ListenForServer() {
            while (true) {
                Console.WriteLine("Waiting for job");
                var job = await InternalHelper.ReadJobAsJsonAsync(_client.GetStream());
                if(RequestReceived != null) {
                    RequestReceived(this, new RequestReceivedEventArgs { Request = job });
                }
                Console.WriteLine("job get!");

                IResult result = new SimpleResult(
                                     true,
                                     new Dictionary<string, string> { { "p1", "a" }, { "p2", "b" } },
                                     job
                                 );

                Console.WriteLine("Sending result");
                InternalHelper.SendJson(Helpers.Helper.ResultToJson(result), _client.GetStream());
                if(RequestProcessed != null) {
                    RequestProcessed(this, new RequestProcessedEventArgs { Result = result });
                }
                Console.WriteLine("Result sent");
            }
        }

        private async Task<bool> SendHelloAsync() {
            return true;
        }

        public event RequestReceivedEventHandle RequestReceived;
        public event RequestProcessedEventHandle RequestProcessed;
    }

    public class RequestReceivedEventArgs : EventArgs {
        public IJob Request { get; set; }
    }

    public class RequestProcessedEventArgs : EventArgs {
        public IResult Result { get; set; }
    }

}
