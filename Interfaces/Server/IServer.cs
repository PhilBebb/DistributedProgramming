using System;
using System.Collections.Generic;
using Interfaces.Shared;

namespace Interfaces.Server
{
	public interface IServer
	{
		IEnumerable<IClient> GetConnectedClients ();

		bool AddClient (IClient client);

		bool RemoveClient (IClient client);

		bool IsClientConnected (IClient client);

		IServerResult RubJob (IJob job);

		IClientSelection ClientSelectionMethod { get; set; }
	}
}

