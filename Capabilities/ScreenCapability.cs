using System;
using Interfaces.Shared.Capabilities;
using System.Diagnostics;

namespace Capabilities {
	public class ScreenCapability : ICapability {
		public override string Name {
			get {
				return "Screen";
			}
			set {
				//compiler complains if it doesn't exist
				throw new NotSupportedException ();
			}
		}

		public override bool HasCapability {
			get {
				if (Helpers.Helper.IsWindows) {
					return (Process.GetCurrentProcess ().MainWindowHandle != IntPtr.Zero);
				}
				//todo: work out how to actually figure this out on *nix
				return false; 
			}
			set {
				//compiler complains if it doesn't exist
				throw new NotSupportedException ();
			}
		}

		public override string CapabilityValue {
			get {
				return HasCapability.ToString ();
			}
			set {
				//compiler complains if it doesn't exist
				throw new NotSupportedException ();
			}
		}

	}
}

