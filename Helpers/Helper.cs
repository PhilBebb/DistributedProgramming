using System;
using Interfaces.Shared;
using Helpers.Internal;

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

		public static string JobToJson (IJob job)
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject (job);
		}

		public static IJob JsonToJob (string jobJson)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<InternalJob> (jobJson);
		}

		public static IResult JsonToResult (string resultJson)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<InternalResult> (resultJson);
		}
	}
}

