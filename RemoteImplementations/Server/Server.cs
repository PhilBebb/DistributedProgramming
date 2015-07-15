using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.Server;
using Interfaces.Shared;
using RemoteImplementations.Networking;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RemoteImplementations {
    /// <summary>
    /// This needs a way better name
    /// </summary>
    public class Server : IServer {
        private TcpListener _listener;

        private object _lock = new object();
        private List<IClient> _clients = new List<IClient>();
        private NetworkStateMachine _stateMachine = null;

        public Server(int port, IClientSelection clientSelection) {
            _stateMachine = new NetworkStateMachine(new TcpListener(new IPEndPoint(IPAddress.Any, port)));
            _stateMachine.ClientConnectedEvent += ClientConnected;

            ClientSelectionMethod = clientSelection;
        }

        public void Start() {
            Helpers.Helper.Log("Starting...");
            _stateMachine.Start();
            Helpers.Helper.Log("Started");
        }

        public void Stop() {
            _listener.Stop();
            try {
                _listener.Server.Dispose();
            } catch(Exception ex) {
                Helpers.Helper.Log(ex.Message);
                throw ex;
            }
        }

        public IEnumerable<IClient> GetConnectedClients() {
            return _clients.AsParallel().Where(c => c.Ping());
        }

        //		public Task<IEnumerable<IClient>> GetConnectedClientsAsync ()
        //		{
        //			throw new NotImplementedException ();
        //		}

        public bool AddClient(IClient client) {
            lock(_lock) {
                if(!_clients.Contains(client)) {
                    _clients.Add(client);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveClient(IClient client) {
            lock(_lock) {
                if(_clients.Contains(client)) {
                    _clients.Remove(client);
                    return true;
                }
            }
            return false;
        }

        public bool IsClientConnected(IClient client) {
            return client.Ping();
        }

        //		public Task<bool> IsClientConnectedAsync (IClient client)
        //		{
        //			throw new NotImplementedException ();
        //		}

        public IServerResult RubJob(IJob job) {
            IEnumerable<IClient> clients = ClientSelectionMethod.GetBestClients(job, this);

            //make tasks for all of them
            IDictionary<IClient, Task<IResult>> clientRunTasks;

            clientRunTasks = clients.ToDictionary(c => c, c => new Task<IResult>(() => c.RunJob(job)));

            //start the timer, run the tasks, wait till time is done
            var clientExecutionTime = Stopwatch.StartNew();

            foreach(var client in clientRunTasks.Values.AsParallel ()) {
                client.Start();
            }

            try {
                Task.WaitAll(clientRunTasks.Values.ToArray());
            } catch(AggregateException ex) {
                foreach(var e in ex.InnerExceptions) {
                    //?
                }
            }

            clientExecutionTime.Stop(); //stop the time now it's done

            TimeSpan timeTaken = clientExecutionTime.Elapsed;

            IDictionary<IClient, IResult> clientResults = new Dictionary<IClient, IResult>();
            foreach(var client in clientRunTasks.Keys) {

                try {
                    clientResults.Add(client, clientRunTasks[client].Result);
                } catch(AggregateException ex) {

                    var resultsDictionary = new Dictionary<string, string>() {
                        { "Error", ex.Message }
                    };

                    foreach(Exception e in ex.InnerExceptions) {
                        resultsDictionary.Add(e.Message, e.StackTrace);
                    }

                    clientResults.Add(client, new SimpleImplementations.SimpleResult(
                        false,
                        resultsDictionary
                        , job));
                }
            }

            IServerResult result = new SimpleImplementations.SimpleServerResult(
                                       timeTaken,
                                       clientResults,
                                       job);

            return result;
        }

        //		public Task<IServerResult> RubJobAsync (IJob job)
        //		{
        //
        //			IEnumerable<IClient> clients = ClientSelectionMethod.GetBestClients (job, this);
        //
        //			//make tasks for all of them
        //			IDictionary<IClient, Task<IResult>> clientRunTasks;
        //
        //			clientRunTasks = clients.ToDictionary (c => c, c => new Task<IResult> (() => c.RunJob (job)));
        //
        //			//start the timer, run the tasks, wait till time is done
        //			var clientExecutionTime = new Stopwatch ();
        //			clientExecutionTime.Start ();
        //			foreach (var client in clientRunTasks.Values) {
        //				client.Start ();
        //			}
        //
        //			Task.WaitAll (clientRunTasks.Values.ToArray ());
        //			clientExecutionTime.Stop (); //stop the time now it's done
        //
        //			TimeSpan timeTaken = clientExecutionTime.Elapsed;
        //
        //			IDictionary<IClient, IResult> clientResults = new Dictionary<IClient, IResult> ();
        //			foreach (var client in clientRunTasks.Keys) {
        //				clientResults.Add (client, clientRunTasks [client].Result);
        //			}
        //
        //			IServerResult result = new SimpleImplementations.SimpleServerResult (
        //				                       timeTaken,
        //				                       clientResults,
        //				                       clientResults.Values.Any (r => r.Success),
        //				                       job);
        //
        //			return result as Task<IServerResult>;
        //		}

        public bool PingClient(IClient client) {
            return client.Ping();
        }

        //		public Task<bool> PingClientAsync (IClient client)
        //		{
        //			throw new NotImplementedException ();
        //		}

        public IClientSelection ClientSelectionMethod {
            get;
            set;
        }

        private void ClientConnected(object sender, ClientConnectedEventArgs e) {
            AddClient(e.Client);
        }
    }
}

