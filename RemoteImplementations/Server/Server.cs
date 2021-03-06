﻿using System;
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
		private Thread _receiverThread = null;

		public Server (int port, IClientSelection clientSelection) {
			_listener = new TcpListener (new IPEndPoint (IPAddress.Any, port));
			ClientSelectionMethod = clientSelection;
		}

		public void Start () {
			Log ("Starting...");
			_listener.Start ();
			Log ("Started");
			ListenForClient ();
		}

		public void StartThreaded () {
			Log ("Starting...");
			_listener.Start ();
			Log ("Started");

			//kill it to make sure
			StopListenerThread ();

			//and now safely create it again
			StartListenerThread ();
		}

		private void StartListenerThread () {
			if (_receiverThread == null) {
				lock (this) {
					if (_receiverThread == null) {
						_receiverThread = new Thread (new ParameterizedThreadStart (ListenForClient));
						_receiverThread.Start (true);
					}
				}
			}
		}

		private void StopListenerThread () {
			if (_receiverThread != null) {
				lock (this) {
					if (_receiverThread != null) {
						_receiverThread.Abort ();
						_receiverThread = null;
					}
				}
			}
		}

		public bool IsRunningThreadded () {
			if (_receiverThread != null) {
				lock (this) {
					if (_receiverThread != null) {
						return _receiverThread.ThreadState == System.Threading.ThreadState.Running;
					}
				}
			}
			return false;
		}

		private void ListenForClient (object o) {
			ListenForClient ((bool)o);
		}

		private void ListenForClient (bool loopForever = false) {
			do {
				try {
					var tcpClient = _listener.AcceptTcpClient ();
					Task.Factory.StartNew (() => HandleConnection (tcpClient));

				} catch (Exception ex) {
					Log (string.Format ("Exception : {0}", ex.Message));
				}
			} while (loopForever);
		}

		public void Stop () {
			_listener.Stop ();
			try {
				_listener.Server.Dispose ();
			} catch (Exception ex) {
				Log (ex.Message);
				throw ex;
			}

			StopListenerThread ();

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

		private void Log (string message, [CallerMemberName] string callername = "") {
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
			return client.Ping ();
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
			var clientExecutionTime = Stopwatch.StartNew ();

			foreach (var client in clientRunTasks.Values.AsParallel ()) {
				client.Start ();
			}

			try {
				Task.WaitAll (clientRunTasks.Values.ToArray ());
			} catch (AggregateException ex) {
				foreach (var e in ex.InnerExceptions) {
					//?
				}
			}

			clientExecutionTime.Stop (); //stop the time now it's done

			TimeSpan timeTaken = clientExecutionTime.Elapsed;

			IDictionary<IClient, IResult> clientResults = new Dictionary<IClient, IResult> ();
			foreach (var client in clientRunTasks.Keys) {

				try {
					clientResults.Add (client, clientRunTasks [client].Result);
				} catch (AggregateException ex) {

					var resultsDictionary = new Dictionary<string, string> () {
						{ "Error", ex.Message }
					};

					foreach (Exception e in ex.InnerExceptions) {
						resultsDictionary.Add (e.Message, e.StackTrace);
					}

					clientResults.Add (client, new SimpleImplementations.SimpleResult (
						false,
						resultsDictionary
                        , job));
				}
			}

			IServerResult result = new SimpleImplementations.SimpleServerResult (
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

