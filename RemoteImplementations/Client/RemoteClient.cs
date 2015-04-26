using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Interfaces.Shared;
using SimpleImplementations;

namespace RemoteImplementations
{
	public class RemoteClient
	{
		private TcpClient _client = null;
		private Socket _sock = null;

		public RemoteClient ()
		{
			_client = new TcpClient ();
		}

		public async void Start (IPAddress address, int port)
		{
			Console.WriteLine ("Starting");
			_client.ConnectAsync (address, port);
			Console.WriteLine ("Connected");

			_sock = _client.Client;
			SendHelloAsync ();

			while (true) {
				Console.WriteLine ("Waiting for job");
				var job = await InternalHelper.ReadJobAsJsonAsync (_sock);
				Console.WriteLine ("job get!");

				IResult result = new SimpleResult (
					                 true,
					                 new Dictionary<string, string>{ { "p1", "a" }, { "p2", "b" } },
					                 job
				                 );
				Console.WriteLine ("Sending result");
				InternalHelper.SendJson (Helpers.Helper.ResultToJson (result), _sock);
				Console.WriteLine ("Result sent");
			}
		}

		private async Task<bool> SendHelloAsync ()
		{
			return true;
		}
	}
}
	