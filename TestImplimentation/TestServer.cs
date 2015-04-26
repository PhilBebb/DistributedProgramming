using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.Server;
using Interfaces.Shared;
using Interfaces;
using System.Diagnostics;
using System.Collections;

namespace TestImplimentation
{
	public class TestServer : IServer
	{
		public Task<IEnumerable<IClient>> GetConnectedClientsAsync ()
		{
			return Task.Factory.StartNew (() => GetConnectedClients ());
		}

		public Task<bool> IsClientConnectedAsync (IClient client)
		{
			return Task.Factory.StartNew (() => IsClientConnected (client));
		}

		public Task<IServerResult> RubJobAsync (IJob job)
		{
			return Task.Factory.StartNew (() => RubJob (job));
		}

		public Task<bool> PingClientAsync (IClient client)
		{
			return Task.Factory.StartNew (() => PingClient (client));
		}

		public bool PingClient (IClient client)
		{
			return client.Ping ();
		}

		private IList<IClient> _connectedClients = new List<IClient> ();
		private readonly object _lock = new object ();

		public IEnumerable<IClient> GetConnectedClients ()
		{
			return _connectedClients;
		}

		public bool AddClient (IClient client)
		{
			lock (_lock) {
				if (!_connectedClients.Contains (client)) {
					_connectedClients.Add (client);
					return true;
				}
			}
			return false;
		}

		public bool RemoveClient (IClient client)
		{
			lock (_lock) {
				if (_connectedClients.Contains (client)) {
					_connectedClients.Remove (client);
					return true;
				}
			}
			return false;
		}

		public bool IsClientConnected (IClient client)
		{
			if (!_connectedClients.Contains (client))
				return false;

			return client.Ping ();
		}

		public IServerResult RubJob (IJob job)
		{
			//get best client(s)
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

			IServerResult result = new TestServerResult (
				                       timeTaken,
				                       clientResults,
				                       clientResults.Values.Any (r => r.Success),
				                       job);
			return result;
		}

		private IClientSelection _clientSelectionMethod = new TestClientSecetion ();

		public IClientSelection ClientSelectionMethod {
			get {
				return _clientSelectionMethod;
			}
			set {
				_clientSelectionMethod = value;
			}
		}

		public TestServer ()
		{
		}
	}
}

