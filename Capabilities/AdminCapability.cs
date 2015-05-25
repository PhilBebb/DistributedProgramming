using System;
using Interfaces.Shared.Capabilities;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Capabilities {
	public class AdminCapability : ICapability {
		//[DllImport ("libc")]
		//public static extern uint getuid ();

		public override string Name {
			get {
				return "Admin";
			}
			set {
				//compiler complains if it doesn't exist
				throw new NotSupportedException ();
			}
		}

		public override bool HasCapability {
			get {
				bool isAdmin = false;
				try {
					//get the currently logged in user
					WindowsIdentity user = WindowsIdentity.GetCurrent ();
					WindowsPrincipal principal = new WindowsPrincipal (user);
					isAdmin = principal.IsInRole (WindowsBuiltInRole.Administrator);
				} catch (UnauthorizedAccessException) {
					isAdmin = false;
				} catch (Exception) {
					isAdmin = false;
				}
				return isAdmin;
			}
			set {
				//compiler complains if it doesn't exist
				throw new NotSupportedException ();
			}

			//return getuid () == 0;
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

