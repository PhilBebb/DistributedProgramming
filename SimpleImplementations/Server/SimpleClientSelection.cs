using System;
using Interfaces.Server;
using Interfaces.Shared;
using System.Collections.Generic;

namespace SimpleImplementations
{
	public class SimpleClientSelection : IClientSelection
	{
		public SimpleClientSelection ()
		{
		}

		public IEnumerable<IClient> GetBestClients (IJob jobToRun, IServer server)
		{
			return server.GetConnectedClients ();
		}

	}
}
