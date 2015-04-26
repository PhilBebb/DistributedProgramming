using Interfaces.Shared.Capabilities;
using System;
using System.Collections.Generic;
using Interfaces.Shared;

namespace TestImplimentation
{
	public class TestJob : IJob
	{
		public TestJob (string name, int id, IEnumerable<ICapability> requiredCapabilities, IDictionary<string, string> ParameterValues)
		{
			Name = name;
			Id = id;
			RequiredCapabilities = requiredCapabilities;
			ParameterValues = ParameterValues;
		}

		public string Name {
			get;
			set;
		}

		public int Id {
			get;
			private set;
		}

		public IEnumerable<ICapability> RequiredCapabilities {
			get;
			private set;
		}

		public IDictionary<string, string> ParameterValues {
			get;
			set;
		}

	}
}

