using System;
using System.Linq;
using Interfaces.Server;
using Interfaces.Shared;
using System.Collections.Generic;

namespace SimpleImplementations {
	/// <summary>
	/// Simple client selection.
	/// Always returns all clients that can perform the job
	/// </summary>
	public class SimpleClientSelection : IClientSelection {
		public SimpleClientSelection () {
		}

		public IEnumerable<IClient> GetBestClients (IJob jobToRun, IServer server) {
			return Helpers.Helper.GetPossibleClients (server.GetConnectedClients (), jobToRun.RequiredCapabilities);
		}

	}
}
