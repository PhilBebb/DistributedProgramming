using System;

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

