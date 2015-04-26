using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces.Shared;
using Interfaces.Shared.Capabilities;

namespace TestImplimentation
{
	public class TestJob : IJob
	{
		private List<ICapability> _requiredCapabilities;

		public TestJob (string name, int id, IEnumerable<ICapability> requiredCapabilities, IDictionary<string, string> parameterValues)
		{
			Name = name;
			Id = id;
			_requiredCapabilities = new List<ICapability> ();
			if (null != requiredCapabilities) {
				_requiredCapabilities.AddRange (requiredCapabilities);
			}
			ParameterValues = new Dictionary<string, string> ();
			if (null != parameterValues) {
				foreach (string key in parameterValues.Keys) {
					ParameterValues.Add (key, parameterValues [key]);
				}
			}
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
			get { return _requiredCapabilities; }
		}

		public IDictionary<string, string> ParameterValues {
			get;
			set;
		}

	}
}

