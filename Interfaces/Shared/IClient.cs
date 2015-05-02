using System;
using System.Collections.Generic;
using Interfaces.Shared.Capabilities;
using System.Threading.Tasks;

namespace Interfaces.Shared {
	public interface IClient {
		string Name {
			get;
			set;
		}

		int Id {
			get;
		}

		IResult RunJob (IJob job);

		//Task<IResult> RunJobAsync (IJob job);

		bool Ping ();

		IEnumerable<ICapability> Capabilities { get; }
	}
}

