using System;
using Interfaces.Shared.Capabilities;

namespace Capabilities
{
	public class OperatingSystemCapability : ICapability
	{
		public string Name {
			get {
				return "Operating System";
			}
		}

		public bool HasCapability {
			get {
				return true;
			}
		}

		public string CapabilityValue {
			get {
				return Enum.GetName (typeof(PlatformID), Environment.OSVersion.Platform);
			}
		}
	}
}

