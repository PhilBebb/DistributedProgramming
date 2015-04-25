using Interfaces.Shared.Capabilities;
using System;
using System.Collections.Generic;

namespace Interfaces.Shared
{
	public interface IJob
	{
		string Name {
			get;
			set;
		}

		int Id {
			get;
		}

		IEnumerable<ICapability> RequiredCapabilities { get; }

		IDictionary<string, string> ParameterValues { get; set; }
	}
}

