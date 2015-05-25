using System;
using Interfaces.Shared.Capabilities;

namespace UnitTests {
	public class MockCapability : ICapability {
		public MockCapability (string name, bool hasCapability) {
			Name = name;
			HasCapability = hasCapability;
			CapabilityValue = hasCapability.ToString ();
		}

		public override string Name {
			get;
			set;
		}

		public override bool HasCapability {
			get;
			set;
		}

		public override string CapabilityValue {
			get;
			set;
		}

	}
}

