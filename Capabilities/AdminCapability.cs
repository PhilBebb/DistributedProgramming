using System;
using Interfaces.Shared.Capabilities;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Capabilities
{
	public class AdminCapability : ICapability
	{
		//[DllImport ("libc")]
		//public static extern uint getuid ();

		public string Name {
			get {
				return "Admin";
			}
		}

		public bool HasCapability {
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

			//return getuid () == 0;
		}

		public string CapabilityValue {
			get {
				return HasCapability.ToString ();
			}
		}

	}
}

