using System;
using Interfaces.Shared.Capabilities;
using System.Diagnostics;

namespace Capabilities
{
	public class ScreenCapability : ICapability
	{
		public string Name {
			get {
				return "Screen";
			}
		}

		public bool HasCapability {
			get {
				if (Helpers.Helper.IsWindows) {
					return (Process.GetCurrentProcess ().MainWindowHandle != IntPtr.Zero);
				}
				//todo: work out how to actually figure this out on *nix
				return false; 
			}
		}

		public string CapabilityValue {
			get {
				return HasCapability.ToString ();
			}
		}

	}
}

