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
			await _client.ConnectAsync (address, port);
			_sock = _client.Client;
			await SendHelloAsync ();

			while (true) {
				IJob job = InternalHelper.ReadJobAsJsonAsync (_sock).Result;

				IResult result = new SimpleResult (
					                 true,
					                 new Dictionary<string, string>{ { "p1", "a" }, { "p2", "b" } },
					                 job
				                 );
				InternalHelper.SendJson (Helpers.Helper.ResultToJson (result), _sock);
			}
		}

		private async Task<bool> SendHelloAsync ()
		{
			return true;
		}
	}
}
	