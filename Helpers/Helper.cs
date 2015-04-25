using System;

namespace Helpers
{
	public static class Helper
	{
		public static bool IsWindows {
			get {
				//shamelessly taken from Mono
				PlatformID platform = Environment.OSVersion.Platform;
				return platform == PlatformID.Win32S || platform == PlatformID.Win32Windows || platform == PlatformID.Win32NT || platform == PlatformID.WinCE;
			}
		}

	}
}

