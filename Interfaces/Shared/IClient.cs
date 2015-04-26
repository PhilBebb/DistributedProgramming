using System;
using System.Collections.Generic;
using Interfaces.Shared.Capabilities;

namespace Interfaces.Shared
{
	public interface IClient
	{
		string Name {
			get;
			set;
		}

		int Id {
			get;
		}

		IResult RunJob (IJob job);

		bool Ping ();

		IEnumerable<ICapability> Capabilities { get; }
	}
}

