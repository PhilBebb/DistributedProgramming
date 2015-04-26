using System;
using Interfaces.Shared;
using System.Collections.Generic;
using Interfaces.Shared.Capabilities;

namespace Helpers.Internal
{
	internal class InternalJob : IJob
	{
		public InternalJob ()
		{
			
		}

		public InternalJob (IJob job)
		{
			Name = job.Name;
			Id = job.Id;
			RequiredCapabilities = job.RequiredCapabilities;
			ParameterValues = job.ParameterValues;
		}

		public string Name {
			get;
			set;
		}

		public int Id {
			get;
			set;
		}

		public IEnumerable<ICapability> RequiredCapabilities {
			get;
			set;
		}

		public IDictionary<string, string> ParameterValues {
			get;
			set;
		}

	}
}

