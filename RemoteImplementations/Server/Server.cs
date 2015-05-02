using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.Server;
using Interfaces.Shared;
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

		private object _lock = new object ();
		private List<IClient> _clients = new List<IClient> ();

		public Server (int port, IClientSelection clientSelection) {
			_listener = new TcpListener (new IPEndPoint (IPAddress.Any, port));
			ClientSelectionMethod = clientSelection;
		}

		public void Start () {
			Log ("Starting...");
			_listener.Start ();
			Log ("Started");

			//while (true) {
			try {
				var tcpClient = _listener.AcceptTcpClient ();
				Task.Factory.StartNew (() => HandleConnection (tcpClient));

			} catch (Exception ex) {
				Log (string.Format ("Exception : {0}", ex.Message));
			}
			//}
		}

		private void HandleConnection (TcpClient tcpClient) {			
			string clientInfo = tcpClient.Client.RemoteEndPoint.ToString ();
			Log (string.Format ("Got connection request from {0}", clientInfo));

			if (!tcpClient.Connected) {
				return;
			}
			RemoteNetworkedClient remote = new RemoteNetworkedClient (tcpClient, clientInfo);
			AddClient (remote);
		}

		private void Log (
			string message,
			[CallerMemberName]string callername = "") {
			System.Console.WriteLine (
				"[{0}] - Thread-{1}- {2}",
				callername,
				Thread.CurrentThread.ManagedThreadId, message);
		}

		public IEnumerable<IClient> GetConnectedClients () {
			return _clients.AsParallel ().Where (c => c.Ping ());
		}

		//		public Task<IEnumerable<IClient>> GetConnectedClientsAsync ()
		//		{
		//			throw new NotImplementedException ();
		//		}

		public bool AddClient (IClient client) {
			lock (_lock) {
				if (!_clients.Contains (client)) {
					_clients.Add (client);
					return true;
				}
			}
			return false;
		}

		public bool RemoveClient (IClient client) {
			lock (_lock) {
				if (_clients.Contains (client)) {
					_clients.Remove (client);
					return true;
				}
			}
			return false;
		}

		public bool IsClientConnected (IClient client) {
			throw new NotImplementedException ();
		}

		//		public Task<bool> IsClientConnectedAsync (IClient client)
		//		{
		//			throw new NotImplementedException ();
		//		}

		public IServerResult RubJob (IJob job) {
			IEnumerable<IClient> clients = ClientSelectionMethod.GetBestClients (job, this);
			
			//make tasks for all of them
			IDictionary<IClient, Task<IResult>> clientRunTasks;
			
			clientRunTasks = clients.ToDictionary (c => c, c => new Task<IResult> (() => c.RunJob (job)));
			
			//start the timer, run the tasks, wait till time is done
			var clientExecutionTime = new Stopwatch ();
			clientExecutionTime.Start ();
			foreach (var client in clientRunTasks.Values) {
				client.Start ();
			}
			
			Task.WaitAll (clientRunTasks.Values.ToArray ());
			clientExecutionTime.Stop (); //stop the time now it's done
			
			TimeSpan timeTaken = clientExecutionTime.Elapsed;
			
			IDictionary<IClient, IResult> clientResults = new Dictionary<IClient, IResult> ();
			foreach (var client in clientRunTasks.Keys) {
				clientResults.Add (client, clientRunTasks [client].Result);	
			}
			
			IServerResult result = new SimpleImplementations.SimpleServerResult (
				                       timeTaken,
				                       clientResults,
				                       clientResults.Values.Any (r => r.Success),
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

		public bool PingClient (IClient client) {
			return client.Ping ();
		}

		//		public Task<bool> PingClientAsync (IClient client)
		//		{
		//			throw new NotImplementedException ();
		//		}

		public IClientSelection ClientSelectionMethod {
			get;
			set;
		}

	}
}

