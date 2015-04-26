using System;
using Interfaces.Shared;
using System.Collections.Generic;

namespace Interfaces.Server
{
	public interface IServerResult : Interfaces.Shared.IResult
	{
		TimeSpan TimeTaken { get; }

		IDictionary<IClient, IResult> ClientResults{ get; }
	}
}

