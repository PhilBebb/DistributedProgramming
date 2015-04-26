using System;
using Interfaces.Server;
using Interfaces.Shared;
using System.Collections.Generic;

namespace TestImplimentation
{
	public class TestClientSecetion : IClientSelection
	{
		public TestClientSecetion ()
		{
		}

		public IEnumerable<IClient> GetBestClients (IJob jobToRun, IServer server)
		{
			return server.GetConnectedClients ();
		}

	}
}

