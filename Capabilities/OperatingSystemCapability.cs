using System;
using Interfaces.Shared.Capabilities;

namespace Capabilities {
	public class OperatingSystemCapability : ICapability {
		public override string Name {
			get {
				return "Operating System";
			}
			set {
				//compiler complains if it doesn't exist
				throw new NotSupportedException ();
			}
		}

		public override  bool HasCapability {
			get {
				return true;
			}
			set {
				//compiler complains if it doesn't exist
				throw new NotSupportedException ();
			}
		}

		public override  string CapabilityValue {
			get {
				return Enum.GetName (typeof(PlatformID), Environment.OSVersion.Platform);
			}
			set {
				//compiler complains if it doesn't exist
				throw new NotSupportedException ();
			}
		}
	}
}

