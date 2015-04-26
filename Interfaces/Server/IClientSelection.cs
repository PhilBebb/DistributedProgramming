using System;
using Interfaces.Shared;
using System.Collections.Generic;

namespace Interfaces.Server
{
	public interface IClientSelection
	{
		/// <summary>
		/// Gets the best clients for a job.
		/// This can be one or many
		/// </summary>
		/// <returns>The next client(s).</returns>
		IEnumerable<IClient> GetBestClients (IJob jobToRun, IServer server);
	}
}

