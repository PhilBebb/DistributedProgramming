using System;
using Interfaces.Shared;
using System.Collections.Generic;
using Interfaces.Shared.Capabilities;

namespace UnitTests {
	public class MockJob : IJob {
		
		public string Name {
			get;
			set;
		}

		public int Id {
			get;
			set;
		}

		public System.Collections.Generic.IEnumerable<ICapability> RequiredCapabilities {
			get;
			set;
		}

		public System.Collections.Generic.IDictionary<string, string> ParameterValues {
			get;
			set;
		}

		public MockJob (string name, int id) {
			Name = name;
			Id = id;
			RequiredCapabilities = new List<ICapability> ();
			ParameterValues = new Dictionary<string, string> ();
		}

		public MockJob (string name) : this (name, DateTime.UtcNow.Millisecond) {
		}

		public MockJob (int id) : this (DateTime.UtcNow.ToString (), id) {
		}

		public MockJob () : this (DateTime.UtcNow.ToString (), DateTime.UtcNow.Millisecond) {
		}

	}
}

