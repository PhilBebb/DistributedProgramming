using System;
using System.Collections.Generic;
using Interfaces.Shared;
using System.Threading.Tasks;

namespace Interfaces.Server
{
	public interface IServer
	{
		IEnumerable<IClient> GetConnectedClients ();

		Task<IEnumerable<IClient>> GetConnectedClientsAsync ();

		bool AddClient (IClient client);

		bool RemoveClient (IClient client);

		bool IsClientConnected (IClient client);

		Task<bool> IsClientConnectedAsync (IClient client);

		IServerResult RubJob (IJob job);

		Task<IServerResult> RubJobAsync (IJob job);

		IClientSelection ClientSelectionMethod { get; set; }

		bool PingClient (IClient client);

		Task<bool> PingClientAsync (IClient client);
	}
}

